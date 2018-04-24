// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    /// <summary>
    /// Represents a static rigid body in the physics simulation.
    /// </summary>
    class StaticRigidBody : RigidBodyComponent {
        private readonly bool _isGround;

        public StaticRigidBody(float size = 1.0f, bool isActive = true, bool isGround = false, ShapeType shapeType = ShapeType.Box, bool pureCollider = false)
            : this(new Vector3(size), isActive, isGround, shapeType, pureCollider) {
        }

        public StaticRigidBody(Vector3 size, bool isActive = true, bool isGround = false, ShapeType shapeType = ShapeType.Box, bool pureCollider = false) {
            IsStatic = true;
            IsAnimated = false;
            IsActive = isActive;
            ShapeType = shapeType;
            PureCollider = pureCollider;
            Tag = BodyTag.DrawMe;
            Size = Conversion.ToJitterVector(size);

            _isGround = isGround;
        }

        /// <summary>
        /// Update of the time-step.
        /// </summary>
        public override void Update() {
            // Apply position and rotation from physics-world to the game-object
            //transform.position = Conversion.ToXnaVector(RigidBody.Position - CenterOfMass);
            //transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));

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

                CenterOfMass = new JVector(0, -5, 0);
                CollisionShape = new BoxShape(bbSize);
            } else {
                base.CalculateShape(type);
            }
        }
    }
}
