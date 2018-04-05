// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Crate : Component, IDamageable {
        ////////// represents a crate that can be cracked when attacked and reveals money and powerup inside //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float crackSpawnRadius = 2f;

        const int minNumCoins = 1;
        const int maxNumCoins = 8;
        const float probOfPowerup = .2f;


        //for rigged boxes
        const float explosionRadius = 1f;
        const float explosionDamage = 30;
        bool cracked = false;

        //private
        bool explosionRigged = false;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            explosionRigged = cracked = false;
        }

        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            if(c.gameObject.tag == ObjectTag.Player) {
                PlayerAttack pa = c.gameObject.GetComponent<PlayerAttack>();
                if (pa.IsAttacking)
                    CrackCrate();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CrackCrate() {
            cracked = true;
            if (explosionRigged) Explode();
            else SpawnValuables();
            
            Elements.instance.Remove(this);
            GameObject.Destroy(gameObject);
        }

        void SpawnValuables() {
            int numCoins = MyRandom.Range(minNumCoins, maxNumCoins + 1);
            for (int i = 0; i < numCoins; i++) {
                Spawner.instance.SpawnMoneyAround(transform.position, crackSpawnRadius);
            }

            if (MyRandom.Value <= probOfPowerup) {
                Spawner.instance.SpawnPowerupAround(transform.position, crackSpawnRadius);
            }
        }

        void Explode() {
            //same code as in bomb
            Collider[] overlapColliders = BRS.Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider c in overlapColliders) {
                if (c.gameObject.HasComponent<IDamageable>()) {
                    c.gameObject.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                }
            }
        }


        public void TakeDamage(float damage) { // TODO is it necessary to have both?
            if(!cracked)CrackCrate();
        }

        public void SetExplosionRigged() { explosionRigged = true; }


        // queries



        // other

    }

}