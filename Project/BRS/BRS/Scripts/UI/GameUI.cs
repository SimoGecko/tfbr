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
            _showWinner = _showPolice = false;
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) {
            if (index == 0) return;
            index--;

            string roundString = "round " + GameManager.RoundNumber + "/" + GameManager.NumRounds;
            UserInterface.DrawString(roundString, new Rectangle(-20, 140, 100, 25), Align.TopRight, Align.TopRight, Align.Center, scale:.7f);

            //police bar
            float policePercent = (float)(1 - _roundtime.Span.TotalSeconds / RoundManager.RoundTime);
            UserInterface.DrawBarStriped(policePercent, new Rectangle(-320, 107, 175, 25), Color.LightGray, Align.TopRight);
            //police car and blinking
            int fgRectWidth = (int)(175 * policePercent);
            Rectangle policeRect = new Rectangle(-320 + fgRectWidth, 120, 64, 64);
            UserInterface.DrawPicture(_policeCar, policeRect, null, Align.TopRight, Align.Center);

            if (_showPolice) {
                //UserInterface.DrawString("Get back to your base!", new Vector2(0, 50), Align.Bottom, bold:true);
                if ((Time.Frame / 10) % 2 == 0) {
                    UserInterface.DrawPicture(_policeLight, policeRect, null, Align.TopRight, Align.Center);
                    //UserInterface.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), _policeLight, Vector2.One * 64, .6f);
                }
            }

            //time
            string roundTimeString = _roundtime.Span.ToReadableString();
            UserInterface.DrawString(roundTimeString, new Rectangle(-320 + fgRectWidth, 144, 75, 25), Align.TopRight, Align.Center, Align.Center);
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