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
        public override void Start() {
            Instance = this;
            _showWinner = _showPolice = false;

            _policeCar = File.Load<Texture2D>("Images/UI/policeCar");
            _policeLight = File.Load<Texture2D>("Images/UI/policeCar_lights");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw() {
            Vector2 centerPos = new Vector2(Screen.Width / 2, Screen.Height / 2);

            string roundNumber = "round: " + GameManager.Instance.RoundNumber + "/" + GameManager.NumRounds;
            UserInterface.Instance.DrawString(centerPos + new Vector2(-100, -100), roundNumber);

            string roundTimeString = "time: " + _roundtime.Span.ToReadableString();
            UserInterface.Instance.DrawString(centerPos + new Vector2(-100, -50), roundTimeString);

            //police bar
            float barPercent = (float)(1 - _roundtime.Span.TotalSeconds / RoundManager.RoundTime);
            UserInterface.Instance.DrawBarBig(centerPos + new Vector2(-128, 0), barPercent, Color.Gray);

            //TODO show blinking police
            int fgRectWidth = (int)(UserInterface.BarBigWidth * barPercent);
            UserInterface.Instance.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), _policeCar, Vector2.One * 64, .6f);

            if (_showPolice) {
                UserInterface.Instance.DrawString(centerPos + new Vector2(-160, 50), "Get back to your base!");
                if ((Time.Frame / 10) % 2 == 0) {
                    UserInterface.Instance.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), _policeLight, Vector2.One * 64, .6f);
                }
            }

            if (_showWinner) {
                UserInterface.Instance.DrawStringBig(centerPos + new Vector2(-250, 150), _winnerString);
            }
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