// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Magnet : Powerup {
        ////////// powerup that spawns a magnet //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Magnet;
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            transform.position = Owner.transform.position + Vector3.Up;

            //instantiate magnet slowing down stuff
            GameObject plantedMagnet = GameObject.Instantiate("plantedMagnetPrefab", transform);
        }

        // queries


        // other

    }
}