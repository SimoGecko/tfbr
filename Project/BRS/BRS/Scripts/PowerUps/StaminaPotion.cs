// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class StaminaPotion : Powerup {
        ////////// increases player stamina //////////

        // --------------------- VARIABLES ---------------------

        //public
        private const float ValuePotion = .2f;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Stamina;
            _useInstantly = true;
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Owner.gameObject.GetComponent<PlayerStamina>().AddStamina(ValuePotion);
        }
    }
}