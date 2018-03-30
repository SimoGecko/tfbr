using BRS.Engine.Physics;
using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.Physics {
    abstract class RigidBodyComponent : Component {
        protected Shape CollisionShape;
        public RigidBody RigidBody;
        protected PhysicsManager PhysicsManager;
        protected BodyTag Tag;

        protected bool IsStatic;
        protected bool IsActive;

        protected Material Material;
        protected JVector CenterOfMass;

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Start() {
            Model model = gameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            bbSize = new JVector(bbSize.X * gameObject.transform.scale.X,
                bbSize.Y * gameObject.transform.scale.Y,
                bbSize.Z * gameObject.transform.scale.Z);
            CollisionShape = new BoxShape(bbSize);

            RigidBody = new Collider(CollisionShape) {
                Position = Conversion.ToJitterVector(transform.position),
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                IsStatic = IsStatic,
                IsActive = IsActive,
                Tag = Tag,
                GameObject = gameObject
            };

            if (Material != null) {
                RigidBody.Material = Material;
            }

            PhysicsManager.World.AddBody(RigidBody);

            base.Start();
        }
    }
}
