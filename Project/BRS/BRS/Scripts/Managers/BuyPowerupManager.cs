// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PowerUps;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class BuyPowerupManager : Component {
        ////////// DESCRIPTION //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        bool[] hasBoughtPowerup;
        //int[] powerupBought;


        //reference
        public static BuyPowerupManager Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            hasBoughtPowerup = new bool[GameManager.NumPlayers];
            //powerupBought = new int[GameManager.NumPlayers];
        }

        public override void Update() {
            if(GameManager.state == GameManager.State.Ended)
                CheckForBuyInput();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CheckForBuyInput() {
            for(int i=0; i<GameManager.NumPlayers; i++) {
                if(Input.GetButtonDown(Buttons.X, i) || (i==0 && Input.GetKeyDown(Keys.X))) {
                    if(CanBuyPowerup(i))
                        BuyPowerup(i);
                }
            }
        }

        void BuyPowerup(int i) {
            Spend1000(i);
            hasBoughtPowerup[i] = true;
            //powerupBought[i] = MyRandom.Range(0, Powerup.NumPowerups);
            //powerupBought[i] = Powerup.StringToInt(GameMode.GetCurrentGameMode().RandomPowerup);
        }

        public void ResetCustom() {
            //Debug.Log("powerup buy manager is reset");
            for(int i=0; i<GameManager.NumPlayers; i++) {
                hasBoughtPowerup[i] = false;
                //powerupBought[i] = 0;
            }
        }
        void Spend1000(int i) {
            ElementManager.Instance.Base(GameManager.TeamIndex(i)).Spend(1000);
        }

        /*
        public bool GetAndConsumePowerup(int index, out int bp) {
            bp = powerupBought[index];
            return hasBoughtPowerup[index];
            
        }*/


        // queries
        public bool CanBuyPowerup(int i) {
            return !hasBoughtPowerup[i] && CanAfford1000(i);
        }
        public bool HasBoughtPowerup(int i) {
            return hasBoughtPowerup[i];
        }
        /*
        public int BoughtPowerup(int i) {
            return powerupBought[i];
        }*/

        bool CanAfford1000(int i) {
            return ElementManager.Instance.Base(GameManager.TeamIndex(i)).CanAfford(1000);
        }


        // other

    }
}