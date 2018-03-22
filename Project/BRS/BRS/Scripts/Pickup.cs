// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Pickup : Component {
        ////////// generic class for object that can be picked up //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        const float delayBeforePickup = .5f; // cannot pickup right after spawned
        bool canPickup = false;
        public System.Action OnPickup;
        //string interactabletag = "player";
        //float pickupthreshold = .5f;

        //reference
        //Transform target;
        //protected GameObject[] players;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //players = GameObject.FindGameObjectsByType(ObjectType.Player);//.GetComponent<Player>();
            //if (players == null) Debug.LogError("player not found");
            //target = player.transform;
            canPickup = false;
            Invoke(delayBeforePickup, () => canPickup = true);
        }

        public override void Update() {
            //CheckPickup();
        }

        public override void OnCollisionEnter(Collider c) {
            Player player = c.gameObject.GetComponent<Player>();
            if (player != null && canPickup) {
                DoPickup(player);
                OnPickup?.Invoke();
            }
            //GameObject.Destroy(gameObject);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        /*
        void CheckPickup() {
            foreach(GameObject o in players) {
                float dist = Vector3.DistanceSquared(transform.position, o.Transform.position);
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