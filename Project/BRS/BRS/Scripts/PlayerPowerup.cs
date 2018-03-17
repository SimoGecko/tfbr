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
        int maxNumberPowerUps = 1;
        List<Powerup> carryingPowerUp = new List<Powerup>();

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() { }
        public override void Update() {
            /*if (Input.GetKeyDown(Keys.E) && HasPowerup) {
                UsePowerup();
            }*/
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void UsePowerup(Player p) {
            if (carryingPowerUp.Count > 0) {
                carryingPowerUp[0].UsePowerUp(p);
                carryingPowerUp.Remove(carryingPowerUp[0]);
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