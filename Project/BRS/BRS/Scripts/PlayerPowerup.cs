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
        const int maxNumberPowerups = 1;


        //private
        List<Powerup> carryingPowerup;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            carryingPowerup = new List<Powerup>();

        }
        public override void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void UsePowerup(Player p) {
            //could implement selector here
            if (carryingPowerup.Count > 0) {
                carryingPowerup[0].UsePowerup();
                carryingPowerup.RemoveAt(0);
            }
        }

        public void Collect(Powerup powerup) {
            carryingPowerup.Add(powerup);
        }

        public bool CanPickUp(Powerup powerup) {
            return carryingPowerup.Count < maxNumberPowerups;
        }

        // queries
        bool HasPowerup { get { return carryingPowerup.Count > 0; } }

        // other

    }
}