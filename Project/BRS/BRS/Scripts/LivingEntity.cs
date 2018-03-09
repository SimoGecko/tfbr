// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class LivingEntity : Component, IDamageable {
        ////////// base class for entities with health that take damage and die //////////

        // --------------------- VARIABLES ---------------------

        //public
        public float startingHealth;

        //private
        protected float health;
        protected bool dead;

        //reference
        public event System.Action OnDeath;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            health = startingHealth;
        }

        

        public override void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void TakeHit(float damage, RaycastHit hit) {
            //do stuff here
            TakeDamage(damage);
        }

        public void TakeDamage(float damage) {
            health -= damage;
            if (health <= 0 && !dead) {
                Die();
            }
        }

        public void Die() {
            dead = true;
            OnDeath?.Invoke();
            GameObject.Destroy(gameObject);
        }


        // queries



        // other

    }

}