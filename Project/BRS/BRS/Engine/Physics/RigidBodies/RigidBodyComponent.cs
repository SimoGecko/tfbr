using BRS.Engine.Physics.Colliders;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    public abstract class RigidBodyComponent : Component {
        public RigidBody RigidBody;

        protected ShapeType ShapeType;
        protected Shape CollisionShape;
        protected BodyTag Tag;

        protected bool IsStatic;
        protected bool IsActive;
        protected bool PureCollider;
        protected float Size = 1.0f;

        protected JVector CenterOfMass;

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Start() {
            CalculateShape(ShapeType);

            RigidBody = new Collider(CollisionShape) {
                Position = Conversion.ToJitterVector(transform.position) - CenterOfMass,
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                CenterOfMass =  CenterOfMass,
                IsStatic = IsStatic,
                IsActive = IsActive,
                Tag = Tag,
                PureCollider = PureCollider,
                GameObject = gameObject,
                Material = new Material { Restitution = 0.0f },
                Mass = 20.0f
            };

            PhysicsManager.Instance.World.AddBody(RigidBody);

            base.Start();
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
                bbSize.X * Size * gameObject.transform.scale.X,
                bbSize.Y * Size * gameObject.transform.scale.Y,
                bbSize.Z * Size * gameObject.transform.scale.Z
                );

            float maxDimension = MathHelper.Max(bbSize.X, MathHelper.Max(bbSize.Y, bbSize.Z));

            switch (type) {
                case ShapeType.Sphere:
                    CollisionShape = new SphereShape(maxDimension);
                    break;

                case ShapeType.BoxUniform:
                    CollisionShape = new BoxShape(maxDimension, maxDimension, maxDimension);
                    break;

                case ShapeType.Box:
                    CollisionShape = new BoxShape(bbSize);
                    break;
            }
        }
    }
}
