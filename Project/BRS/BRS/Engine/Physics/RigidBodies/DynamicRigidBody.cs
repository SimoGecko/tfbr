using Jitter.LinearMath;

namespace BRS.Engine.Physics.RigidBodies {
    class DynamicRigidBody : RigidBodyComponent {
        public DynamicRigidBody(PhysicsManager physicsManager = null, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false) {
            if (physicsManager == null) physicsManager = PhysicsManager.Instance;
            PhysicsManager = physicsManager;
            IsStatic = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
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
