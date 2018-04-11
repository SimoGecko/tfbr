using BRS.Engine.Physics.Colliders;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    class StaticRigidBody : RigidBodyComponent {
        private readonly bool _isGround;

        public StaticRigidBody(bool isActive = true, bool isGround = false, ShapeType shapeType = ShapeType.Box, bool pureCollider = false, float size = 1.0f) {
            IsStatic = true;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = size;

            _isGround = isGround;
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

        protected override void CalculateShape(ShapeType type) {
            if (_isGround) {
                Model model = gameObject.Model;
                BoundingBox bb = BoundingBoxHelper.Calculate(model);
                JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
                bbSize = new JVector(bbSize.X * gameObject.transform.scale.X,
                    10,
                    bbSize.Z * gameObject.transform.scale.Z);

                CenterOfMass = new JVector(0, 5, 0);
                CollisionShape = new BoxShape(bbSize);
            } else {
                base.CalculateShape(type);
            }
        }
    }
}
