// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class PlayerPowerup : Component {
        ////////// deals with storing powerups and using them //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        Powerup powerupInInventory;
       
        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() { }
        public override void Update() {
            if (Input.GetKeyDown(Keys.E) && HasPowerup) {
                UsePowerup();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void UsePowerup() {

        }
        

        // queries
        bool HasPowerup { get { return powerupInInventory != null; } }

        // other

    }
}