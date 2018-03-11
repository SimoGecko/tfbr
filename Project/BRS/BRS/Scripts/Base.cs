// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Base : Component {
        ////////// base in the game that has health and collects money //////////

        // --------------------- VARIABLES ---------------------

        //public
        public int baseIndex = 0;
        const float deloadDistanceThreshold = 2f;

        //private
        int totalMoney;



        //reference
        PlayerInventory playerInventory;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            playerInventory = GameObject.FindGameObjectWithName("player_"+baseIndex).GetComponent<PlayerInventory>();
            if (playerInventory == null) Debug.LogError("player not found");
        }

        public override void Update() {
            if(Vector3.DistanceSquared(transform.position, playerInventory.transform.position) < deloadDistanceThreshold) {
                DeloadPlayer();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void DeloadPlayer() {
            totalMoney += playerInventory.CarryingValue;
            playerInventory.Deload();
            UpdateUI();
        }

        void UpdateUI() {
            UserInterface.instance.SetPlayerMoneyBase(totalMoney, baseIndex);
        }



        // queries
        public int TotalMoney { get { return totalMoney; } }


        // other

    }

}