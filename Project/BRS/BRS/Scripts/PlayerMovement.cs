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

           
            float rotationOld = rotation;
            rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);
            //transform.eulerAngles = new Vector3(0, rotation, 0);

            //move forward
            float speedboost = boosting ? boostSpeedMultiplier : 1f;
            Vector3 linearVelocity = transform.toLocalRotation(Vector3.Forward * currentSpeed * speedboost * smoothMagnitude);
            //transform.Translate(linearVelocity);

            // Apply forces/changes to physics
            RigidBodyComponent rigidBodyComponent = gameObject.GetComponent<RigidBodyComponent>();
            rigidBodyComponent.RigidBody.LinearVelocity = Conversion.ToJitterVector(linearVelocity);
            rigidBodyComponent.RigidBody.Orientation = JMatrix.CreateRotationY(rotation * MathHelper.Pi / 180.0f);
            //rigidBodyComponent.RigidBody.AngularVelocity = new JVector(0, (-rotationOld + rotation), 0);
            rigidBodyComponent.RigidBody.Mass = 10;

            rigidBodyComponent.RigidBody.AddForce(Conversion.ToJitterVector(linearVelocity));
            Debug.Log(rigidBodyComponent.RigidBody.Position.ToString());

            _previousLinearVelocity = linearVelocity;
        }


        // queries
        float currentSpeed { get { return MathHelper.Lerp(maxSpeed, minSpeed, playerInventory.MoneyPercent); } }




        // other

    }

}