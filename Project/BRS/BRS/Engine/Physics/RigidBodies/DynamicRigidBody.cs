// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Represents a dynamic rigid body in the physics simulation which is only controlled by the physics simulation.
    /// </summary>
    class DynamicRigidBody : RigidBodyComponent {
        public DynamicRigidBody(float size = 1.0f, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false)
            : this(new Vector3(size), isActive, shapeType, pureCollider) {
        }


        public DynamicRigidBody(Vector3 size, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false) {
            IsStatic = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);
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
