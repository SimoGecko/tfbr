using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    class StaticRigidBody : RigidBodyComponent {
        private readonly bool _isGround;

        public StaticRigidBody(PhysicsManager physicsManager, bool isActive = true, bool isGround = false, ShapeType shapeType = ShapeType.Box) {
            PhysicsManager = physicsManager;
            IsStatic = true;
            IsActive = isActive;
            ShapeType = shapeType;
            Tag = BodyTag.DrawMe;

            _isGround = isGround;
        }

        // todo: refactor again, just testing
        public override void Start() {
            if (_isGround) {
                Model model = gameObject.Model;
                BoundingBox bb = BoundingBoxHelper.Calculate(model);
                JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
                bbSize = new JVector(bbSize.X * gameObject.transform.scale.X,
                    10,
                    bbSize.Z * gameObject.transform.scale.Z);
                CollisionShape = new BoxShape(bbSize);

                RigidBody = new Collider(CollisionShape) {
                    Position = Conversion.ToJitterVector(transform.position - new Vector3(0, 5, 0)),
                    Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                    IsStatic = IsStatic,
                    IsActive = IsActive,
                    Tag = Tag,
                    GameObject = gameObject
                };

                PhysicsManager.World.AddBody(RigidBody);
            } else {
                base.Start();
            }
        }

        /// <summary>
        /// Update of the time-step.
        /// </summary>
        public override void Update() {
            // Apply position and rotation from physics-world to the game-object
            //transform.position = Conversion.ToXnaVector(RigidBody.Position - CenterOfMass);
            transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));

            base.Update();
        }
    }
}
