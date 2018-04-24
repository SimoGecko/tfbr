// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics.Colliders;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Component to connect the game-object with the physics-simulation.
    /// </summary>
    public abstract class RigidBodyComponent : Component {

        public RigidBody RigidBody;

        protected ShapeType ShapeType;
        protected Shape CollisionShape;
        protected BodyTag Tag;

        protected bool IsStatic;
        protected bool IsActive;
        protected bool IsAnimated;
        protected bool PureCollider;
        protected JVector Size = JVector.One;

        protected JVector CenterOfMass;

        private float _threshold = 0.0001f;

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
                Material = new Jitter.Dynamics.Material { KineticFriction = 10.0f, Restitution = 0.0f, StaticFriction = 10.0f },
                Mass = 20.0f
            };

            PhysicsManager.Instance.World.AddBody(RigidBody);

            if (ShapeType == ShapeType.BoxInvisible) {
                gameObject.Model = null;
            }

            base.Awake();
        }

        public override void Destroy() {
            Debug.Log("Remove world object for " + gameObject.name);
            PhysicsManager.Instance.World.RemoveBody(RigidBody);

            base.Destroy();
        }

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
    }
}
