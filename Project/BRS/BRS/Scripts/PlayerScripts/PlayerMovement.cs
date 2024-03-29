﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using System;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Deals with the movement of the player around the map
    /// </summary>
    class PlayerMovement : Component {

        // --------------------- VARIABLES ---------------------

        //public
        // const
        private float MinSpeed = 4f;
        private float MaxSpeed = 7f; // Todo: As soon as it is built in Release-mode, 7 is too fast
        private const float MaxTurningRate = 360*5;// deg/sec

        private const float BoostSpeedMultiplier = 1.5f;
        private const float SlowdownMalus = .3f;
        private const float SpeedPadMultiplier = 2f;

        //private
        public float Speed;

        private float _smoothMagnitude, _refMagnitude;
        private float _refangle, _refangle2;
        private float _inputAngle;
        private float _rotation;
        private float _targetRotation;


        //BOOST
        public bool Boosting;
        public bool PowerupBoosting;

        //SLOWDOWN
        private bool _slowdown;
        public bool SpeedPad;

        // drove through oil
        public bool OilTracks;

        //reference
        PlayerInventory playerInventory;
        private SteerableCollider _collider;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            playerInventory = gameObject.GetComponent<PlayerInventory>();

            MovingRigidBody dynamicRigidBody = gameObject.GetComponent<MovingRigidBody>();
            _collider = dynamicRigidBody?.RigidBody as SteerableCollider;

            // Reset all variables to the start-values
            Reset();
        }

        public override void Update() {

        }

        public override void Reset() {
            _smoothMagnitude = _refMagnitude = 0;
            _refangle = _refangle2 = 0;
            _inputAngle = 0;
            _rotation = _targetRotation = 0;

            _slowdown = SpeedPad = false;
            Boosting = PowerupBoosting = false;
            OilTracks = false;
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

            Vector3 linearVelocity = Vector3.Forward * Speed * Time.DeltaTime;

            // If physics is available apply the forces/changes to it, otherwise to the gameobject itself
            if (_collider != null) {
                _collider.RotationY = MathHelper.ToRadians(_rotation);
                _collider.Speed = JVector.Transform(Conversion.ToJitterVector(linearVelocity) * 200,
                    _collider.Orientation);
            } else {
                transform.Translate(linearVelocity * Time.DeltaTime);
                transform.rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, _rotation);
                transform.eulerAngles = new Vector3(0, _rotation, 0);
            }
        }

        public void SetSlowdown(bool b) {
            _slowdown = b;
        }
        public void SetOilTracks(bool b) {
            OilTracks = b;
        }
        internal void SetSpeedPad(bool b) {
            SpeedPad = b;
        }
        public void SetMaxSpeed(float f) {
            MaxSpeed = f;
        }
        public void SetMinSpeed(float f) {
            MinSpeed = f;
        }

        public void ResetSmoothMatnitude() {
            _smoothMagnitude = 0.0f;
            _refMagnitude = 0.0f;
        }
        

        // queries
        float CapacityBasedSpeed { get { return MathHelper.Lerp(MaxSpeed, MinSpeed, playerInventory.MoneyPercent); } }


        // other

        /// <summary>
        /// Reset the rotation of the player to the given value.
        /// </summary>
        /// <param name="rotation">Rotation given as degrees.</param>
        public void ResetRotation(float rotation) {
            _rotation = rotation;
            _targetRotation = rotation;
        }

    }

}