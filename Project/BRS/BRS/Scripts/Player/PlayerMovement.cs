// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
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
        const float maxTurningRate = 720; // deg/sec
        const float boostSpeedMultiplier = 1.5f;

        const float slowdownMalus = .3f;
        const float speedPadMultiplier = 2f;

        //private
        float rotation;
        float smoothMagnitude, refMagnitude;
        float refangle, refangle2;
        float inputAngle;
        float targetRotation;


        //BOOST
        public bool boosting;
        public bool powerupBoosting;

        //SLOWDOWN
        bool slowdown = false;
        bool speedPad = false;

        //reference
        PlayerInventory playerInventory;
        private SteerableRigidBody _rigidBody;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //transform.Rotate(Vector3.Up, -90);
            //rotation = targetRotation = -90;

            playerInventory = gameObject.GetComponent<PlayerInventory>();

            MovingRigidBody dynamicRigidBody = gameObject.GetComponent<MovingRigidBody>();
            _rigidBody = dynamicRigidBody?.RigidBody as SteerableRigidBody;
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

            // Calculate rotation
            rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);

            // Calculate velocity
            float speedboost = boosting || powerupBoosting ? boostSpeedMultiplier : 1f;
            speedboost *= slowdown ? slowdownMalus : 1f;

            Vector3 linearVelocity;
            if (speedPad) { // override and force to move at max speed
                linearVelocity = Vector3.Forward * capacityBasedSpeed * speedPadMultiplier;
            } else {
                linearVelocity = Vector3.Forward * capacityBasedSpeed * speedboost * smoothMagnitude;
            }


            // Apply forces/changes to physics
            // Todo: Handle steering correctly
            if (_rigidBody != null) {
                _rigidBody.RotationY = MathHelper.ToRadians(rotation);
                _rigidBody.Speed = JVector.Transform(Conversion.ToJitterVector(linearVelocity) * 3, _rigidBody.Orientation);
            }
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