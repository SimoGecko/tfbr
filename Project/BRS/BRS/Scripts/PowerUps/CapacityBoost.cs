// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Rendering;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.PowerUps {
    class CapacityBoost : Powerup {
        ////////// increases the player capacity //////////

        // --------------------- VARIABLES ---------------------
        private const int ValueBoost = 2;

        public CapacityBoost() {
            powerupType = PowerupType.Capacity;
            ParticleRay = ParticleType3D.PowerUpCapacityRay;
            ParticleStar = ParticleType3D.PowerUpCapacityStar;
            _useInstantly = true;
        }

        // --------------------- BASE METHODS ------------------

        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            Owner.gameObject.GetComponent<PlayerInventory>().UpdateCapacity(ValueBoost);
        }
    }
}