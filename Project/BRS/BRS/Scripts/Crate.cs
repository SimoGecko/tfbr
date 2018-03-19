﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Crate : Component {
        ////////// represents a crate that can be cracked when attacked and reveals money and powerup inside //////////

        // --------------------- VARIABLES ---------------------

        //public
        const int minNumCoins = 1;
        const int maxNumCoins = 8;


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            if(c.gameObject.myTag == "player") {
                PlayerAttack pa = c.gameObject.GetComponent<PlayerAttack>();
                if (pa.IsAttacking)
                    CrackCrate();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CrackCrate() {
            int numCoins = MyRandom.Range(minNumCoins, maxNumCoins + 1);
            for (int i=0; i<numCoins; i++) {
                Spawner.instance.SpawnMoneyAround(transform.position);
            }
            Spawner.instance.RemoveCrate(this);
            GameObject.Destroy(gameObject);
        }


        // queries



        // other

    }

}