// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    public interface IDamageable : IComponent {
        void TakeDamage(float damage);
    }

    class LivingEntity : Component, IDamageable {
        ////////// base class for entities with health that take damage and die //////////

        // --------------------- VARIABLES ---------------------

        //public
        public float startingHealth = 100;

        //private
        protected float health;
        protected bool dead;

        //reference
        public event System.Action OnDeath;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Respawn();
        }

        

        public override void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public virtual void TakeDamage(float damage) {
            health -= damage;
            if (health < 0) health = 0;
            if (health <= 0 && !dead) {
                Die();
            }
        }

        protected virtual void Die() {
            dead = true;
            OnDeath?.Invoke();
            //GameObject.Destroy(gameObject);
        }

        protected virtual void Respawn() {
            health = startingHealth;
            dead = false;
        } 

        public void AddHealth(float amount) {
            health = MathHelper.Min(health + amount, startingHealth);
        }

        public void UpdateMaxHealth(float amountToAdd) {
            startingHealth += amountToAdd;
        }


        // queries
        public float HealthPercent { get { return health / startingHealth; } }
        public float Health { get { return health; } }



        // other

    }

}