// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    /// <summary>
    /// Handles the rigid body which can be controlled by the player
    /// </summary>
    class SteerableCollider : Collider {
        /// <summary>
        /// Rotation around the Y-axis given as radians
        /// </summary>
        public float RotationY { get; set; }
        public JVector Speed { get; set; }

        public SteerableCollider(Shape shape) : base(shape) {
        }

        public SteerableCollider(Shape shape, Jitter.Dynamics.Material material) : base(shape, material) {
        }

        public SteerableCollider(Shape shape, Jitter.Dynamics.Material material, bool isParticle) : base(shape, material, isParticle) {
        }

        public override void PostStep(float timestep) {
            AddForce(Speed);
            LinearVelocity = Speed;

            Position = new JVector(Position.X, HeightHalf + 0.01f, Position.Z);
            Orientation = JMatrix.CreateRotationY(RotationY);

            base.PostStep(timestep);
        }

        public void CorrectPosition() {
            Position = new JVector(Position.X, HeightHalf + 0.01f, Position.Z);
        }
    }
}
