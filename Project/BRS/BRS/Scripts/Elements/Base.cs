// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Elements {
    class Base : LivingEntity {
        ////////// base in the game that has health and collects money //////////

        // --------------------- VARIABLES ---------------------

        //public
        public int TotalMoney { get; private set; }
        public Color BaseColor { get; private set; }

        //private
        private const float DeloadDistanceThreshold = 4f;
        private const float TimeBetweenUnloads = .05f;
        private const float MoneyPenalty = .5f; // percent
        private readonly int _baseIndex = 0;
        

        //reference


        // --------------------- BASE METHODS ------------------
        public Base(int baseIndex) {
            _baseIndex = baseIndex;
            BaseColor = Graphics.ColorIndex(baseIndex);
        }

        public override void Start() {
            base.Start();
            TotalMoney = 0;

            // Todo: This causes currently a strange loop. Players are need UI to be started already, but the UI which contains the Suggestions uses the player and bases to be startet first!
            UpdateUI();
        }

        public override void Update() {
            //UpdateUI();
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer) {
                Player p = c.GameObject.GetComponent<Player>();
                if (p.TeamIndex == _baseIndex) {
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
            BaseUI.Instance.UpdateBaseUI(_baseIndex, Health, StartingHealth, TotalMoney);
        }

        protected override void Die() {
            //TODO show gameover because base exploded
        }

        public override void TakeDamage(float damage) {
            //base.TakeDamage(damage);
            //UpdateUI();
        }

        public void NotifyRoundEnd() {
            foreach(var p in TeamPlayers()) {
                if (!PlayerInsideRange(gameObject)) {
                    //apply penalty (could happen twice)
                    TotalMoney -= (int)(TotalMoney * MoneyPenalty);
                }
            }
            //SHOW money penalty (BUSTED!)
            UpdateUI();
        }


        // queries
        bool PlayerInsideRange(GameObject p) {
            return (p.transform.position - transform.position).LengthSquared() <= DeloadDistanceThreshold* DeloadDistanceThreshold;
        }

        Player[] TeamPlayers() {
            List<Player> result = new List<Player>();
            GameObject[] players = GameObject.FindGameObjectsWithTag(ObjectTag.Player);
            foreach (var player in players) {
                Player p = player.GetComponent<Player>();
                if (p.TeamIndex == _baseIndex) result.Add(p);
            }
            return result.ToArray();
        }


        // other
        async void DeloadPlayerProgression(PlayerInventory pi) {
            while (pi.CarryingValue > 0 && PlayerInsideRange(pi.gameObject)) { 
                TotalMoney += pi.ValueOnTop;
                pi.DeloadOne();
                UpdateUI();
                await Time.WaitForSeconds(TimeBetweenUnloads);
            }
        }

    }

}