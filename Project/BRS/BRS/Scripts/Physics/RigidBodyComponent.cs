using System;
using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Load;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.Physics {
    abstract class RigidBodyComponent : Component {
        private Shape _collisionShape;
        public RigidBody RigidBody { get; private set; }
        protected PhysicsManager PhysicsManager;

        protected bool IsStatic;
        protected bool IsActive;

        protected Material Material;
        protected JVector CenterOfMass { get; private set; }

        private float _treshold = 0.01f;

        /// <summary>
        /// Initialization of the rigid-body
        /// </summary>
        public override void Start() {
            Model model = gameObject.Model;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            _collisionShape = new BoxShape(bbSize);

            // Calculate the center of mass but use a little threshold
            if (gameObject.Type == ObjectType.Player) {
                JVector com = 0.5f * Conversion.ToJitterVector(bb.Max + bb.Min);
                CenterOfMass = new JVector(com.X > _treshold ? com.X : 0,
                    com.Y > _treshold ? com.Y : 0,
                    com.Z > _treshold ? com.Z : 0);
            }

            RigidBody = new RigidBody(_collisionShape) {
                Position = Conversion.ToJitterVector(transform.position),
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(transform.rotation)),
                IsStatic = IsStatic,
                IsActive = IsActive,
                Tag = BodyTag.DrawMe
            };


            if (Material != null) {
                RigidBody.Material = Material;
            }

            PhysicsManager.World.AddBody(RigidBody);

            base.Start();
        }
    }
}
