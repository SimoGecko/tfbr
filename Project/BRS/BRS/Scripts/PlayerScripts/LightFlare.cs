// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using Microsoft.Xna.Framework;
using System.ComponentModel.DataAnnotations;
using BRS.Engine.Utilities;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Initialize the dynamic shadow which will be synchronised with the position of the game-object
    /// </summary>
    class LightFlare : Component {

        // --------------------- VARIABLES ---------------------

        public enum LightType {
            [Display(Description = "lightYellow")]
            LightYellow,
            [Display(Description = "lightBlue")]
            LightBlue,
            [Display(Description = "lightRed")]
            LightRed
        }

        public enum UpdateFunction { SinPositive, SinAbsolute }

        //public

        //private
        private bool _isInitialized;
        private readonly Vector3 _offset;
        private readonly Quaternion _localOrientation;
        private readonly LightType _lightType;
        private readonly UpdateFunction _updateFunction;
        private float _alphaStart;
        private float _min, _max;
        private float _speed = 10.0f;

        // const

        //reference
        private Transform _target;
        private GameObject _light;
        private Follower _follower;

        // --------------------- BASE METHODS ------------------

        public LightFlare(LightType lightType, Vector3 offset, Quaternion localOrientation, float alphaStart, UpdateFunction updateFunction, float min = 0.0f, float max = 1.0f) {
            _lightType = lightType;
            _offset = offset;
            _localOrientation = localOrientation;
            _alphaStart = alphaStart;
            _updateFunction = updateFunction;
            _min = min;
            _max = max;
        }

        public override void Awake() {
            if (_isInitialized) {
                return;
            }

            _target = gameObject.transform;
            _light = GameObject.Instantiate(_lightType.GetDescription(), _target);
            _follower = new Follower(_light, _offset, _localOrientation);

            if (_updateFunction == UpdateFunction.SinPositive) {
                _alphaStart = MyRandom.Value * (float)Math.PI;
                _speed = MyRandom.Range(10.0f, 15.0f);
                _min = MyRandom.Range(0.00f, 0.08f);
                _max = MyRandom.Range(0.10f, 0.18f);
            }

            CalculateLightSize();

            _isInitialized = true;
        }

        public override void Start() {
            if (gameObject.HasComponent<MovingRigidBody>()) {
                gameObject.GetComponent<MovingRigidBody>().AddSyncedObject(_follower);
            }
            if (gameObject.HasComponent<AnimatedRigidBody>()) {
                gameObject.GetComponent<AnimatedRigidBody>().AddSyncedObject(_follower);
            }
            if (gameObject.HasComponent<DynamicRigidBody>()) {
                gameObject.GetComponent<DynamicRigidBody>().AddSyncedObject(_follower);
            }
        }

        public override void Update() {
            float newAlpha = _light.material.Alpha;
            float t = _alphaStart + _speed * (float)Time.Gt.TotalGameTime.TotalSeconds;
            float sinValue = (float)Math.Sin(t);

            switch (_updateFunction) {
                case UpdateFunction.SinPositive:
                    newAlpha = (sinValue + 1) * (_max - _min) / 2 + _min;
                    break;

                case UpdateFunction.SinAbsolute:
                    newAlpha = sinValue;
                    break;
            }

            _light.material.Alpha = MathHelper.Clamp(newAlpha, 0.0f, 1.0f);
        }

        public override void Destroy() {
            GameObject.Destroy(_light);
        }

        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other

        private void CalculateLightSize() {
            _light.transform.scale = new Vector3(0.1f, 1.0f, 0.1f);
        }

    }
}