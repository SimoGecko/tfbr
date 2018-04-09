using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    class DynamicCollider : Collider {
        private readonly bool _updateRotation;

        public DynamicCollider(PhysicsManager physicsManager, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false, bool updateRotation = true) {
            PhysicsManager = physicsManager;
            IsStatic = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            _updateRotation = updateRotation;
        }

        /// <summary>
        /// Update of the time-step.
        /// </summary>
        public override void Update() {
            // Apply position and rotation from physics-world to the game-object
            transform.position = Conversion.ToXnaVector(RigidBody.Position - CenterOfMass);
            //transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));

            base.Update();
        }
    }
}
