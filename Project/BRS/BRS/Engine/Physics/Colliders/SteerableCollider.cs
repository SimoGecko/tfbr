﻿using Windows.Devices.Geolocation;
using BRS.Scripts.PlayerScripts;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    class SteerableCollider : Collider {
        public float RotationY { get; set; }
        public JVector Speed { get; set; }
        //public bool PositionUpdatedByCollision { get; set; }

        public SteerableCollider(Shape shape) : base(shape) {
        }

        public SteerableCollider(Shape shape, Material material) : base(shape, material) {
        }

        public SteerableCollider(Shape shape, Material material, bool isParticle) : base(shape, material, isParticle) {
        }


        public override void PostStep(float timestep) {
            //if (!PositionUpdatedByCollision) {
            AddForce(Speed);
            LinearVelocity = Speed;
            //} else {
            //AddForce(JVector.Zero);
            //LinearVelocity = JVector.Zero;
            //GameObject.GetComponent<PlayerMovement>().ResetSmoothMatnitude();
            //}
            Position = new JVector(Position.X, Height * .5f, Position.Z);
            Orientation = JMatrix.CreateRotationY(RotationY);

            //LinearVelocity = new JVector(LinearVelocity.X, 0, LinearVelocity.Z);
            //Debug.Log(LinearVelocity, "Before: ");

            //Debug.Log(LinearVelocity);

            //LinearVelocity = JVector.Dot(new JVector(Speed.X, 0, Speed.Z), Orientation);

            //PositionUpdatedByCollision = false;

            base.PostStep(timestep);
        }
    }
}
