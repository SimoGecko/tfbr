// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        public string namePowerup;


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
        }

        public override void Update() {
            base.Update();
            transform.RotateAround(transform.position, Vector3.Up, 180 * Time.deltatime);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void DoPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                pp.Collect(this);
                Spawner.instance.RemovePowerup(this);
                GameObject.Destroy(gameObject);
                UserInterface.instance.UpdatePlayerPowerupUI(p.playerIndex, namePowerup, false);
            }
            
        }

        public virtual void UsePowerUp(Player p) { UserInterface.instance.UpdatePlayerPowerupUI(p.playerIndex, namePowerup, true); }

        // queries



        // other

    }

    //-------------------------------------------------------------------------------------------------- all simple powerups
    class HealthPotion : Powerup {
        float valuePotion = 20;

        public override void Start() {
            base.Start();
            namePowerup = "health";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.AddHealth(valuePotion);
        }
    }

    class HealthBoost : Powerup {
        float valueBoost = 20;

        public override void Start() {
            base.Start();
            namePowerup = "health";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.UpdateMaxHealth(valueBoost);
        }
    }

    class StaminaPotion : Powerup {
        float valuePotion = .2f;

        public override void Start() {
            base.Start();
            namePowerup = "shield";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.AddStamina(valuePotion);
        }
    }

    class StaminaBoost : Powerup {
        float valueBoost = .2f;

        public override void Start() {
            base.Start();
            namePowerup = "shield";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.UpdateMaxStamina(valueBoost);
        }
    }

    class CapacityBoost : Powerup {
        int valueBoost = 2;

        public override void Start() {
            base.Start();
            namePowerup = "capacity";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.gameObject.GetComponent<PlayerInventory>().UpdateCapacity(valueBoost);
        }
    }

    class SpeedBoost : Powerup {
        const float boostTime = 3;

        public override void Start() {
            base.Start();
            namePowerup = "speed";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            p.gameObject.GetComponent<PlayerMovement>().powerUpBoosting = true;
            new Timer(boostTime, () => p.gameObject.GetComponent<PlayerMovement>().powerUpBoosting = false);
        }
    }

    class Bomb : Powerup {
        const float timeBombToExplode = 3;
        const float radiusExplosion = 2;
        const float damageExplosion = 60;

        public override void Start() {
            base.Start();
            namePowerup = "bomb";
        }

        public override void UsePowerUp(Player p) {
            base.UsePowerUp(p);
            Vector3 posBomb = p.transform.position;
            Powerup pu = p.gameObject.GetComponent<PlayerPowerup>().DropBomb(posBomb);
            new Timer(timeBombToExplode, () => BombExplosion(pu, posBomb));
        }

        void BombExplosion(Powerup pu, Vector3 posBomb) {
            GameObject[] bases = GameObject.FindGameObjectsWithTag("base");
            GameObject[] players = GameObject.FindGameObjectsWithTag("player");
            GameObject[] vautlDoor = GameObject.FindGameObjectsWithTag("VaultDoor");

            foreach (GameObject go in bases) {
                if ((go.Transform.position - posBomb).LengthSquared() < radiusExplosion * radiusExplosion)
                    go.GetComponent<Base>().TakeHit(damageExplosion);
            }

            foreach (GameObject go in players) {
                if ((go.Transform.position - posBomb).LengthSquared() < radiusExplosion * radiusExplosion)
                    go.GetComponent<Player>().TakeHit(damageExplosion);
            }

            if (vautlDoor != null) { 
                foreach (GameObject go in vautlDoor) {
                    if ((go.Transform.position - posBomb).LengthSquared() < radiusExplosion * radiusExplosion) {
                        GameObject.Destroy(go);
                    }

                }
            }

            Spawner.instance.RemovePowerup(pu);
            GameObject.Destroy(pu.gameObject);

            
        }
    }
}