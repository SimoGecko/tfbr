// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class GameUI : Component {
        ////////// draws info related to the game and rounds //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        Texture2D policeCar, policeLight;

        public Timer roundtime;
        bool showWinner;
        bool showPolice;
        string winnerString = "";

        //reference
        public static GameUI instance;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            showWinner = showPolice = false;

            policeCar = File.Load<Texture2D>("Images/UI/policeCar");
            policeLight = File.Load<Texture2D>("Images/UI/policeCar_lights");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw() {
            Vector2 centerPos = new Vector2(Screen.WIDTH / 2 - 100, Screen.HEIGHT / 2 - 100);

            string roundTimeString = "round: " + roundtime.span.ToReadableString();
            UserInterface.instance.DrawString(centerPos, roundTimeString);

            //police bar
            float barPercent = (float)(1 - roundtime.span.TotalSeconds / RoundManager.roundTime);
            UserInterface.instance.DrawBar(centerPos + new Vector2(0, 60), barPercent, Color.Gray);

            //TODO show blinking police
            int fgRectWidth = (int)(UserInterface.BARWIDTH * barPercent);
            UserInterface.instance.DrawPicture(centerPos + new Vector2(fgRectWidth, 67), policeCar, Vector2.One * 64, .6f);

            if (showPolice) {
                UserInterface.instance.DrawString(centerPos + new Vector2(-50, 100), "Get back to your base!");
                if ((Time.frame / 10) % 2 == 0) {
                    UserInterface.instance.DrawPicture(centerPos + new Vector2(fgRectWidth, 67), policeLight, Vector2.One * 64, .6f);
                }
            }

            if (showWinner) {
                UserInterface.instance.DrawString(centerPos + new Vector2(-100, 100), winnerString);
            }
        }

        public void UpdateGameWinnerUI(int winner) {
            winnerString = "Player " + (winner + 1) + " won!";
            showWinner = true;
        }

        public void UpdatePoliceComing() {
            showPolice = true;
        }

        // queries



        // other

    }
}