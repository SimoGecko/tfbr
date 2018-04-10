using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    class SteerableCollider : Collider {
        public float RotationY { get; set; }
        public JVector Speed { get; set; }
        private readonly float _height;

        public SteerableCollider(Shape shape) : base(shape) {
            _height = BoundingBox.Max.Y - BoundingBox.Min.Y;
        }

        public SteerableCollider(Shape shape, Material material) : base(shape, material) {
            _height = BoundingBox.Max.Y - BoundingBox.Min.Y;
        }

        public SteerableCollider(Shape shape, Material material, bool isParticle) : base(shape, material, isParticle) {
            _height = BoundingBox.Max.Y - BoundingBox.Min.Y;
        }


        public override void PostStep(float timestep) {
            Position = new JVector(Position.X, _height * .5f, Position.Z);
            //LinearVelocity = new JVector(LinearVelocity.X, 0, LinearVelocity.Z);
            Orientation = JMatrix.CreateRotationY(RotationY);

            //Debug.Log(LinearVelocity, "Before: ");

            AddForce(Speed);
            LinearVelocity = Speed;
            //Debug.Log(LinearVelocity);

            //LinearVelocity = JVector.Dot(new JVector(Speed.X, 0, Speed.Z), Orientation);

            base.PostStep(timestep);
        }
    }
}
