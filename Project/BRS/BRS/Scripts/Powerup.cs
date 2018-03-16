// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


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
        protected override void OnPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                pp.Collect(this);
                GameObject.Destroy(gameObject);
            }
        }

        public virtual void UsePowerUp(Player p) { }


        // queries



        // other

    }


    class HealthPotion : Powerup {
        ////////// Health potion: regain some life //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        float valuePotion = 20;

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
        protected override void OnPickup(Player p) {
            base.OnPickup(p);
        }

        public override void UsePowerUp(Player p) {
            p.AddHealth(valuePotion);
        }

        // queries



        // other

    }
    class HealthBoost : Powerup {
        ////////// Health potion: regain some life //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        float valueBoost = 20;

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
        protected override void OnPickup(Player p) {
            base.OnPickup(p);
        }

        public override void UsePowerUp(Player p) {
            p.UpdateMaxHealth(valueBoost);
        }

        // queries



        // other

    }

    class StaminaPotion : Powerup {
        ////////// Health potion: regain some life //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        float valuePotion = .2f;

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
        protected override void OnPickup(Player p) {
            base.OnPickup(p);
        }

        public override void UsePowerUp(Player p) {
            p.AddStamina(valuePotion);
        }

        // queries



        // other

    }
    class StaminaBoost : Powerup {
        ////////// Health potion: regain some life //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        float valueBoost = .2f;

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
        protected override void OnPickup(Player p) {
            base.OnPickup(p);
        }

        public override void UsePowerUp(Player p) {
            p.UpdateMaxStamina(valueBoost);
        }

        // queries



        // other

    }

    class CapacityBoost : Powerup {
        ////////// Health potion: regain some life //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        int valueBoost = 2;

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
        protected override void OnPickup(Player p) {
            base.OnPickup(p);
        }

        public override void UsePowerUp(Player p) {
            p.gameObject.GetComponent<PlayerInventory>().UpdateCapacity(valueBoost);
        }

        // queries



        // other

    }
}