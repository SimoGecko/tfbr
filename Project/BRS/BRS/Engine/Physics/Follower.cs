// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics {
    /// <summary>
    /// Class to synchronize the position and orientation of one object with another.
    /// </summary>
    public class Follower {

        public enum FollowingType { OnFloor, Orientated }

        #region Properties and attributes

        /// <summary>
        /// Gameobject which is the follower-object to render
        /// </summary>
        public readonly GameObject GameObject;

        /// <summary>
        /// Offset of the synced object to the COM
        /// </summary>
        public readonly Vector3 Offset;

        /// <summary>
        /// Local orientation of the synced object
        /// </summary>
        public Quaternion Orientation;

        /// <summary>
        /// Follower-type: Projecting as shadow to ground or relative with correct orientation
        /// </summary>
        public readonly FollowingType Type;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new follower
        /// </summary>
        /// <remarks>Object A => follower, Object B => the object to follow</remarks>
        /// <param name="gameObject">The game-object for A.</param>
        /// <param name="offset">Offset to the COM of B</param>
        /// <param name="orientation">Local orientation of A</param>
        /// <param name="type">Following type of A</param>
        public Follower(GameObject gameObject, Vector3 offset, Quaternion orientation, FollowingType type) {
            GameObject = gameObject;
            Offset = offset;
            Orientation = orientation;
            Type = type;
        }

        #endregion
    }
}
