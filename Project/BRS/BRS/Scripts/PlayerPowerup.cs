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
        const int maxNumberPowerUps = 1;


        //private
        List<Powerup> carryingPowerUp;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            carryingPowerUp = new List<Powerup>();

        }
        public override void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void UsePowerup(Player p) {
            //could implement selector here
            if (carryingPowerUp.Count > 0) {
                carryingPowerUp[0].UsePowerUp(p);
                carryingPowerUp.RemoveAt(0);
            }
        }

        public void Collect(Powerup powerUp) {
            carryingPowerUp.Add(powerUp);
        }

        public bool CanPickUp(Powerup powerUp) {
            return carryingPowerUp.Count < maxNumberPowerUps;
        }

        // queries
        bool HasPowerup { get { return carryingPowerUp.Count > 0; } }

        // other

    }
}