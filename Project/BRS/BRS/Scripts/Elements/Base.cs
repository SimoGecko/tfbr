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


        // --------------------- BASE METHODS ------------------
        public Base(int baseIndex) {
            BaseIndex = baseIndex;
        }

        public override void Start() {
            base.Start();
            TotalMoney = 0;
            UpdateUI();
        }

        public override void Update() {
            //UpdateUI();
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.gameObject.tag == ObjectTag.Player;
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
            UpdateUI();
        }

        void UpdateUI() {
            BaseUI.instance.UpdateBaseUI(BaseIndex, health, startingHealth, TotalMoney);
        }

        protected override void Die() {
            //TODO show gameover because base exploded
        }

        public override void TakeDamage(float damage) {
            base.TakeDamage(damage);
            UpdateUI();
        }

        public void NotifyRoundEnd() {
            foreach(var p in TeamPlayers()) {
                if (!PlayerInsideRange(gameObject)) {
                    //apply penalty (could happen twice)
                    TotalMoney -= (int)(TotalMoney * moneyPenalty);
                }
            }
        }


        // queries
        bool PlayerInsideRange(GameObject p) {
            return (p.transform.position - transform.position).LengthSquared() <= deloadDistanceThreshold* deloadDistanceThreshold;
        }

        Player[] TeamPlayers() {
            List<Player> result = new List<Player>();
            GameObject[] players = GameObject.FindGameObjectsWithTag(ObjectTag.Player);
            foreach (var player in players) {
                Player p = player.GetComponent<Player>();
                if (p.TeamIndex == BaseIndex) result.Add(p);
            }
            return result.ToArray();
        }


        // other
        async void DeloadPlayerProgression(PlayerInventory pi) {
            while (pi.CarryingValue > 0 && PlayerInsideRange(pi.gameObject)) { 
                TotalMoney += pi.ValueOnTop;
                pi.DeloadOne();
                UpdateUI();
                await Time.WaitForSeconds(timeBetweenUnloads);
            }
        }

    }

}