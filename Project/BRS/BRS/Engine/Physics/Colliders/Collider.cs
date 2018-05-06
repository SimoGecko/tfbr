// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics.Colliders {
    /// <summary>
    /// Represents the rigid body extension for the physics to have access to the game-object during simulation.
    /// </summary>
    public class Collider : RigidBody {

        #region Properties and attributes

        // Link to the simulated gameobject
        public GameObject GameObject { get; set; }
        // Link to the synced object (shadow)
        public List<Follower> SyncedObjects;


        public float Length => BoundingBoxSize.X;
        public float Height => BoundingBoxSize.Y;
        public float Width => BoundingBoxSize.Z;
        public float LengthHalf => BoundingBoxSizeHalf.X;
        public float HeightHalf => BoundingBoxSizeHalf.Y;
        public float WidthHalf => BoundingBoxSizeHalf.Z;
        public JVector BoundingBoxSize { get; }
        public JVector BoundingBoxSizeHalf { get; }

        /// <summary>
        /// Last rotation can be used to determine the angle of the last rotation-change in the curve
        /// </summary>
        public float LastRotation => _newRotation - _oldRotation;


        // Stores the old and new rotation to see in which direction the rigid-body makes the curve
        private float _oldRotation;
        private float _newRotation;


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
                _oldRotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(Orientation)).ToEuler().Y;
                _newRotation = GameObject.transform.eulerAngles.Y;

                // First calculate the correct orientation/rotation and then adjust the position with respect to the rotated COM
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(GameObject.transform.rotation));
                Position = Conversion.ToJitterVector(GameObject.transform.position) + JVector.Transform(CenterOfMass, Orientation);

            } else {
                _oldRotation = GameObject.transform.eulerAngles.Y;
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

                _newRotation = GameObject.transform.eulerAngles.Y;
            }


            if (SyncedObjects.Count > 0) {
                Vector3 projectedToGround = GameObject.transform.position;
                projectedToGround.Y = 0;
                float rotation = GameObject.transform.eulerAngles.Y;

                foreach (Follower follower in SyncedObjects) {
                    switch (follower.Type) {
                        case Follower.FollowingType.OnFloor:
                            float offset = MathHelper.Clamp(GameObject.transform.position.Y, 1, 10);
                            follower.GameObject.transform.rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(rotation));
                            follower.GameObject.transform.position = projectedToGround + offset * follower.Offset;

                            break;

                        case Follower.FollowingType.Orientated:
                            var tmp = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(rotation));
                            follower.GameObject.transform.rotation = tmp * follower.Orientation;
                            follower.GameObject.transform.position = projectedToGround + Vector3.Transform(follower.Offset, tmp);

                            break;
                    }
                }
            }

            base.PostStep(timestep);
        }

        #endregion

    }
}
