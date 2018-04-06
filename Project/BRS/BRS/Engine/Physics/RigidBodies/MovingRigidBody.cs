using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.RigidBodies {
    class MovingRigidBody : RigidBodyComponent {
        private float _treshold = 0.01f;

        public MovingRigidBody(PhysicsManager physicsManager, bool isActive = true, ShapeType shapeType = ShapeType.Box) {
            PhysicsManager = physicsManager;
            IsStatic = false;
            IsActive = isActive;
            ShapeType = shapeType;
            Tag = BodyTag.DrawMe;
        }

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Start() {
            Model model = GameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            bbSize = new JVector(bbSize.X * GameObject.transform.scale.X,
                bbSize.Y * GameObject.transform.scale.Y,
                bbSize.Z * GameObject.transform.scale.Z);
            CollisionShape = new BoxShape(bbSize);

            JVector com = 0.5f * Conversion.ToJitterVector(bb.Max + bb.Min);
            com = new JVector(com.X * GameObject.transform.scale.X,
                com.Y * GameObject.transform.scale.Y,
                com.Z * GameObject.transform.scale.Z);
            CenterOfMass = new JVector(com.X > _treshold ? com.X : 0,
                com.Y > _treshold ? com.Y : 0,
                com.Z > _treshold ? com.Z : 0);


            RigidBody = new SteerableRigidBody(CollisionShape) {
                Position = Conversion.ToJitterVector(transform.position),
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                IsStatic = IsStatic,
                IsActive = IsActive,
                Tag = BodyTag.DrawMe,
                Mass = 20.0f,
                GameObject = GameObject
            };

            RigidBody.Material = new Material { KineticFriction = 0, Restitution = 0, StaticFriction = 0 };

            PhysicsManager.World.AddBody(RigidBody);
        }

        /// <summary>
        /// Update of the time-step.
        /// </summary>
        public override void Update() {
            // Apply position and rotation from physics-world to the game-object
            transform.position = Conversion.ToXnaVector(RigidBody.Position - CenterOfMass);
            transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(RigidBody.Orientation));
           
            //Debug.Log(RigidBody.Orientation, "MovingRigidBody:\n");

            base.Update();
        }
    }
}
