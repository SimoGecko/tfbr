// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;
using BRS.Scripts.Managers;
using BRS.Scripts.Elements;

namespace BRS.Scripts.PowerUps {
    class Bomb : Powerup {
        ////////// bomb that can be planted //////////

        // --------------------- VARIABLES ---------------------

        //public
        public Bomb() {
            powerupType = PowerupType.Bomb;
        }
        
        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
        }


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