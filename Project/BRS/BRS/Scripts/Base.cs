// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Base : Component {
        ////////// base in the game that has health and collects money //////////

        // --------------------- VARIABLES ---------------------

        //public
        public int BaseIndex { get; set; } = 0;

        //private
        public int TotalMoney { get; private set; }



        //reference
        Player player;


        // --------------------- BASE METHODS ------------------
        public Base(int baseIndex) {
            BaseIndex = baseIndex;
        }

        public override void Start() {
            player = GameObject.FindGameObjectWithName("player_" + BaseIndex).GetComponent<Player>();
            if (player == null) Debug.LogError("player not found");
        }

        public override void Update() {
            if (Vector3.DistanceSquared(transform.position, player.transform.position) < 2f) {
                DeloadPlayer();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void DeloadPlayer() {
            TotalMoney += player.carryingmoney;
            player.Deload();
            UpdateUI();
        }

        void UpdateUI() {
            UserInterface.instance.SetPlayerMoneyBase(TotalMoney, BaseIndex);
        }



        // queries


        // other

    }

}