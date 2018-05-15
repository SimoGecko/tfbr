// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Represents an animated rigid body in the physics simulation which is controlled by the gameplay only.
    /// </summary>
    class AnimatedRigidBody : RigidBodyComponent {

        #region Constructor

        /// <summary>
        /// Initialize an animated rigid body
        /// </summary>
        /// <param name="size">Size of the collider-shape adjusted uniquely on all axes</param>
        /// <param name="isActive">Active in physical simulation</param>
        /// <param name="shapeType">Type of the collider-shape</param>
        /// <param name="pureCollider">True if it's only for collision -> simulation only adjusted by static-objects</param>
        /// <param name="synchedObjects">The objects which it's location and orientation are syned</param>
        public AnimatedRigidBody(float size = 1.0f, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false, List<Follower> synchedObjects = null)
            : this(new Vector3(size), isActive, shapeType, pureCollider, synchedObjects) {
        }


        /// <summary>
        /// Initialize an animated rigid body
        /// </summary>
        /// <param name="size">Scaled size of the collider-shape</param>
        /// <param name="isActive">Active in physical simulation</param>
        /// <param name="shapeType">Type of the collider-shape</param>
        /// <param name="pureCollider">True if it's only for collision -> simulation only adjusted by static-objects</param>
        /// <param name="synchedObjects">The objects which it's location and orientation are syned</param>
        public AnimatedRigidBody(Vector3 size, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false, List<Follower> synchedObjects = null) {
            IsStatic = true;
            IsAnimated = true;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);
            SyncedObjects = synchedObjects ?? new List<Follower>();
        }

        #endregion

    }
}
