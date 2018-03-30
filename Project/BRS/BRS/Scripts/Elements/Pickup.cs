// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Pickup : Component {
        ////////// generic class for object that can be picked up //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float delayBeforePickup = .5f; // cannot pickup right after spawned
        //const float pickupDistThreshold = .5f;


        //private
        protected bool canPickup;
        public System.Action OnPickup;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            canPickup = false;
            Invoke(delayBeforePickup, () => canPickup = true);
        }

        public override void Update() {
            
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer && canPickup) {
                DoPickup(c.GameObject.GetComponent<Player>());
                OnPickup?.Invoke();
            }
            //GameObject.Destroy(gameObject);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        /*
        void CheckPickup() {
            foreach(GameObject o in players) {
                float dist = Vector3.DistanceSquared(transform.position, o.transform.position);
                if (dist < pickupthreshold) {
                    OnPickup(o);
                }
            }
            
        }*/

        protected virtual void DoPickup(Player p) {
            //fill
        }


        // queries



        // other

    }

}