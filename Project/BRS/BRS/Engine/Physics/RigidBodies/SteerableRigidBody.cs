using BRS.Engine.Physics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Scripts.Physics {
    class SteerableRigidBody : Collider {
        public float RotationY { get; set; }
        public SteerableRigidBody(Shape shape) : base(shape) {
        }

        public SteerableRigidBody(Shape shape, Material material) : base(shape, material) {
        }

        public SteerableRigidBody(Shape shape, Material material, bool isParticle) : base(shape, material, isParticle) {
        }

        public override void PreStep(float timestep) {
            float height = BoundingBox.Max.Y - BoundingBox.Min.Y;

            if (Position.Y - height * .5f < 0.0f) {
                Position = new JVector(Position.X, height * .5f, Position.Z);
                LinearVelocity = new JVector(LinearVelocity.X, 0, LinearVelocity.Z);
            }
            //Debug.Log(Orientation, "SteerableBody - Pre:\n");

            base.PreStep(timestep);
        }

        public override void PostStep(float timestep) {
            float height = BoundingBox.Max.Y - BoundingBox.Min.Y;

            if (true || Position.Y - height * .5f < 0.0f) {
                Position = new JVector(Position.X, height * .5f, Position.Z);
                LinearVelocity = new JVector(LinearVelocity.X, 0, LinearVelocity.Z);
                Orientation = JMatrix.CreateRotationY(RotationY);
            }
            //Debug.Log(Orientation, "SteerableBody - Post:\n");

            base.PostStep(timestep);
        }
    }
}
