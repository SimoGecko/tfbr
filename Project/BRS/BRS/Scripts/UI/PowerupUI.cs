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
        //string[] pngNames = { "bomb", "key", "capacity", "speed", "health", "shield", "trap" };
        const int atlasWidth = 128;

        //private
        //Dictionary<string, int> powerupStringToIndex = new Dictionary<string, int>();
        int numPowerups = System.Enum.GetValues(typeof(PowerupType)).Length;
        //Texture2D[] powerupsPng;// = new Texture2D[numPowerups];
        Texture2D powerupsAtlas;
        Rectangle[] powerupsRectangle;
        Texture2D slot;

        PowerupUIStruct[] powerupUI;

        //reference
        public static PowerupUI instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            powerupUI = new PowerupUIStruct[GameManager.numPlayers];
            for (int i = 0; i < GameManager.numPlayers; i++) powerupUI[i].currentPowerups = new int[0];

            //load pngs
            /*
            powerupsPng = new Texture2D[numPowerups];
            for (int i = 0; i < numPowerups; ++i) {
                powerupsPng[i] = File.Load<Texture2D>("Images/powerup/" + ((PowerupType)i).ToString() + "_pic");
                //if (!powerupStringToIndex.ContainsKey(pngNames[i])) powerupStringToIndex.Add(pngNames[i], i);
            }*/
            powerupsAtlas = File.Load<Texture2D>("Images/powerup/powerups"); // atlas
            powerupsRectangle = new Rectangle[numPowerups];
            for (int i = 0; i < numPowerups; ++i) {
                int column = i%4;
                int row = i/4;
                powerupsRectangle[i] = new Rectangle(column*atlasWidth, row*atlasWidth, atlasWidth, atlasWidth);
            }

            slot = File.Load<Texture2D>("Images/powerup/powerup_slot");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(int index) { // TODO clean out this code
            Vector2 position = new Vector2(300, 100);
            position += Vector2.UnitX * UserInterface.instance.GetOffset(index);
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, 50, 50);

            UserInterface.instance.DrawPicture(destRect, slot);

            if (powerupUI[index].currentPowerups.Length>0) { // it's going to draw just one
                foreach (int powerup in powerupUI[index].currentPowerups) {
                    //UserInterface.instance.DrawPicture(destRect, powerupsPng[powerup]);
                    UserInterface.instance.DrawPicture(destRect, powerupsAtlas, powerupsRectangle[powerup]);
                    Suggestions.instance.GiveCommand(index, destRect.Evaluate(new Vector2(.5f, 1)) + new Vector2(0, 30), XboxButtons.X);
                }
            }
        }

        public void UpdatePlayerPowerupUI(int index, int[] powerupList) {
            powerupUI[index].currentPowerups = powerupList;//.Add(name);
            //else     powerupUI[index].currentPowerup.Remove(name);
        }



        // queries



        // other

    }
    public struct PowerupUIStruct {
        //power ups
        public int[] currentPowerups;

    }
}