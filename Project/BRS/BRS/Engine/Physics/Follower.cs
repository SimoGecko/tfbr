using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics {
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

        public Follower(GameObject syncedObject) 
            : this(syncedObject, Vector3.Zero, Quaternion.Identity, FollowingType.Orientated) { }

        public Follower(GameObject gameObject, Vector3 offset, Quaternion orientation, FollowingType type) {
            GameObject = gameObject;
            Offset = offset;
            Orientation = orientation;
            Type = type;
        }
    }
}
