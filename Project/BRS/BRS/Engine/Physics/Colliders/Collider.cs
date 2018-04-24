﻿// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    /// <summary>
    /// Represents the rigid body extension for the physics to have access to the game-object during simulation.
    /// </summary>
    public class Collider : RigidBody {

        #region Properties and attributes

        // Link to the simulated gameobject
        public GameObject GameObject { get; set; }

        public float Length => BoundingBoxSize.X;
        public float Height => BoundingBoxSize.Y;
        public float Width => BoundingBoxSize.Z;
        public float LengthHalf => BoundingBoxSizeHalf.X;
        public float HeightHalf => BoundingBoxSizeHalf.Y;
        public float WidthHalf => BoundingBoxSizeHalf.Z;
        public JVector BoundingBoxSize { get; }
        public JVector BoundingBoxSizeHalf { get; }


        /// <summary>
        /// If the object is animated, the position and rotation is only taken from the gameobject's transform-object.
        /// It's put into the physics-world but it is not animated in anyway => transform-object is never overwritten by physics.
        /// </summary>
        public bool IsAnimated { get; set; }

        #endregion

        #region Constructor

        public Collider(Shape shape)
            : this(shape, new Jitter.Dynamics.Material()) {
        }


        public Collider(Shape shape, Jitter.Dynamics.Material material, bool isParticle = false)
            : base(shape, material, isParticle) {
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
            BoundingBoxSizeHalf = 0.5f * BoundingBoxSize;
        }

        #endregion

        #region Jitter-loop

        /// <summary>
        /// Functionality which is applied before the physics is updated
        /// </summary>
        /// <param name="timestep"></param>
        public override void PreStep(float timestep) {
            if (IsAnimated) {
                // First calculate the correct orientation/rotation and then adjust the position with respect to the rotated COM
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(GameObject.transform.rotation));
                Position = Conversion.ToJitterVector(GameObject.transform.position) + JVector.Transform(CenterOfMass, Orientation);
            }
        }


        /// <summary>
        /// Functionality which is applied after the physics is updated
        /// </summary>
        /// <param name="timestep"></param>
        public override void PostStep(float timestep) {
            if (!IsStatic && !IsAnimated) {
                GameObject.transform.position = Conversion.ToXnaVector(Position - JVector.Transform(CenterOfMass, Orientation));
                GameObject.transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(Orientation));
            }

            base.PostStep(timestep);
        }

        #endregion

    }
}
