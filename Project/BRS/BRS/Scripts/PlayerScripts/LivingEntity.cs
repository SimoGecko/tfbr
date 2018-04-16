// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PlayerScripts {
    public interface IDamageable : IComponent {
        void TakeDamage(float damage);
    }

    class LivingEntity : Component, IDamageable {
        ////////// base class for entities with health that take damage and die //////////

        // --------------------- VARIABLES ---------------------

        //public
        public float StartingHealth = 100;

        //private
        protected float Health;
        protected bool Dead;
        public Action OnTakeDamage;

        public LivingEntity() {
            Health = StartingHealth;
            Dead = false;
        }

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
            Health -= damage;
            OnTakeDamage?.Invoke();
            if (Health < 0) Health = 0;
            if (Health <= 0 && !Dead) {
                Die();
            }
        }

        protected virtual void Die() {
            Dead = true;
            OnDeath?.Invoke();
            //GameObject.Destroy(gameObject);
        }

        protected virtual void Respawn() {
            Health = StartingHealth;
            Dead = false;
        }

        public void AddHealth(float amount) {
            Health = MathHelper.Min(Health + amount, StartingHealth);
        }

        public void UpdateMaxHealth(float amountToAdd) {
            StartingHealth += amountToAdd;
        }


        // queries
        public float HealthPercent { get { return Health / StartingHealth; } }



        // other

    }

}