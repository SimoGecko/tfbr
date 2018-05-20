// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Represents a static rigid body in the physics simulation.
    /// </summary>
    class StaticRigidBody : RigidBodyComponent {

        #region Properties and attributes

        // Defines if the object is a ground => used to make the ground thicker to make it more stable
        private readonly bool _isGround;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a static rigid body
        /// </summary>
        /// <param name="size">Size of the collider-shape adjusted uniquely on all axes</param>
        /// <param name="isActive">Active in physical simulation</param>
        /// <param name="isGround">True  if the object is a part of the ground => used to make the physics simulation more stable.</param>
        /// <param name="shapeType">Type of the collider-shape</param>
        /// <param name="pureCollider">True if it's only for collision -> simulation only adjusted by static-objects</param>
        public StaticRigidBody(float size = 1.0f, bool isActive = true, bool isGround = false, ShapeType shapeType = ShapeType.Box, bool pureCollider = false)
            : this(new Vector3(size), isActive, isGround, shapeType, pureCollider) {
        }


        /// <summary>
        /// Initialize a moving static body
        /// </summary>
        /// <param name="size">Scaled size of the collider-shape</param>
        /// <param name="isActive">Active in physical simulation</param>
        /// <param name="isGround">True  if the object is a part of the ground => used to make the physics simulation more stable.</param>
        /// <param name="shapeType">Type of the collider-shape</param>
        /// <param name="pureCollider">True if it's only for collision -> simulation only adjusted by static-objects</param>
        public StaticRigidBody(Vector3 size, bool isActive = true, bool isGround = false, ShapeType shapeType = ShapeType.Box, bool pureCollider = false) {
            IsStatic = true;
            IsAnimated = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);

            _isGround = isGround;
        }

        #endregion

        #region Helper-functions


        /// <summary>
        /// Calculate the tightest bounding-shape with the given <paramref name="type"/>
        /// </summary>
        /// <param name="type">Type of the bounding-shape</param>
        protected override void CalculateShape(ShapeType type) {
            if (_isGround) {
                JVector  bbSize = new JVector(1000, 10, 1000);

                CenterOfMass = new JVector(0, -5, 0);
                CollisionShape = new BoxShape(bbSize);
            } else {
                base.CalculateShape(type);
            }
        }

        #endregion

    }
}
