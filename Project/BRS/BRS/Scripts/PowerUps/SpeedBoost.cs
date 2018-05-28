// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Rendering;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.PowerUps {
    class SpeedBoost : Powerup {
        ////////// gives a speed boost for a limited amount of time //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float BoostTime = 3f;

        public SpeedBoost() {
            powerupType = PowerupType.Speed;
            ParticleRay = ParticleType3D.PowerUpSpeedRay;
            ParticleStar = ParticleType3D.PowerUpSpeedStar;
        }

        // --------------------- BASE METHODS ------------------

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