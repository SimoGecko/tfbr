// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class Bomb : Powerup {
        ////////// bomb that can be planted and explodes after some time damaging what's around //////////

        // --------------------- VARIABLES ---------------------

        //public
        
        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Bomb;
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            transform.position = Owner.transform.position + Vector3.Up;
            GameObject plantedBomb = GameObject.Instantiate("plantedBombPrefab", transform);
        }
    }
}