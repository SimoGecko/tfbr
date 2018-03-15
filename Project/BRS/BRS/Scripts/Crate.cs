// (c) Simone Guggiari 2018
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
            Player p = c.gameObject.GetComponent<Player>();
            if(p!= null) {
                PlayerAttack pa = p.gameObject.GetComponent<PlayerAttack>();
                if (pa.IsAttacking)
                    CrackCrate();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CrackCrate() {
            for(int i=0; i<MyRandom.Range(minNumCoins, maxNumCoins+1); i++) {
                Spawner.instance.SpawnMoneyAround(transform.position);
            }
            GameObject.Destroy(gameObject);
        }


        // queries



        // other

    }

}