// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Rendering;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;

namespace BRS.Scripts.PowerUps {
    class ExplodingBox : Powerup {
        ////////// powerup that spawn a box that explodes once cracked //////////

        // --------------------- VARIABLES ---------------------

        //public
        public ExplodingBox() {
            powerupType = PowerupType.Explodingbox;
            ParticleRay = ParticleType3D.PowerUpExplodingboxRay;
            ParticleStar = ParticleType3D.PowerUpExplodingboxStar;
        }

        // --------------------- BASE METHODS ------------------

        // --------------------- CUSTOM METHODS ----------------
        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate exploding box
            GameObject newCrate = GameObject.Instantiate("cratePrefab", transform);
            Crate c = newCrate.GetComponent<Crate>();
            c.SetExplosionRigged(Owner.TeamIndex);
            ElementManager.Instance.Add(c);
        }
    }
}