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
            PowerupType = PowerupType.Explodingbox;
            powerupColor = Color.Orange;
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
            transform.position = Owner.transform.position;
            GameObject newCrate = GameObject.Instantiate("cratePrefab", transform.position + Vector3.Up * .25f, Quaternion.Identity);
            Crate c = newCrate.GetComponent<Crate>();
            c.SetExplosionRigged();
            ElementManager.Instance.Add(c);
        }
    }
}