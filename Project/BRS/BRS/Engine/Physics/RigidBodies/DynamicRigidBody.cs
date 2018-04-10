using Jitter.LinearMath;

namespace BRS.Engine.Physics.RigidBodies {
    class DynamicRigidBody : RigidBodyComponent {
        public DynamicRigidBody(bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false, float size = 1.0f) {
            IsStatic = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = size;
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
