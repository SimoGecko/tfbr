using BRS.Engine.Physics;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Scripts.Physics {
    class DynamicRigidBody : RigidBodyComponent {
        public DynamicRigidBody(PhysicsManager physicsManager, bool isActive = true, Material material = null) {
            PhysicsManager = physicsManager;
            IsStatic = false;
            IsActive = isActive;
            Material = material;
        }

        /// <summary>
        /// Update of the time-step.
        /// </summary>
        public override void Update() {
            // Apply position and rotation from physics-world to the game-object
            transform.position = Conversion.ToXnaVector(RigidBody.Position - CenterOfMass);
            transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));

            base.Update();
        }
    }
}
