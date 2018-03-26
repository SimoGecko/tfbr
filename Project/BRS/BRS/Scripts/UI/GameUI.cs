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

        Timer roundtime;
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
            Vector2 centerPos = new Vector2(Screen.WIDTH / 2, Screen.HEIGHT / 2);

            string roundNumber = "round: " + GameManager.instance.RoundNumber + "/" + GameManager.numRounds;
            UserInterface.instance.DrawString(centerPos + new Vector2(-100, -100), roundNumber);

            string roundTimeString = "time: " + roundtime.span.ToReadableString();
            UserInterface.instance.DrawString(centerPos + new Vector2(-100, -50), roundTimeString);

            //police bar
            float barPercent = (float)(1 - roundtime.span.TotalSeconds / RoundManager.roundTime);
            UserInterface.instance.DrawBarBig(centerPos + new Vector2(-128, 0), barPercent, Color.Gray);

            //TODO show blinking police
            int fgRectWidth = (int)(UserInterface.BARBIGWIDTH * barPercent);
            UserInterface.instance.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), policeCar, Vector2.One * 64, .6f);

            if (showPolice) {
                UserInterface.instance.DrawString(centerPos + new Vector2(-50, 100), "Get back to your base!");
                if ((Time.frame / 10) % 2 == 0) {
                    UserInterface.instance.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), policeLight, Vector2.One * 64, .6f);
                }
            }

            if (showWinner) {
                UserInterface.instance.DrawStringBig(centerPos + new Vector2(-100, 200), winnerString);
            }
        }

        public void UpdateGameWinnerUI(int winner) {
            winnerString = "Player " + (winner + 1) + " won!";
            showWinner = true;
        }

        public void UpdatePoliceComing() {
            showPolice = true;
        }

        public void StartMatch(Timer rt) {
            roundtime = rt;
            showWinner = showPolice = false;
        }

        // queries



        // other

    }
}