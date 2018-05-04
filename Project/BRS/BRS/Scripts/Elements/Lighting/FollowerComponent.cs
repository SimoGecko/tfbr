// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BRS.Scripts.Elements.Lighting {
    /// <summary>
    /// Initialize the dynamic shadow which will be synchronised with the position of the game-object
    /// </summary>
    abstract class FollowerComponent : Component {

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private bool _isInitialized;

        // const

        //reference
        protected List<Follower> Followers;


        // --------------------- BASE METHODS ------------------

        public override void Awake() {
            if (_isInitialized) {
                return;
            }

            Followers = CreateFollower();

            _isInitialized = true;
        }

        public override void Start() {
            for (int i = 0; i < Followers.Count; ++i) {
                Follower follower = Followers[i];
                if (gameObject.HasComponent<MovingRigidBody>()) {
                    gameObject.GetComponent<MovingRigidBody>().AddSyncedObject(follower);
                }
                if (gameObject.HasComponent<AnimatedRigidBody>()) {
                    gameObject.GetComponent<AnimatedRigidBody>().AddSyncedObject(follower);
                }
                if (gameObject.HasComponent<DynamicRigidBody>()) {
                    gameObject.GetComponent<DynamicRigidBody>().AddSyncedObject(follower);
                }
            }
        }

        public abstract override void Update();

        public override void Destroy() {
            for (int i = 0; i < Followers.Count; ++i) {
                GameObject.Destroy(Followers[i].GameObject);
            }

            Followers.Clear();
        }

        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other
        protected abstract List<Follower> CreateFollower();
        

    }
}