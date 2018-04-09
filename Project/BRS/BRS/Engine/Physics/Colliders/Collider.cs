using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.Colliders {
    public abstract class Collider : Component {
        public RigidBody RigidBody;

        protected ShapeType ShapeType;
        protected Shape CollisionShape;
        protected PhysicsManager PhysicsManager;
        protected BodyTag Tag;

        protected bool IsStatic;
        protected bool IsActive;
        protected bool PureCollider;

        protected JVector CenterOfMass;

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Start() {
            CalculateShape(ShapeType);

            RigidBody = new JRigidBody(CollisionShape) {
                Position = Conversion.ToJitterVector(transform.position),
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                IsStatic = IsStatic,
                IsActive = IsActive,
                Tag = Tag,
                PureCollider = PureCollider,
                GameObject = GameObject,
                Material = new Material { Restitution = 0.0f }
            };

            PhysicsManager.World.AddBody(RigidBody);

            base.Start();
        }

        public override void Destroy() {
            Debug.Log("Remove world object for " + GameObject.name);
            PhysicsManager.World.RemoveBody(RigidBody);

            base.Destroy();
        }

        private void CalculateShape(ShapeType type) {
            Model model = GameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            bbSize = new JVector(bbSize.X * GameObject.transform.scale.X,
                bbSize.Y * GameObject.transform.scale.Y,
                bbSize.Z * GameObject.transform.scale.Z);

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
