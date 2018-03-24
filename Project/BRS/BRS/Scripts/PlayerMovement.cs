﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Scripts.Physics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerMovement : Component {
        ////////// Deals with the movement of the player around the map //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float minSpeed = 5f;
        const float maxSpeed = 10f;
        const float maxTurningRate = 360; // deg/sec
        const float boostSpeedMultiplier = 1.5f;

        //private
        float rotation;
        float smoothMagnitude, refMagnitude;
        float refangle, refangle2;
        float inputAngle;
        float targetRotation;
        private Vector3 _previousLinearVelocity = Vector3.Zero;

        //BOOST
        public bool boosting;
        public bool powerupBoosting;

        //reference
        PlayerInventory playerInventory;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //transform.Rotate(Vector3.Up, -90);
            //rotation = targetRotation = -90;

            playerInventory = gameObject.GetComponent<PlayerInventory>();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Move(Vector3 input) {
            float magnitude = Utility.Clamp01(input.Length());
            smoothMagnitude = Utility.SmoothDamp(smoothMagnitude, magnitude, ref refMagnitude, .1f);

            //rotate towards desired angle
            if (smoothMagnitude > .05f) { // avoid changing if 0
                inputAngle = MathHelper.ToDegrees((float)Math.Atan2(input.Z, input.X));
                inputAngle = Utility.WrapAngle(inputAngle, targetRotation);
                targetRotation = Utility.SmoothDampAngle(targetRotation, inputAngle - 90, ref refangle, .3f, maxTurningRate * smoothMagnitude);
            } else {
                targetRotation = Utility.SmoothDampAngle(targetRotation, rotation, ref refangle2, .3f, maxTurningRate * smoothMagnitude);
            }

            rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);
            float speedboost = boosting || powerupBoosting ? boostSpeedMultiplier : 1f;

            //// move
            //transform.eulerAngles = new Vector3(0, rotation, 0);
            //transform.Translate(Vector3.Forward * currentSpeed * speedboost * smoothMagnitude * Time.deltatime);

            // Apply forces/changes to physics
            //gameObject.Position = new JVector(transform.position.X, 0.5f, transform.position.Z);
            //gameObject.Orientation = JMatrix.CreateRotationY(rotation * MathHelper.Pi / 180.0f);


            //rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);
            //transform.eulerAngles = new Vector3(0, rotation, 0);

            //move forward
            //float speedboost = boosting ? boostSpeedMultiplier : 1f;
            Vector3 linearVelocity = transform.toLocalRotation(Vector3.Forward * currentSpeed * speedboost * smoothMagnitude);
            //transform.Translate(linearVelocity);

            // Apply forces/changes to physics
            // Todo: Handle steering correctly
            DynamicRigidBody rigidBody = gameObject.GetComponent<DynamicRigidBody>();

            if (rigidBody != null) {
                rigidBody.RigidBody.LinearVelocity = new JVector(linearVelocity.X, 0, linearVelocity.Z);
                rigidBody.RigidBody.Position = new JVector(rigidBody.RigidBody.Position.X, 1f,
                    rigidBody.RigidBody.Position.Z);
                rigidBody.RigidBody.Orientation = JMatrix.CreateRotationY(rotation * MathHelper.Pi / 180.0f);
                //rigidBodyComponent.RigidBody.AngularVelocity = new JVector(0, (-rotationOld + rotation), 0);
                rigidBody.RigidBody.Mass = 10;

                rigidBody.RigidBody.AddForce(Conversion.ToJitterVector(linearVelocity));
                //rigidBodyComponent.RigidBody.AddForce(new JVector(100, 0, 0));
                Debug.Log(rigidBody.RigidBody.Position.ToString());
            }

            _previousLinearVelocity = linearVelocity;
        }


        // queries
        float currentSpeed { get { return MathHelper.Lerp(maxSpeed, minSpeed, playerInventory.MoneyPercent); } }




        // other

    }

}