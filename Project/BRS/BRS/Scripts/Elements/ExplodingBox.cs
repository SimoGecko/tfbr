// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class ExplodingBox : Powerup {
        ////////// powerup that spawn a box that explodes once cracked //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            powerupType = PowerupType.explodingbox;
        }

        public override void Update() {
            base.Update();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //instantiate exploding box
            transform.position = owner.transform.position;
            GameObject newCrate = GameObject.Instantiate("cratePrefab", transform.position + Vector3.Up * .25f, Quaternion.Identity);
            Crate c = newCrate.GetComponent<Crate>();
            c.SetExplosionRigged();
            Elements.instance.Add(c);
        }


        // queries



        // other

    }

}