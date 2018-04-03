// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Weight : Powerup {
        ////////// powerup that spawn a weight that falls on top of the enemy player //////////

        const float weightSpawnHeight = 5;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            powerupType = PowerupType.weight;
        }

        // --------------------- CUSTOM METHODS ----------------


        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate falling weight
            Player randomEnemyPlayer = Elements.instance.Enemy(owner.teamIndex);
            GameObject fallingWeight = GameObject.Instantiate("fallingWeightPrefab", randomEnemyPlayer.transform.position + Vector3.Up * weightSpawnHeight, Quaternion.Identity);
        }
    }

    class FallingWeight : Component {
        ////////// weight that falls on top of the enemy player //////////
        // --------------------- VARIABLES ---------------------

        //public
        const float fallSpeed = 15f;
        const int fallDamage = 30;
        const float lifetime = 3f;

        //private
        bool invokedDelete = false;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }
        public override void Update() {
            if (transform.position.Y > 0) {
                transform.position += Vector3.Down * fallSpeed * Time.deltaTime;
            } else if(!invokedDelete) {
                invokedDelete = true;
                GameObject.Destroy(gameObject, lifetime);
            }
        }

        // --------------------- CUSTOM METHODS ----------------
        public override void OnCollisionEnter(Collider c) {
            if (invokedDelete) return;
            if (c.gameObject.HasComponent<IDamageable>()) {
                c.gameObject.GetComponent<IDamageable>().TakeDamage(fallDamage);
            }
        }

        // commands



        // queries



        // other
    }

}