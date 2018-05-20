// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class SpeedBoost : Powerup {
        ////////// gives a speed boost for a limited amount of time //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float BoostTime = 3f;

        public SpeedBoost() {
            powerupType = PowerupType.Speed;
        }

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            PlayerMovement pm = Owner.gameObject.GetComponent<PlayerMovement>();
            pm.PowerupBoosting = true;
            new Timer(BoostTime, () => pm.PowerupBoosting = false);
        }
    }
}