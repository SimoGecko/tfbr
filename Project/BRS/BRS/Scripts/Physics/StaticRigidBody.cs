using BRS.Engine.Physics;
using Jitter.Dynamics;

namespace BRS.Scripts.Physics {
    class StaticRigidBody : RigidBodyComponent {
     
        public StaticRigidBody(PhysicsManager physicsManager, bool isActive = true, Material material = null) {
            PhysicsManager = physicsManager;
            IsStatic = true;
            IsActive = isActive;
            Material = material;
        }
    }
}
