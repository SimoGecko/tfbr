using BRS.Engine.Physics;
using BRS.Load;
using Jitter.Dynamics;

namespace BRS.Scripts.Physics {
    class StaticRigidBody : RigidBodyComponent {
     
        public StaticRigidBody(PhysicsManager physicsManager, bool isActive = true, Material material = null, BodyTag tag = BodyTag.DontDrawMe) {
            PhysicsManager = physicsManager;
            IsStatic = true;
            IsActive = isActive;
            Material = material;
            Tag = tag;
        }
    }
}
