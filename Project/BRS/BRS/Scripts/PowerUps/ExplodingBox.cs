// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.Elements;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {
    class ExplodingBox : Powerup {
        ////////// powerup that spawn a box that explodes once cracked //////////

        // --------------------- VARIABLES ---------------------

        //public
        public ExplodingBox() {
            powerupType = PowerupType.Explodingbox;
        }

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
        }


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