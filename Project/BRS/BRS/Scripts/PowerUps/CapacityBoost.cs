// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class CapacityBoost : Powerup {
        ////////// increases the player capacity //////////

        // --------------------- VARIABLES ---------------------
        private const int ValueBoost = 2;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Capacity;
            _useInstantly = true;
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Owner.gameObject.GetComponent<PlayerInventory>().UpdateCapacity(ValueBoost);
        }
    }
}