// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Rendering;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;

namespace BRS.Scripts.PowerUps {
    class Bomb : Powerup {
        ////////// bomb that can be planted //////////

        // --------------------- VARIABLES ---------------------

        //public
        public Bomb() {
            powerupType = PowerupType.Bomb;
            ParticleRay = ParticleType3D.PowerUpBombRay;
            ParticleStar = ParticleType3D.PowerUpBombStar;
        }
        
        // --------------------- BASE METHODS ------------------

        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            GameObject plantedBomb = GameObject.Instantiate("plantedBombPrefab", transform);
            plantedBomb.GetComponent<PlantedBomb>().Plant(Owner.TeamIndex);
            ElementManager.Instance.Add(plantedBomb);
        }
    }
}