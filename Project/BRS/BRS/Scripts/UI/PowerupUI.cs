// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class PowerupUI : Component {
        ////////// draws the powerups for the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        string[] pngNames = { "bomb", "key", "capacity", "speed", "health", "shield", "trap" };


        //private
        Dictionary<string, int> powerupStringToIndex = new Dictionary<string, int>();
        Texture2D[] powerupsPng = new Texture2D[7];


        //reference
        PowerupUIStruct[] powerupUI;
        public static PowerupUI instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            powerupUI = new PowerupUIStruct[GameManager.numPlayers];
            for (int i = 0; i < GameManager.numPlayers; i++) powerupUI[i].currentPowerup = new List<string>();

            for (int i = 0; i < pngNames.Length; ++i) {
                powerupsPng[i] = File.Load<Texture2D>("Images/powerup/" + pngNames[i] + "_pic");
                if (!powerupStringToIndex.ContainsKey(pngNames[i]))
                     powerupStringToIndex.Add(pngNames[i], i);
            }
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(int index) { // TODO clean out this code
            int offset = UserInterface.instance.GetOffset(index);
            Rectangle destRect = new Rectangle(10 + offset, 370, 50, 50);

            if (powerupUI[index].currentPowerup != null) {
                foreach (string name in powerupUI[index].currentPowerup) {
                    UserInterface.instance.DrawPicture(destRect, powerupsPng[powerupStringToIndex[name]]);
                }
            } else {
                powerupUI[index].currentPowerup = new List<string>();
            }
        }

        public void UpdatePlayerPowerupUI(int index, string name, bool add) {
            if (add) powerupUI[index].currentPowerup.Add(name);
            else     powerupUI[index].currentPowerup.Remove(name);
        }



        // queries



        // other

    }
    public struct PowerupUIStruct {
        //power ups
        public List<string> currentPowerup;

    }
}