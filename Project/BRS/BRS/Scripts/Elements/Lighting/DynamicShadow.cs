// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BRS.Scripts.Elements.Lighting {
    /// <summary>
    /// Initialize the dynamic shadow which will be synchronised with the position of the game-object
    /// </summary>
    class DynamicShadow : FollowerComponent {

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private readonly Vector3 _offset = new Vector3(0.25f, 0.1f, -0.25f);

        // const

        //reference

        // --------------------- BASE METHODS ------------------

        protected override List<Follower> CreateFollower() {
            Transform target = gameObject.transform;
            GameObject shadow = GameObject.Instantiate("dynamicShadow", target);
            shadow.transform.scale = CalculateShadowSize(shadow);

            return new List<Follower> { new Follower(shadow, _offset, Quaternion.Identity, Follower.FollowingType.OnFloor) };
        }

        public override void Update() {
        }

        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other

        private Vector3 CalculateShadowSize(GameObject shadow) {
            Vector3 modelSize = BoundingBoxHelper.CalculateSize(gameObject.Model, transform.scale);
            Vector3 shadowSize = BoundingBoxHelper.CalculateSize(shadow.Model, shadow.transform.scale);

            float xFactor = shadowSize.X / modelSize.X;
            float zFactor = shadowSize.Z / modelSize.Z;

            return new Vector3(1.5f / xFactor, 1.0f, 1.5f / zFactor);
        }

    }
}