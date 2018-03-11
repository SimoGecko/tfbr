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
        int weight = 1;

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
            PlayerInventory player = o.GetComponent<PlayerInventory>();
            if (player != null) {
                if (player.CanPickUp(this)) {
                    player.Collect(this);
                    GameObject.Destroy(gameObject);
                }
            }
        }


        // queries
        public int Value  { get { return value; } }
        public int Weight { get { return weight; } }


        // other

    }

}