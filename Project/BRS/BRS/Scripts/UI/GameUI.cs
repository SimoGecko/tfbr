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

        Vector3 _soundPosition = new Vector3(0.0f, 0.0f, 0.0f);

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
        public override void Draw(int index) {
            if (index == 0) return;
            index--;
            string roundString = "round " + GameManager.RoundNumber + "/" + GameManager.NumRounds;
            UserInterface.DrawString(roundString, new Rectangle(-20, -390, 200, 20), Align.BotRight, Align.BotRight, Align.Center);

            //police bar
            float policePercent = (float)(1 - _roundtime.Span.TotalSeconds / RoundManager.RoundTime);
            UserInterface.DrawBarStriped(policePercent, new Rectangle(-220, -370, 150, 20), Color.LightGray, Align.BotRight);
            //police car and blinking
            int fgRectWidth = (int)(150 * policePercent);
            Rectangle policeRect = new Rectangle(-220 + fgRectWidth, -360, 64, 64);
            UserInterface.DrawPicture(_policeCar, policeRect, null, Align.BotRight, Align.Center);

            if (_showPolice) {
                //UserInterface.DrawString("Get back to your base!", new Vector2(0, 50), Align.Bottom, bold:true);
                if ((Time.Frame / 10) % 2 == 0) {
                    UserInterface.DrawPicture(_policeLight, policeRect, null, Align.BotRight, Align.Center);
                    //UserInterface.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), _policeLight, Vector2.One * 64, .6f);
                }
            }

            //time
            string roundTimeString = _roundtime.Span.ToReadableString();
            UserInterface.DrawString(roundTimeString, new Rectangle(-20, -350, 50, 20), Align.BotRight, Align.BotRight, Align.Right);
        }

        public void DrawOldUI2() {
            /*
            Vector2 centerPos = new Vector2(Screen.Width / 2, Screen.Height / 2);

            string roundNumber = "round: " + GameManager.Instance.RoundNumber + "/" + GameManager.NumRounds;
            UserInterface.DrawStringOLD(centerPos + new Vector2(-100, -100), roundNumber);

            string roundTimeString = "time: " + _roundtime.Span.ToReadableString();
            UserInterface.DrawStringOLD(centerPos + new Vector2(-100, -50), roundTimeString);

            //police bar
            float barPercent = (float)(1 - _roundtime.Span.TotalSeconds / RoundManager.RoundTime);
            UserInterface.DrawBarBig(centerPos + new Vector2(-128, 0), barPercent, Color.Gray);

            int fgRectWidth = (int)(UserInterface.BigBarWidth * barPercent);
            UserInterface.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), _policeCar, Vector2.One * 64, .6f);

            if (_showPolice) {
                UserInterface.DrawStringOLD(centerPos + new Vector2(-160, 50), "Get back to your base!");
                if ((Time.Frame / 10) % 2 == 0) {
                    UserInterface.DrawPicture(centerPos + new Vector2(-128 + fgRectWidth, 7), _policeLight, Vector2.One * 64, .6f);
                }
                Audio.Play("police", _soundPosition);
            }

            if (_showWinner) {
                UserInterface.DrawStringBig(centerPos + new Vector2(-250, 150), _winnerString);
            } 
            */
>>>>>>> develop
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