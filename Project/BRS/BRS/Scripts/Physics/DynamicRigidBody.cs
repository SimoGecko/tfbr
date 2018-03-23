using BRS.Engine.Physics;
using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.Physics {
    class DynamicRigidBody : Component {
        private Shape _collisionShape;
        public RigidBody RigidBody { get; private set; }
        private readonly PhysicsManager _physicsManager;

        private readonly bool _isStatic;
        private readonly bool _isActive;

        private readonly Material _material;

        public DynamicRigidBody(PhysicsManager physicsManager, bool isActive = true, Material material = null) {
            _physicsManager = physicsManager;
            _isStatic = false;
            _isActive = isActive;
            _material = material;
        }

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Start() {
            Model model = gameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            _collisionShape = new BoxShape(bbSize);
            //_collisionShape = ConvexHullHelper.Calculate(model); //Shape sh = new ConvexHullShape(model.Meshes);

            RigidBody = new RigidBody(_collisionShape)
            {
                Position = Conversion.ToJitterVector(transform.position),
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                IsStatic = _isStatic,
                IsActive = _isActive,
                Tag = gameObject.Type == ObjectType.Player ? BodyTag.DrawMe : BodyTag.DontDrawMe
            };
            //RigidBody.Tag = BodyTag.DontDrawMe;

            if (_material != null) {
                RigidBody.Material = _material;
            }

            _physicsManager.World.AddBody(RigidBody);

            base.Start();
        }

        /// <summary>
        /// Update of the time-step.
        /// </summary>
        public override void Update() {
            // Apply position and rotation from physics-world to the game-object
            transform.position = Conversion.ToXnaVector(RigidBody.Position);
            transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));

            base.Update();
        }
    }
}
