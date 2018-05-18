// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    /// <summary>
    /// Handles the rigid body which can be controlled by the player
    /// </summary>
    class SteerableCollider : Collider {

        #region Properties and attributes

        /// <summary>
        /// Rotation around the Y-axis given as radians
        /// </summary>
        public float RotationY { get; set; }

        /// <summary>
        /// Current speed for the simulation
        /// </summary>
        public JVector Speed { get; set; }

        #endregion

        #region Constructor

        public SteerableCollider(Shape shape)
            : this(shape, new Jitter.Dynamics.Material()) {
        }

        public SteerableCollider(Shape shape, Jitter.Dynamics.Material material, bool isParticle = false)
            : base(shape, material, isParticle) {
        }

        #endregion

        #region Jitter-loop

        /// <summary>
        /// Functionality which is applied before the physics is updated
        /// </summary>
        /// <param name="timestep"></param>
        public override void PreStep(float timestep) {
            AddForce(Speed);
            LinearVelocity = Speed;
            //Position += Speed;

            Position = new JVector(Position.X, HeightHalf + 0.01f, Position.Z);
            Orientation = JMatrix.CreateRotationY(RotationY);

            base.PreStep(timestep);
        }


        /// <summary>
        /// Functionality which is applied after the physics is updated
        /// </summary>
        /// <param name="timestep"></param>
        public override void PostStep(float timestep) {
            //AddForce(Speed);
            //LinearVelocity = Speed;

            Position = new JVector(Position.X, HeightHalf + 0.1f, Position.Z);
            Orientation = JMatrix.CreateRotationY(RotationY);

            base.PostStep(timestep);
        }

        #endregion

        /// <summary>
        /// Correct the position of the rigid-body => adjust the y-axis
        /// </summary>
        public void CorrectPosition() {
            Position = new JVector(Position.X, HeightHalf + 0.01f, Position.Z);
        }
    }
}
