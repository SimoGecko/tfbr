// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

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
        public MovingRigidBody(float size = 1.0f, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false)
            : this(new Vector3(size), isActive, shapeType, pureCollider) {
        }


        /// <summary>
        /// Initialize a moving rigid body
        /// </summary>
        /// <param name="size">Scaled size of the collider-shape</param>
        /// <param name="isActive">Active in physical simulation</param>
        /// <param name="shapeType">Type of the collider-shape</param>
        /// <param name="pureCollider">True if it's only for collision -> simulation only adjusted by static-objects</param>
        public MovingRigidBody(Vector3 size, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false) {
            IsStatic = false;
            IsAnimated = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);
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
                Material = new Jitter.Dynamics.Material { KineticFriction = 1.0f, Restitution = 1.0f, StaticFriction = 1.0f }
            };

            StartPosition = RigidBody.Position;
            StartOrientation = RigidBody.Orientation;

            PhysicsManager.Instance.World.AddBody(RigidBody);
        }

        /// <summary>
        /// Start of the object => correct the possition
        /// </summary>
        public override void Start() {
            SteerableCollider.CorrectPosition();
        }

        #endregion

    }
}
