// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class Magnet : Powerup {
        ////////// powerup that spawns a magnet //////////

        // --------------------- VARIABLES ---------------------

        //public

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Magnet;
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate magnet slowing down stuff
            transform.position = Owner.transform.position + Vector3.Up;
            GameObject plantedMagnet = GameObject.Instantiate("plantedMagnetPrefab", transform);
        }
    }
}