// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Crate : Component, IDamageable {
        ////////// represents a crate that can be cracked when attacked and reveals money and powerup inside //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float crackSpawnRadius = 1f;

        const int minNumCoins = 1;
        const int maxNumCoins = 8;
        const float probOfPowerup = .2f;


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            if(c.GameObject.tag == ObjectTag.Player) {
                PlayerAttack pa = c.GameObject.GetComponent<PlayerAttack>();
                if (pa.IsAttacking)
                    CrackCrate();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CrackCrate() {
            int numCoins = MyRandom.Range(minNumCoins, maxNumCoins + 1);
            for (int i=0; i<numCoins; i++) {
                Spawner.instance.SpawnMoneyAround(transform.position, crackSpawnRadius);
            }

            if (MyRandom.Value <= probOfPowerup) {
                Spawner.instance.SpawnPowerupAround(transform.position, crackSpawnRadius);
            }
            Elements.instance.Remove(this);
            GameObject.Destroy(gameObject);
        }


        public void TakeDamage(float damage) {
            CrackCrate();
        }


        // queries



        // other

    }

}