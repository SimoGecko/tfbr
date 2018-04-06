// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

namespace BRS.Scripts {
    class Trap : Powerup {
        ////////// powerup that can be dropped to slow down enemy //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Trap;
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //transform.position = owner.transform.position + Vector3.Up;
            //instantiate oil trap
            GameObject.Instantiate("oilPrefab", Owner.transform.position, MyRandom.YRotation());
            Debug.Log("instant");
        }



        // queries



        // other

    }
}