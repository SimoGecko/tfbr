// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.Elements {
    class Pickup : Component {
        ////////// generic class for object that can be picked up //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float DelayBeforePickup = .5f; // cannot pickup right after spawned


        //private
        protected bool CanPickup;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            CanPickup = false;
            Invoke(DelayBeforePickup, () => CanPickup = true);
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer && CanPickup) {
                DoPickup(c.GameObject.GetComponent<Player>());
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public virtual void DoPickup(Player p) {
            //fill
        }


        // queries



        // other

    }

}