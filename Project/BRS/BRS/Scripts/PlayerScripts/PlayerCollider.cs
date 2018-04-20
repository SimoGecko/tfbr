// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
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

        //private
        private Vector3 _startPos, _endPos;
        private float _collidedRefTime;
        private float _collidedStartTime;

        private float _currentAngle;
        private float _endAngle;
        private float _refAngle;

        //const
        private const float Duration = .2f;

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
        public void Begin(Vector3 endPosition, float endAngle) {
            Audio.Play("attack", transform.position);

            //Debug.Log(Time.CurrentTime);
            IsCollided = true;
            _collidedRefTime = 0;
            _startPos = transform.position;
            _endPos = endPosition;
            _collidedStartTime = Time.CurrentTime;

            _currentAngle = transform.rotation.Y;
            _endAngle = _currentAngle + 2.0f;

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

                _currentAngle = Utility.SmoothDampAngle(_currentAngle, _endAngle, ref _refAngle, Duration);
                Debug.Log(_currentAngle);

                // Apply new position to the rigid-body
                // Todo by Andy for Andy: can be surely written better :-)
                transform.position = new Vector3(newPosition.X, transform.position.Y, newPosition.Z);
                transform.eulerAngles = new Vector3(0, _currentAngle, 0);
                _rigidBody.RigidBody.Position = new JVector(newPosition.X, _rigidBody.RigidBody.Position.Y, newPosition.Z);
                _rigidBody.RigidBody.Orientation = JMatrix.CreateRotationY(_currentAngle);
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