﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Scripts.Physics;
using Jitter.Dynamics;
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

        const float slowdownMalus = .3f;
        const float speedPadMultiplier = 2f;

        //private
        float rotation, rotationOld;
        float smoothMagnitude, refMagnitude;
        float refangle, refangle2;
        float inputAngle;
        float targetRotation;
        private Vector3 _previousLinearVelocity = Vector3.Zero;

        

        //BOOST
        public bool boosting;
        public bool powerupBoosting;

        //SLOWDOWN
        bool slowdown = false;
        bool speedPad = false;

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
            speedboost *= slowdown ? slowdownMalus : 1f;

            //// move
            transform.eulerAngles = new Vector3(0, rotation, 0);
            //transform.Translate(Vector3.Forward * currentSpeed * speedboost * smoothMagnitude * Time.deltatime);

            //move forward
            //compute final speed
            if (speedPad) { // override and force to move at max speed
                transform.Translate(Vector3.Forward * capacityBasedSpeed * speedPadMultiplier * Time.deltaTime);
            } else {
                transform.Translate(Vector3.Forward * capacityBasedSpeed * speedboost * smoothMagnitude * Time.deltaTime);
            }
            return;


            // Apply forces/changes to physics
            //gameObject.Position = new JVector(transform.position.X, 0.5f, transform.position.Z);
            //gameObject.Orientation = JMatrix.CreateRotationY(rotation * MathHelper.Pi / 180.0f);


            //rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);
            //transform.eulerAngles = new Vector3(0, rotation, 0);

            //move forward
            //float speedboost = boosting ? boostSpeedMultiplier : 1f;
            Vector3 linearVelocity = Vector3.Forward * capacityBasedSpeed * speedboost * smoothMagnitude;
            //transform.Translate(linearVelocity);

            // Apply forces/changes to physics
            // Todo: Handle steering correctly
            MovingRigidBody dynamicRigidBody = gameObject.GetComponent<MovingRigidBody>();

            if (dynamicRigidBody != null) {
                RigidBody rb = dynamicRigidBody.RigidBody;
                
                JVector lv = JVector.Transform(Conversion.ToJitterVector(linearVelocity), rb.Orientation);
                
                rb.LinearVelocity = new JVector(lv.X, 0, lv.Z);

                rb.Orientation = JMatrix.CreateRotationY(rotation * MathHelper.Pi / 180.0f);

                //rb.LinearVelocity = (new JVector(0, 0, 10));
                //rigidBodyComponent.RigidBody.AddForce(new JVector(100, 0, 0));
                //Debug.Log(rb.Position.ToString());

                //string ori = String.Format("{9:0.00} results in:\n{0:0.00} {1:0.00} {2:0.00}\n{3:0.00} {4:0.00} {5:0.00}\n{6:0.00} {7:0.00} {8:0.00}",
                //    rb.Orientation.M11,
                //    rb.Orientation.M12,
                //    rb.Orientation.M13,
                //    rb.Orientation.M21,
                //    rb.Orientation.M22,
                //    rb.Orientation.M23,
                //    rb.Orientation.M31,
                //    rb.Orientation.M32,
                //    rb.Orientation.M33,
                //    rotation * MathHelper.Pi / 180.0f
                //    );
                //Debug.Log(ori);
            }

            _previousLinearVelocity = linearVelocity;
            rotationOld = rotation;
        }

        public void SetSlowdown(bool b) {
            slowdown = b;
        }
        internal void SetSpeedPad(bool b) {
            speedPad = b;
        }


        // queries
        float capacityBasedSpeed { get { return MathHelper.Lerp(maxSpeed, minSpeed, playerInventory.MoneyPercent); } }




        // other

    }

}