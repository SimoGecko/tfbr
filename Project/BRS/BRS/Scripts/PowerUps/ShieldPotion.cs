// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Rendering;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;

namespace BRS.Scripts.PowerUps {
    class ShieldPotion : Powerup {
        ////////// increases base health //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float ValuePotion = 20f;

        public ShieldPotion() {
            powerupType = PowerupType.Shield;
            ParticleRay = ParticleType3D.PowerUpShieldRay;
            ParticleStar = ParticleType3D.PowerUpShieldStar;
            _useInstantly = true;
        }

        // --------------------- BASE METHODS ------------------

        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Base b = ElementManager.Instance.Base(Owner.TeamIndex);
            if (b != null)  b.AddHealth(ValuePotion);
        }
    }
}