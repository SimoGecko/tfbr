// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.Elements;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class ShieldPotion : Powerup {
        ////////// increases base health //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float ValuePotion = 20f;

        public ShieldPotion() {
            powerupType = PowerupType.Shield;
            _useInstantly = true;
        }

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Base b = ElementManager.Instance.Base(Owner.TeamIndex);
            if (b != null)  b.AddHealth(ValuePotion);
        }
    }
}