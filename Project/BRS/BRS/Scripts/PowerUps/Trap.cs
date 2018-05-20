// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class Trap : Powerup {
        ////////// powerup that can be dropped to slow down enemy //////////

        // --------------------- VARIABLES ---------------------

        //public

        public Trap() {
            powerupType = PowerupType.Trap;
        }

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
        }


        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate oil trap
            GameObject oil = GameObject.Instantiate("oilPrefab", Owner.transform.position, MyRandom.YRotation());
            ElementManager.Instance.Add(oil);
        }
    }
}