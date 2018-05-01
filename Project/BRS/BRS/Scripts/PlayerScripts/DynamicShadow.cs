// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Initialize the dynamic shadow which will be synchronised with the position of the game-object
    /// </summary>
    class DynamicShadow : Component {

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private bool _isInitialized;
        private readonly Vector3 _offset = new Vector3(0.25f, 0.1f, -0.25f);

        // const

        //reference
        private Transform _target;
        private GameObject _shadow;

        // --------------------- BASE METHODS ------------------

        public override void Awake() {
            if (_isInitialized) {
                return;
            }

            _target = gameObject.transform;
            _shadow = GameObject.Instantiate("dynamicShadow", _target);

            CalculateShadowSize();

            _isInitialized = true;
        }
        public override void Start() {
            if (gameObject.HasComponent<MovingRigidBody>()) {
                gameObject.GetComponent<MovingRigidBody>().SetSyncedObject(_shadow, _offset);
            }
            if (gameObject.HasComponent<AnimatedRigidBody>()) {
                gameObject.GetComponent<AnimatedRigidBody>().SetSyncedObject(_shadow, _offset);
            }
            if (gameObject.HasComponent<DynamicRigidBody>()) {
                gameObject.GetComponent<DynamicRigidBody>().SetSyncedObject(_shadow, _offset);
            }
        }

        public override void Destroy() {
            GameObject.Destroy(_shadow);
        }

        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other

        private void CalculateShadowSize() {
            Vector3 modelSize = BoundingBoxHelper.CalculateSize(gameObject.Model, transform.scale);
            Vector3 shadowSize = BoundingBoxHelper.CalculateSize(_shadow.Model, _shadow.transform.scale);

            float xFactor = shadowSize.X / modelSize.X;
            float zFactor = shadowSize.Z / modelSize.Z;

            Vector3 scale = new Vector3(3.0f / xFactor, 1.0f, 3.0f / zFactor);
            _shadow.transform.scale = scale;
        }

    }
}