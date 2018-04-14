// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class Weight : Powerup {
        ////////// powerup that spawn a weight that falls on top of the enemy player //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float WeightSpawnHeight = 5;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Weight;
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate falling weight
            Player randomEnemyPlayer = ElementManager.Instance.Enemy(Owner.TeamIndex);
            Vector3 spawnPos = randomEnemyPlayer.transform.position + Vector3.Up * WeightSpawnHeight;
            GameObject fallingWeight = GameObject.Instantiate("fallingWeightPrefab", spawnPos, MyRandom.YRotation());
            ElementManager.Instance.Add(fallingWeight);
        }
    }
}