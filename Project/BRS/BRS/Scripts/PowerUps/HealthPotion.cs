// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class HealthPotion : Powerup {
        ////////// increases player health //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float ValuePotion = 20;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Health;
            _useInstantly = true;

        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Owner.AddHealth(ValuePotion);
        }
    }
}