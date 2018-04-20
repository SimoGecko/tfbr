﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Deals with the movement of the player around the map
    /// </summary>
    class PlayerMovement : Component {

        // --------------------- VARIABLES ---------------------

        //public
        public float Speed;

        //private
        private float _rotation;
        private float _smoothMagnitude, _refMagnitude;
        private float _refangle, _refangle2;
        private float _inputAngle;
        private float _targetRotation;

        // const
        private const float MinSpeed = 3f;
        private const float MaxSpeed = 7f;
        private const float MaxTurningRate = 360; // deg/sec
        private const float BoostSpeedMultiplier = 1.5f;

        private const float SlowdownMalus = .3f;
        private const float SpeedPadMultiplier = 2f;


        //BOOST
        public bool Boosting;
        public bool PowerupBoosting;


        //SLOWDOWN
        bool _slowdown;
        public bool SpeedPad;

        //reference
        PlayerInventory playerInventory;
        private SteerableCollider _collider;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //transform.Rotate(Vector3.Up, -90);
            //rotation = targetRotation = -90;

            playerInventory = gameObject.GetComponent<PlayerInventory>();

            MovingRigidBody dynamicRigidBody = gameObject.GetComponent<MovingRigidBody>();
            _collider = dynamicRigidBody?.RigidBody as SteerableCollider;
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Move(Vector3 input) {
            float magnitude = Utility.Clamp01(input.Length());
            _smoothMagnitude = Utility.SmoothDamp(_smoothMagnitude, magnitude, ref _refMagnitude, .1f);

            //rotate towards desired angle
            if (_smoothMagnitude > .05f) { // avoid changing if 0
                _inputAngle = MathHelper.ToDegrees((float)Math.Atan2(input.Z, input.X));
                _inputAngle = Utility.WrapAngle(_inputAngle, _targetRotation);
                _targetRotation = Utility.SmoothDampAngle(_targetRotation, _inputAngle - 90, ref _refangle, .3f, MaxTurningRate * _smoothMagnitude);
            } else {
                _targetRotation = Utility.SmoothDampAngle(_targetRotation, _rotation, ref _refangle2, .3f, MaxTurningRate * _smoothMagnitude);
            }

            // Calculate rotation
            _rotation = MathHelper.Lerp(_rotation, _targetRotation, _smoothMagnitude);

            // Calculate velocity
            float speedboost = Boosting || PowerupBoosting ? BoostSpeedMultiplier : 1f;
            speedboost *= _slowdown ? SlowdownMalus : 1f;

            if (SpeedPad) { // override and force to move at max speed
                Speed = CapacityBasedSpeed * SpeedPadMultiplier;
            } else {
                Speed = CapacityBasedSpeed * speedboost * _smoothMagnitude;
            }

            Vector3 linearVelocity = Vector3.Forward * Speed;


            // Apply forces/changes to physics
            // Todo: Handle steering correctly
            if (_collider != null) {
                _collider.RotationY = MathHelper.ToRadians(_rotation);
                _collider.Speed = JVector.Transform(Conversion.ToJitterVector(linearVelocity) * 3, _collider.Orientation);
            }
        }

        public void SetSlowdown(bool b) {
            _slowdown = b;
        }

        internal void SetSpeedPad(bool b) {
            SpeedPad = b;
        }

        public void ResetSmoothMatnitude() {
            _smoothMagnitude = 0.0f;
        }


        // queries
        float CapacityBasedSpeed { get { return MathHelper.Lerp(MaxSpeed, MinSpeed, playerInventory.MoneyPercent); } }




        // other

    }

}