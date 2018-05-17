// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics.Colliders;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Represents a steerable rigid body in the physics simulation which is controlled by the physics simulation and the player.
    /// </summary>
    class MovingRigidBody : RigidBodyComponent {

        #region Properties and attributes

        /// <summary>
        /// Returns the steerable -collider which is used in the physics-simulation
        /// </summary>
        public SteerableCollider SteerableCollider => RigidBody as SteerableCollider;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a moving rigid body
        /// </summary>
        /// <param name="size">Size of the collider-shape adjusted uniquely on all axes</param>
        /// <param name="isActive">Active in physical simulation</param>
        /// <param name="shapeType">Type of the collider-shape</param>
        /// <param name="pureCollider">True if it's only for collision -> simulation only adjusted by static-objects</param>
        /// <param name="synchedObjects">The objects which it's location and orientation are syned</param>
        public MovingRigidBody(float size = 1.0f, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false, List<Follower> synchedObjects = null)
            : this(new Vector3(size), isActive, shapeType, pureCollider, synchedObjects) {
        }


        /// <summary>
        /// Initialize a moving rigid body
        /// </summary>
        /// <param name="size">Scaled size of the collider-shape</param>
        /// <param name="isActive">Active in physical simulation</param>
        /// <param name="shapeType">Type of the collider-shape</param>
        /// <param name="pureCollider">True if it's only for collision -> simulation only adjusted by static-objects</param>
        /// <param name="synchedObjects">The objects which it's location and orientation are syned</param>
        public MovingRigidBody(Vector3 size, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false, List<Follower> synchedObjects = null) {
            IsStatic = false;
            IsAnimated = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);
            SyncedObjects = synchedObjects ?? new List<Follower>();
        }

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Awake() {
            CalculateShape(ShapeType.Box);

            RigidBody = new SteerableCollider(CollisionShape) {
                Position = Conversion.ToJitterVector(transform.position),
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                CenterOfMass = CenterOfMass,
                IsStatic = IsStatic,
                IsActive = IsActive,
                Tag = BodyTag.DrawMe,
                Mass = 20.0f,
                GameObject = gameObject,
                SyncedObjects = SyncedObjects,
                Material = new Jitter.Dynamics.Material { KineticFriction = 1.0f, Restitution = 1.0f, StaticFriction = 1.0f }
            };

            StartOrientation = RigidBody.Orientation;
            StartPosition = RigidBody.Position;

            PhysicsManager.Instance.World.AddBody(RigidBody);
        }

        /// <summary>
        /// Start of the object => correct the position
        /// </summary>
        public override void Start() {
            SteerableCollider.CorrectPosition();
        }

        /// <summary>
        /// Reset the rigid-body as well the gameobject to the start position
        /// </summary>
        public override void Reset() {
            RigidBody.Position = StartPosition;
            RigidBody.Orientation = StartOrientation;
            gameObject.transform.position = Conversion.ToXnaVector(StartPosition);
            gameObject.transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(StartOrientation));
        }

        #endregion

    }
}
