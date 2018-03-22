// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Base : LivingEntity {
        ////////// base in the game that has health and collects money //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float deloadDistanceThreshold = 4f;
        const float timeBetweenUnloads = .1f;
        const float moneyPenalty = .5f; // percent

        //private
        public int BaseIndex { get; set; } = 0;
        public int TotalMoney { get; private set; }



        //reference
        //Player player; // should be array
        //PlayerInventory playerInventory;


        // --------------------- BASE METHODS ------------------
        public Base(int baseIndex) {
            BaseIndex = baseIndex;
        }

        public override void Start() {
            base.Start();
            TotalMoney = 0;

            //player = GameObject.FindGameObjectWithName("player_" + BaseIndex).GetComponent<Player>();
            //if (player == null) Debug.LogError("player not found");
        }

        public override void Update() {
            /*if(Vector3.DistanceSquared(transform.position, playerInventory.transform.position) < deloadDistanceThreshold) {
                DeloadPlayer();
            }*/
            UpdateUI();
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.gameObject.Type == ObjectType.Player;

            if (isPlayer) {
                Player p = c.gameObject.GetComponent<Player>();
                if (p.TeamIndex == BaseIndex) {
                    //DeloadPlayer(p.gameObject.GetComponent<PlayerInventory>());
                    DeloadPlayerProgression(p.gameObject.GetComponent<PlayerInventory>());
                }
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void DeloadPlayer(PlayerInventory pi) {
            TotalMoney += pi.CarryingValue;
            pi.DeloadAll();
            //UpdateUI();
        }

        void UpdateUI() {
            UserInterface.instance.UpdateBaseUI(BaseIndex, health, startingHealth, TotalMoney);
        }

        protected override void Die() {
            
        }

        public void NotifyRoundEnd() {
            GameObject[] players = GameObject.FindGameObjectsByType(ObjectType.Player);
            foreach(var player in players) {
                Player p = player.GetComponent<Player>();
                if (p.TeamIndex == BaseIndex && !PlayerInsideRange(gameObject)) {
                    //apply penalty (could happen twice)
                    TotalMoney -= (int)(TotalMoney * moneyPenalty);
                }
            }
            
        }


        // queries
        bool PlayerInsideRange(GameObject p) {
            return (p.transform.position - transform.position).LengthSquared() <= deloadDistanceThreshold* deloadDistanceThreshold;
        }


        // other
        async void DeloadPlayerProgression(PlayerInventory pi) {
            while (pi.CarryingValue > 0 && PlayerInsideRange(pi.gameObject)) { 
                TotalMoney += pi.ValueOnTop;
                pi.DeloadOne();
                //UpdateUI();
                await Time.WaitForSeconds(timeBetweenUnloads);
            }
        }

    }

}