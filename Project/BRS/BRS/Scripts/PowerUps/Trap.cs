// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Rendering;
using BRS.Scripts.Managers;

namespace BRS.Scripts.PowerUps {
    class Trap : Powerup {
        ////////// powerup that can be dropped to slow down enemy //////////

        // --------------------- VARIABLES ---------------------

        //public

        public Trap() {
            powerupType = PowerupType.Trap;
            ParticleRay = ParticleType3D.PowerUpTrapRay;
            ParticleStar = ParticleType3D.PowerUpTrapStar;
        }

        // --------------------- BASE METHODS ------------------

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