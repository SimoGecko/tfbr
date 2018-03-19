// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    ////////// a collection of simple powerup POTIONS and BOOSTS that can be used and have instant effects //////////

    //HEALTH
    class HealthPotion : Powerup {
        const float valuePotion = 20;

        public override void Start() {
            base.Start();
            powerupName = "health"; // TODO distinguish
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.AddHealth(valuePotion);
        }
    }

    class HealthBoost : Powerup {
        const int valueBoost = 20;

        public override void Start() {
            base.Start();
            powerupName = "health";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.UpdateMaxHealth(valueBoost);
        }
    }

    //STAMINA
    class StaminaPotion : Powerup {
        const float valuePotion = .2f;

        public override void Start() {
            base.Start();
            powerupName = "shield"; // not shield
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.AddStamina(valuePotion);
        }
    }

    class StaminaBoost : Powerup {
        const float valueBoost = .2f;

        public override void Start() {
            base.Start();
            powerupName = "stamina";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.UpdateMaxStamina(valueBoost);
        }
    }


    //CAPACITY
    class CapacityBoost : Powerup {
        const int valueBoost = 2;

        public override void Start() {
            base.Start();
            powerupName = "capacity";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.gameObject.GetComponent<PlayerInventory>().UpdateCapacity(valueBoost);
        }
    }

    class SpeedBoost : Powerup {
        const float boostTime = 3f;

        public override void Start() {
            base.Start();
            powerupName = "speed";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            PlayerMovement pm = p.gameObject.GetComponent<PlayerMovement>();
            pm.powerUpBoosting = true;
            new Timer(boostTime, () => pm.powerUpBoosting = false);
        }
    }

}