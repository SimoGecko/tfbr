// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Represents an animated rigid body in the physics simulation which is controlled by the gameplay only.
    /// </summary>
    class AnimatedRigidBody : RigidBodyComponent {
        public AnimatedRigidBody(float size = 1.0f, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false)
            : this(new Vector3(size), isActive, shapeType, pureCollider) {
        }


        public AnimatedRigidBody(Vector3 size, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false) {
            IsStatic = true;
            IsAnimated = true;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);
        }
    }
}
