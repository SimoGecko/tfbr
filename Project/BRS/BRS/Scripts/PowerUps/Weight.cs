// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class Weight : Powerup {
        ////////// powerup that spawn a weight that falls on top of the enemy player //////////

        const float WeightSpawnHeight = 5;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Weight;
        }

        // --------------------- CUSTOM METHODS ----------------
        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate falling weight
            Player randomEnemyPlayer = ElementManager.Instance.Enemy(Owner.TeamIndex);
            GameObject fallingWeight = GameObject.Instantiate("fallingWeightPrefab", randomEnemyPlayer.transform.position + Vector3.Up * WeightSpawnHeight, Quaternion.Identity);
        }
    }
}