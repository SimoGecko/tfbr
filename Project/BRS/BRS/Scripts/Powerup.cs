// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float rotSpeed = 90;

        //private
        protected bool destroyOnUse = true;
        protected bool rotate = true;
        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            transform.Rotate(Vector3.Up, MyRandom.Value * 360);
        }

        public override void Update() {
            base.Update();
            if(rotate)
                transform.Rotate(Vector3.Up, rotSpeed * Time.deltatime);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void DoPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                pp.Collect(this);
                Spawner.instance.RemovePowerup(this);
                if(!destroyOnUse)
                    gameObject.Active = false;
                else
                    GameObject.Destroy(gameObject);
            }
        }

        public virtual void UsePowerUp(Player p) { }


        // queries



        // other

    }

    //-------------------------------------------------------------------------------------------------- all simple powerups
    class HealthPotion : Powerup {
        float valuePotion = 20;

        public override void UsePowerUp(Player p) {
            p.AddHealth(valuePotion);
        }
    }

    class HealthBoost : Powerup {
        float valueBoost = 20;

        public override void UsePowerUp(Player p) {
            p.UpdateMaxHealth(valueBoost);
        }
    }

    class StaminaPotion : Powerup {
        float valuePotion = .2f;

        public override void UsePowerUp(Player p) {
            p.AddStamina(valuePotion);
        }
    }

    class StaminaBoost : Powerup {
        float valueBoost = .2f;

        public override void UsePowerUp(Player p) {
            p.UpdateMaxStamina(valueBoost);
        }
    }

    class CapacityBoost : Powerup {
        int valueBoost = 2;

        public override void UsePowerUp(Player p) {
            p.gameObject.GetComponent<PlayerInventory>().UpdateCapacity(valueBoost);
        }
    }

    class SpeedBoost : Powerup {
        const float boostTime = 3;

        public override void UsePowerUp(Player p) {
            p.gameObject.GetComponent<PlayerMovement>().powerUpBoosting = true;
            new Timer(boostTime, () => p.gameObject.GetComponent<PlayerMovement>().powerUpBoosting = false);
        }
    }

    
}