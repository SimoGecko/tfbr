// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Rendering;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.PowerUps {
    class StaminaPotion : Powerup {
        ////////// increases player stamina //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float ValuePotion = 1f;

        public StaminaPotion() {
            powerupType = PowerupType.Stamina;
            ParticleRay = ParticleType3D.PowerUpStaminaRay;
            ParticleStar = ParticleType3D.PowerUpStaminaStar;
            _useInstantly = false;
        }

        // --------------------- BASE METHODS ------------------

        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Owner.gameObject.GetComponent<PlayerStamina>().AddStamina(ValuePotion);
        }
    }
}