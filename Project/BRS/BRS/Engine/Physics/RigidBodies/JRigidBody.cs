using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace BRS.Engine.Physics.RigidBodies {
    public class JRigidBody : RigidBody {
        public GameObject GameObject { get; set; }

        public JRigidBody(Shape shape) : base(shape) {
        }

        public JRigidBody(Shape shape, Material material) : base(shape, material) {
        }

        public JRigidBody(Shape shape, Material material, bool isParticle) : base(shape, material, isParticle) {
        }
    }
}
