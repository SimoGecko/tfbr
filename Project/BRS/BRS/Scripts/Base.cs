// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Base : LivingEntity {
        ////////// base in the game that has health and collects money //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float deloadDistanceThreshold = 10f;
        public int BaseIndex { get; set; } = 0;

        //private
        public int TotalMoney { get; private set; }



        //reference
        Player player;
        PlayerInventory playerInventory;


        // --------------------- BASE METHODS ------------------
        public Base(int baseIndex) {
            BaseIndex = baseIndex;
        }

        public override void Start() {
            base.Start();
            TotalMoney = 0;
            player = GameObject.FindGameObjectWithName("player_" + BaseIndex).GetComponent<Player>();
            if (player == null) Debug.LogError("player not found");
        }

        public override void Update() {
            /*if(Vector3.DistanceSquared(transform.position, playerInventory.transform.position) < deloadDistanceThreshold) {
                DeloadPlayer();
            }*/
        }

        public override void OnCollisionEnter(Collider c) {
            Player player = c.gameObject.GetComponent<Player>();
            if(player != null && player.teamIndex == BaseIndex) {
                //DeloadPlayer(player.gameObject.GetComponent<PlayerInventory>());
                DeloadPlayerProgression(player.gameObject.GetComponent<PlayerInventory>());
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
            UserInterface.instance.SetPlayerMoneyBase(TotalMoney, BaseIndex);
        }

        protected override void Die() {
            
        }


        // queries
        bool PlayerInsideRange(GameObject p) {
            return (p.Transform.position - transform.position).LengthSquared() <= deloadDistanceThreshold;
        }


        // other
        async void DeloadPlayerProgression(PlayerInventory pi) {
            float timeBetweenUnloads = .1f;
            while (pi.CarryingValue > 0 && PlayerInsideRange(pi.gameObject)) { 
                TotalMoney += pi.ValueOnTop;
                pi.DeloadOne();
                UpdateUI();
                await Time.WaitForSeconds(timeBetweenUnloads);
            }
        }

    }

}