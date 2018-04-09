// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.Elements {
    class Pickup : Component {
        ////////// generic class for object that can be picked up //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float DelayBeforePickup = .5f; // cannot pickup right after spawned
        //const float pickupDistThreshold = .5f;


        //private
        protected bool CanPickup;
        //public System.Action OnPickup;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            CanPickup = false;
            Invoke(DelayBeforePickup, () => CanPickup = true);
        }

        public override void Update() {
            
        }

        public override void OnCollisionEnter(JRigidBody c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer && CanPickup) {
                DoPickup(c.GameObject.GetComponent<Player>());
                //OnPickup?.Invoke();
            }
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