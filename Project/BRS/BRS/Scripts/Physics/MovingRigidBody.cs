using BRS.Engine.Physics;
using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.Physics {
    class MovingRigidBody : RigidBodyComponent {
        private float _treshold = 0.01f;

        public MovingRigidBody(PhysicsManager physicsManager, bool isActive = true, Material material = null) {
            PhysicsManager = physicsManager;
            IsStatic = false;
            IsActive = isActive;
            Material = material;
            Tag = BodyTag.DrawMe;
        }

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Start() {
            Model model = gameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            CollisionShape = new BoxShape(bbSize);

            JVector com = 0.5f * Conversion.ToJitterVector(bb.Max + bb.Min);
            CenterOfMass = new JVector(com.X > _treshold ? com.X : 0,
                com.Y > _treshold ? com.Y : 0,
                com.Z > _treshold ? com.Z : 0);

            Material = new Material {KineticFriction = 0, Restitution = 0, StaticFriction = 0};

            RigidBody = new RigidBody(CollisionShape) {
                Position = Conversion.ToJitterVector(transform.position),
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                IsStatic = IsStatic,
                IsActive = IsActive,
                Tag = BodyTag.DrawMe,
                Mass = 20.0f
            };

            if (Material != null) {
                RigidBody.Material = Material;
            }

            PhysicsManager.World.AddBody(RigidBody);
        }

        /// <summary>
        /// Update of the time-step.
        /// </summary>
        public override void Update() {
            // Apply position and rotation from physics-world to the game-object
            transform.position = Conversion.ToXnaVector(RigidBody.Position - CenterOfMass);
            transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));

            base.Update();
        }
    }
}
