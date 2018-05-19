// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class Weight : Powerup {
        ////////// powerup that spawn a weight that falls on top of the enemy player //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float WeightSpawnHeight = 5;

        public Weight() {
            PowerupType = PowerupType.Weight;
            powerupColor = Color.Gray;
        }

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();

        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate falling weight
            Player randomEnemyPlayer = ElementManager.Instance.Enemy(Owner.TeamIndex);
            Vector3 spawnPos = randomEnemyPlayer.transform.position + Vector3.Up * WeightSpawnHeight;

            // Initialize the weight with a linear velocity which matches the player
            Vector3 velocity = Conversion.ToXnaVector(randomEnemyPlayer.gameObject.GetComponent<MovingRigidBody>().RigidBody.LinearVelocity);
            if (velocity.LengthSquared() > 0.1f) {
                velocity.Normalize();
            }
            velocity = 0.75f * randomEnemyPlayer.gameObject.GetComponent<PlayerMovement>().Speed * velocity + Vector3.Down;

            GameObject fallingWeight = GameObject.Instantiate("fallingWeightPrefab", spawnPos, MyRandom.YRotation(), velocity);
            ElementManager.Instance.Add(fallingWeight);
        }
    }
}