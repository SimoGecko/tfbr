// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Curve = BRS.Engine.Utilities.Curve;

namespace BRS.Scripts.PlayerScripts {
    class PlayerCollider : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        public bool IsCollided { get; private set; }

        //private
        private Vector3 _collidedStartPos, _collidedEndPos;
        private float _collidedRefTime;
        private float _collidedStartTime;

        //const
        private const float AttackDuration = .2f;

        //reference
        private MovingRigidBody _rigidBody;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            IsCollided = false;
            _rigidBody = gameObject.GetComponent<MovingRigidBody>();
        }
        public override void Update() { }

        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Begin(Vector3 endPosition) {
            Audio.Play("attack", transform.position);

            //Debug.Log(Time.CurrentTime);
            IsCollided = true;
            _collidedRefTime = 0;
            _collidedStartPos = transform.position;
            _collidedEndPos = endPosition;
            _collidedStartTime = Time.CurrentTime;

            _rigidBody.RigidBody.LinearVelocity = JVector.Zero;

            PhysicsDrawer.Instance.AddPointToDraw(_collidedStartPos);
            PhysicsDrawer.Instance.AddPointToDraw(_collidedEndPos);

            Invoke(AttackDuration, End);
        }

        public void Coroutine() {
            if (_collidedRefTime <= 1) {
                _collidedRefTime += Time.DeltaTime / AttackDuration;
                float t = Curve.EvaluateSqrt(_collidedRefTime);
                Vector3 newPosition = Vector3.LerpPrecise(_collidedStartPos, _collidedEndPos, t);
                //Gizmos.DrawWireSphere(_attackStartPos, 1.0f);
                
                // Apply new position to the rigid-body
                // Todo by Andy for Andy: can be surely written better :-)
                transform.position = new Vector3(newPosition.X, transform.position.Y, newPosition.Z);
                _rigidBody.RigidBody.Position = new JVector(newPosition.X, _rigidBody.RigidBody.Position.Y, newPosition.Z);
            } else {
                IsCollided = false;
            }
        }

        void End() {
            IsCollided = false;
        }


        // queries


        // other

    }
}