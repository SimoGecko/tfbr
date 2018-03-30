using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace BRS.Engine.Physics {
    public class Collider : RigidBody {
        public GameObject GameObject { get; set; }

        public Collider(Shape shape) : base(shape) {
        }

        public Collider(Shape shape, Material material) : base(shape, material) {
        }

        public Collider(Shape shape, Material material, bool isParticle) : base(shape, material, isParticle) {
        }
    }
}
