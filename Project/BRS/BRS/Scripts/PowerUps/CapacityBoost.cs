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

        public CapacityBoost() {
            PowerupType = PowerupType.Capacity;
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
            Owner.gameObject.GetComponent<PlayerInventory>().UpdateCapacity(ValueBoost);
        }
    }
}