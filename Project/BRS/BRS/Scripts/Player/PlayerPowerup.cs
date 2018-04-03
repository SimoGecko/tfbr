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
        List<Powerup> carryingPowerup; // last collected is first to use -> LIFO
        //it's like a stack. if you add more that maxNumber, the one at position 0 is deleted

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
                carryingPowerup[carryingPowerup.Count - 1].UsePowerup();
                carryingPowerup.RemoveAt(carryingPowerup.Count - 1);
            }
            PowerupUI.instance.UpdatePlayerPowerupUI(p.PlayerIndex, CarryingPowerups());
        }

        public void Collect(Powerup powerup) {
            if (carryingPowerup.Count == maxNumberPowerups) {
                carryingPowerup.RemoveAt(0);
            }
            carryingPowerup.Add(powerup);
            PowerupUI.instance.UpdatePlayerPowerupUI(powerup.owner.PlayerIndex, CarryingPowerups());
        }

        public bool CanPickUp(Powerup powerup) {
            return true;
            //return carryingPowerup.Count < maxNumberPowerups;
        }

        // queries
        bool HasPowerup { get { return carryingPowerup.Count > 0; } }

        public int[] CarryingPowerups() {
            List<int> result = new List<int>();
            foreach (var p in carryingPowerup) result.Add((int)p.powerupType);
            return result.ToArray();
        }

        // other

    }
}