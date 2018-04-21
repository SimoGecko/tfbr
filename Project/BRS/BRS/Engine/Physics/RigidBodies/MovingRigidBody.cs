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

        public SteerableCollider SteerableCollider => RigidBody as SteerableCollider;

        public MovingRigidBody(float size = 1.0f, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false)
            : this(new Vector3(size), isActive, shapeType, pureCollider) {
        }

        public MovingRigidBody(Vector3 size, bool isActive = true, ShapeType shapeType = ShapeType.Box, bool pureCollider = false) {
            IsStatic = false;
            IsAnimated = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);
        }

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

            PhysicsManager.Instance.World.AddBody(RigidBody);
        }
    }
}
