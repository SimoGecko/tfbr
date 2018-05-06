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

        public HealthPotion() {
            PowerupType = PowerupType.Health;
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
            Owner.AddHealth(ValuePotion);
        }
    }
}