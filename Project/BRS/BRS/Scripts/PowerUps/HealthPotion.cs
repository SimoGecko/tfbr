// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB


using BRS.Engine.Rendering;

namespace BRS.Scripts.PowerUps {
    class HealthPotion : Powerup {
        ////////// increases player health //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float ValuePotion = 20;

        public HealthPotion() {
            powerupType = PowerupType.Health;
            ParticleRay = ParticleType3D.PowerUpHealthRay;
            ParticleStar = ParticleType3D.PowerUpHealthStar;
            _useInstantly = true;
        }

        // --------------------- BASE METHODS ------------------

        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Owner.AddHealth(ValuePotion);
        }
    }
}