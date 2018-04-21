// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Curve = BRS.Engine.Utilities.Curve;

namespace BRS.Scripts.PlayerScripts {
    class PlayerCollider : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        public bool IsCollided { get; private set; }

        /// <summary>
        /// Current rotation given in degrees
        /// </summary>
        public float CurrentRotation { get; private set; }

        //private
        private Vector3 _startPos, _endPos;
        private float _collidedRefTime;
        private float _collidedStartTime;

        private float _startRotation;
        private float _endRotation;
        private float _refRotation;
        private float _smoothRotation;

        //const
        private const float Duration = 0.2f;

        //reference
        private MovingRigidBody _rigidBody;
        private Collider _otherCollider;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            IsCollided = false;
            _rigidBody = gameObject.GetComponent<MovingRigidBody>();
        }
        public override void Update() { }

        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Begin(Collider other, Vector3 endPosition, float endAngle) {
            if (IsCollided && _otherCollider == other) {
                return;
            }

            Audio.Play("attack", transform.position);

            //Debug.Log(Time.CurrentTime);
            IsCollided = true;
            _otherCollider = other;
            _collidedRefTime = 0;
            _startPos = transform.position;
            _endPos = endPosition;
            _collidedStartTime = Time.CurrentTime;
            _smoothRotation = 0.0f;

            _startRotation= MathHelper.ToDegrees(_rigidBody.SteerableCollider.RotationY);
            _endRotation = _startRotation + endAngle;
            CurrentRotation = _startRotation;

            _rigidBody.RigidBody.LinearVelocity = JVector.Zero;

            PhysicsDrawer.Instance.AddPointToDraw(_startPos);
            PhysicsDrawer.Instance.AddPointToDraw(_endPos);

            Invoke(Duration, End);
        }

        public void Coroutine() {
            if (_collidedRefTime <= 1) {
                _collidedRefTime += Time.DeltaTime / Duration;
                float t = Curve.EvaluateSqrt(_collidedRefTime);
                Vector3 newPosition = Vector3.LerpPrecise(_startPos, _endPos, t);


                _smoothRotation = Utility.SmoothDamp(_smoothRotation, 1.0f, ref _smoothRotation, Duration);

                //CurrentRotation = Utility.SmoothDampAngle(CurrentRotation, _endRotation, ref _refRotation, Duration);
                CurrentRotation = MathHelper.Lerp(_startRotation, _endRotation, t);
                Debug.Log(CurrentRotation);

                // Apply new position to the rigid-body
                _rigidBody.RigidBody.Position = new JVector(newPosition.X, _rigidBody.RigidBody.Position.Y, newPosition.Z);
                _rigidBody.SteerableCollider.RotationY = MathHelper.ToRadians(CurrentRotation);
                _rigidBody.RigidBody.Update();
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