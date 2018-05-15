// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    class GameUI : Component {
        ////////// draws info related to the game and rounds //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private Texture2D _policeCar, _policeLight;
        private Texture2D blackTex;

        private Timer _roundtime;
        private bool _showWinner;
        private bool _showPolice;
        private string _winnerString = "";

        //reference
        public static GameUI Instance;

        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            Instance = this;
        }

        public override void Start() {
            _policeCar = File.Load<Texture2D>("Images/UI/policeCar");
            _policeLight = File.Load<Texture2D>("Images/UI/policeCar_lights");
            blackTex = File.Load<Texture2D>("Images/UI/black");
            _showWinner = _showPolice = false;
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) {
            if (index == 0) {
                //draw vertical center line
                UserInterface.DrawPicture(blackTex, new Rectangle(0, 0, 4, 2000), null, Align.Right, Align.Center);
                return;
            }
            index--;

            string roundString = "round " + RoundManager.RoundNumber + "/" + RoundManager.NumRounds;
            UserInterface.DrawString(roundString, new Rectangle(-20, 140, 100, 25), Align.TopRight, Align.TopRight, Align.Center, scale:.7f);

            //police bar
            float policePercent = (float)(1 - _roundtime.Span.TotalSeconds / RoundManager.RoundTime);
            UserInterface.DrawBarStriped(policePercent, new Rectangle(-270, -310, 250, 25), Color.LightGray, Align.BotRight);
            //police car and blinking
            int fgRectWidth = (int)(250 * policePercent);
            Rectangle policeRect = new Rectangle(-270 + fgRectWidth, -298, 64, 64);
            UserInterface.DrawPicture(_policeCar, policeRect, null, Align.BotRight, Align.Center);

            if (_showPolice) {
                //UserInterface.DrawString("Get back to your base!", new Vector2(0, 50), Align.Bottom, bold:true);
                if ((Time.Frame / 10) % 2 == 0) {
                    UserInterface.DrawPicture(_policeLight, policeRect, null, Align.TopRight, Align.Center);
                    //UserInterface.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), _policeLight, Vector2.One * 64, .6f);
                }
            }

            //time
            string roundTimeString = _roundtime.Span.ToReadableString();
            UserInterface.DrawString(roundTimeString, new Rectangle(-20, -310, 75, 25), Align.BotRight, Align.BotRight, Align.Right);
            UserInterface.DrawString("time left:", new Rectangle(-145, -310, 125, 25), Align.BotRight, Align.BotRight, Align.Left, scale:.7f);
        }
        

        public void UpdateGameWinnerUI(int winner) {
            _winnerString = "Player " + (winner + 1) + " won!";
            _showWinner = true;
        }

        public void UpdatePoliceComing() {
            _showPolice = true;
        }

        public void StartMatch(Timer rt) {
            _roundtime = rt;
            _showWinner = _showPolice = false;
        }

        // queries



        // other

    }
}