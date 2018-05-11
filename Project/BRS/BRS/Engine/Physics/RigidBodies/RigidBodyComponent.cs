// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics.Colliders;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Component to connect the game-object with the physics-simulation.
    /// </summary>
    public abstract class RigidBodyComponent : Component {

        #region Properties and attributes

        /// <summary>
        /// Rigid body for the game-object which is used in the physics simulation
        /// </summary>
        public Collider RigidBody;

        /// <summary>
        /// Type of the bounding-shape
        /// </summary>
        protected ShapeType ShapeType;

        /// <summary>
        /// Bounding shape with type given in ShapeType
        /// </summary>
        protected Shape CollisionShape;

        /// <summary>
        /// Body-tag is used to mark someobjects for the physical-drawing
        /// </summary>
        protected BodyTag Tag;

        /// <summary>
        /// Defines if the rigid-body is static in the phycisc simulation
        /// </summary>
        protected bool IsStatic;

        /// <summary>
        /// Defines if the rigid-body is active in the phycisc simulation
        /// </summary>
        protected bool IsActive;

        /// <summary>
        /// Defines if the rigid-body is animated in the phycisc simulation.
        /// Effect: Animation in our code => only put position/rotation into physics simulation, not update the otherway
        /// </summary>
        protected bool IsAnimated;

        /// <summary>
        /// Defines if the rigid-body is a pure collider in the phycisc simulation
        /// </summary>
        protected bool PureCollider;

        /// <summary>
        /// Defines the scale of the bounding-shape
        /// </summary>
        protected JVector Size = JVector.One;

        /// <summary>
        /// Center of mass of the rigid body with respect to the bounding-shape
        /// </summary>
        protected JVector CenterOfMass;

        /// <summary>
        /// Stores the start position in the scene of the rigid body
        /// </summary>
        protected JVector StartPosition;

        /// <summary>
        /// Stores the start orientation/rotation in the scene of the rigid body
        /// </summary>
        protected JMatrix StartOrientation;

        /// <summary>
        /// Stores the list of synced objects which need the same spacial updates as this rigid-body
        /// </summary>
        protected List<Follower> SyncedObjects = new List<Follower>();

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Awake() {
            CalculateShape(ShapeType);

            RigidBody = new Collider(CollisionShape) {
                Position = Conversion.ToJitterVector(transform.position) + CenterOfMass,
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                CenterOfMass = CenterOfMass,
                IsStatic = IsStatic,
                IsActive = IsActive,
                IsAnimated = IsAnimated,
                Tag = Tag,
                PureCollider = PureCollider,
                GameObject = gameObject,
                SyncedObjects = new List<Follower>(),
                Material = new Jitter.Dynamics.Material { KineticFriction = 10.0f, Restitution = 0.0f, StaticFriction = 10.0f },
                Mass = 20.0f
            };

            StartPosition = RigidBody.Position;
            StartOrientation = RigidBody.Orientation;

            PhysicsManager.Instance.World.AddBody(RigidBody);

            if (ShapeType == ShapeType.BoxInvisible) {
                gameObject.Model = null;
            }

            base.Awake();
        }

        public override void Reset() {
            RigidBody.Position = StartPosition;
            RigidBody.Orientation = StartOrientation;
            gameObject.transform.position = Conversion.ToXnaVector(StartPosition - JVector.Transform(CenterOfMass, StartOrientation));
            gameObject.transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(StartOrientation));

            base.Reset();
        }


        /// <summary>
        /// Destroing the game-boject/component means to remove the rigid-body from the physics simulation
        /// </summary>
        public override void Destroy() {
            Debug.Log("Remove world object for " + gameObject.name);
            PhysicsManager.Instance.World.RemoveBody(RigidBody);

            base.Destroy();
        }

        #endregion

        /// <summary>
        /// Calculate the tightest bounding-shape with the given <paramref name="type"/>
        /// </summary>
        /// <param name="type">Type of the bounding-shape</param>
        protected virtual void CalculateShape(ShapeType type) {
            Model model = gameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            bbSize = new JVector(
                bbSize.X * Size.X * gameObject.transform.scale.X,
                bbSize.Y * Size.Y * gameObject.transform.scale.Y,
                bbSize.Z * Size.Z * gameObject.transform.scale.Z
            );

            JVector com = 0.5f * Conversion.ToJitterVector(bb.Max + bb.Min);
            CenterOfMass = new JVector(com.X * gameObject.transform.scale.X,
                com.Y * gameObject.transform.scale.Y,
                com.Z * gameObject.transform.scale.Z);

            float maxDimension = MathHelper.Max(bbSize.X, MathHelper.Max(bbSize.Y, bbSize.Z));

            switch (type) {
                case ShapeType.Sphere:
                    CollisionShape = new SphereShape(maxDimension);
                    break;

                case ShapeType.BoxUniform:
                    CollisionShape = new BoxShape(maxDimension, maxDimension, maxDimension);
                    break;

                case ShapeType.Box:
                case ShapeType.BoxInvisible:
                    CollisionShape = new BoxShape(bbSize);
                    break;
            }
        }

        public void AddSyncedObject(Follower follower) {
            SyncedObjects.Add(follower);
            RigidBody?.SyncedObjects.Add(follower);
        }
    }
}
