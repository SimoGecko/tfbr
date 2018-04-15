// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    /// <summary>
    /// Represents the rigid body extension for the physics to have access to the game-object during simulation.
    /// </summary>
    public class Collider : RigidBody {
        public GameObject GameObject { get; set; }

        public float Length => BoundingBoxSize.X;
        public float Height => BoundingBoxSize.Y;
        public float Width => BoundingBoxSize.Z;
        public JVector BoundingBoxSize { get; }

        public Collider(Shape shape) : base(shape) {
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
        }

        public Collider(Shape shape, Material material) : base(shape, material) {
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
        }

        public Collider(Shape shape, Material material, bool isParticle) : base(shape, material, isParticle) {
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
        }
    }
}
