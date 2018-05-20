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
        private Texture2D _blackTex;

        private Timer _roundtime;
        private bool _showWinner;
        private bool _showPolice;

        //reference
        public static GameUI Instance;

        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            Instance = this;
        }

        public override void Start() {
            _policeCar   = File.Load<Texture2D>("Images/UI/policeCar");
            _policeLight = File.Load<Texture2D>("Images/UI/policeCar_lights");
            _blackTex    = File.Load<Texture2D>("Images/UI/black");
            _showWinner = _showPolice = false;
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) {
            if (index == -1) {
                if(GameManager.NumPlayers>=2)
                    //draw vertical center line
                    UserInterface.DrawPicture(_blackTex, new Rectangle(0, 0, 5, 4000), null, Align.BotRight, Align.Center);
                if (GameManager.NumPlayers >= 4)
                    //draw horizontal center line
                    UserInterface.DrawPicture(_blackTex, new Rectangle(0, 0, 4000, 5), null, Align.BotRight, Align.Center);
                return;
            }

            string roundString = "round " + RoundManager.RoundNumber + "/" + RoundManager.NumRounds;
            UserInterface.DrawString(roundString, new Rectangle(-20, 140, 100, 25), Align.TopRight, Align.TopRight, Align.Center, scale:.7f);

            //police bar
            float policePercent = (float)(1 - _roundtime.Span.TotalSeconds / RoundManager.RoundTime);
            UserInterface.DrawBarStriped(policePercent, new Rectangle(-220, -260, 200, 25), Color.LightGray, Align.BotRight);

            //police car and blinking
            int fgRectWidth = (int)(200 * policePercent);
            Rectangle policeRect = new Rectangle(-220 + fgRectWidth, -248, 64, 64);
            UserInterface.DrawPicture(_policeCar, policeRect, null, Align.BotRight, Align.Center);

            if (_showPolice) {
                if ((Time.Frame / 10) % 2 == 0) {
                    UserInterface.DrawPicture(_policeLight, policeRect, null, Align.TopRight, Align.Center);
                }
            }

            //time
            UserInterface.DrawString("time left:", new Rectangle(-95, -260, 125, 25), Align.BotRight, Align.BotRight, Align.Left, scale:.7f);
            string roundTimeString = _roundtime.Span.ToReadableString();
            UserInterface.DrawString(roundTimeString, new Rectangle(-20, -260, 75, 25), Align.BotRight, Align.BotRight, Align.Right);
        }
        

        public void UpdateGameWinnerUI(int winner) {
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