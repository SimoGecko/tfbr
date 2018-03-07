// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Money : Pickup {
        ////////// represents an amount of money that can be collected  //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        int value = 1;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
        }

        public override void Update() {
            base.Update();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void OnPickup(GameObject o) {
            Player player = o.GetComponent<Player>();
            if (player != null) {
                if (player.CanPickUp) {
                    player.CollectMoney(value);
                    GameObject.Destroy(gameObject);
                }
            }
        }


        // queries



        // other

    }

}