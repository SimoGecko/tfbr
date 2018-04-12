using System.Numerics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
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
