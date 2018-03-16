using BRS.Engine.Physics;
using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.Physics {
    class RigidBodyComponent : Component {
        private Shape _collisionShape;
        public RigidBody RigidBody { get; private set; }
        private readonly PhysicsManager _physicsManager;

        private readonly bool _isStatic;

        private Material _material;

        public RigidBodyComponent(PhysicsManager physicsManager, bool isStatic, Material material = null) {
            _physicsManager = physicsManager;
            _isStatic = isStatic;
            _material = material;
        }

        public override void Start() {
            Model model = gameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            _collisionShape = new BoxShape(bbSize);

            RigidBody = new RigidBody(_collisionShape);
            RigidBody.Position = Conversion.ToJitterVector(transform.position);
            RigidBody.Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation));
            RigidBody.IsStatic = _isStatic;
            RigidBody.Tag = BodyTag.DontDrawMe;

            if (_material != null) {
                RigidBody.Material = _material;
            }

            _physicsManager.World.AddBody(RigidBody);

            base.Start();
        }

        public override void Update() {
            transform.position = Conversion.ToXnaVector(RigidBody.Position);
            transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));

            //// Apply forces/changes to physics
            //gameObject.Transform.position = new JVector(transform.position.X, 0.5f, transform.position.Z);
            //gameObject.Transform.orientation = JMatrix.CreateRotationY(rotation * MathHelper.Pi / 180.0f);

            base.Update();
        }
    }
}
