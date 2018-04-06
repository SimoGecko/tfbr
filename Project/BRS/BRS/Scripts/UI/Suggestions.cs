// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    public enum XboxButtons { A, X, Y, B, LB, RB, LT, RT, DD, DL, DR, DU, D, L, R, M };

    class Suggestions : Component {
        ////////// shows on screen tips for the player //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private Texture2D _xboxButtons;
        private Texture2D _comicBubble;
        private List<ButtonCommand> _commands;

        // const
        private const int AtlasWidth = 105;
        private const int ButWidth = 50;

        //reference
        public static Suggestions Instance;
        public Transform Player;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _xboxButtons = File.Load<Texture2D>("Images/UI/xbox_buttons");
            _comicBubble = File.Load<Texture2D>("Images/UI/comic_bubble");
            _commands = new List<ButtonCommand>();

            Player p = ElementManager.Instance.Player(1);
            if (p != null) Player = p.transform;
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(int index) {
            foreach (ButtonCommand c in _commands) {
                if (c.Index == index) {
                    Rectangle destination = new Rectangle((int)c.Pos.X, (int)c.Pos.Y, ButWidth, ButWidth);
                    bool wiggle = (int)Time.CurrentTime % 5 == 0;
                    float angle = (wiggle) ? (float)System.Math.Sin(Time.CurrentTime * 40) : 0;
                    UserInterface.Instance.DrawPicture(destination, _xboxButtons, SourceRectangle(c.Button), 10 * angle);
                }
            }
            //comic bubble
            Player p = ElementManager.Instance.Player(index);
            if (p.GameObject.GetComponent<PlayerInventory>().IsFull()) {
                Point bubblePosition = Camera.Main.WorldToScreenPoint(p.transform.position).ToPoint() + new Point(0, -150);
                Rectangle dest = new Rectangle(bubblePosition, new Point(100, 100));
                UserInterface.Instance.DrawPicture(dest, _comicBubble);
                UserInterface.Instance.DrawString(bubblePosition.ToVector2() + new Vector2(10, 35), "I'm full!");
            }

            _commands.Clear();
        }

        public void GiveCommand(int _index, Vector2 _position, XboxButtons _button) {
            _commands.Add(new ButtonCommand() { Index = _index, Pos = _position, Button = _button });
        }



        // queries
        Rectangle SourceRectangle(XboxButtons button) {
            int column = (int)button % 4;
            int row = (int)button / 4;
            return new Rectangle(column * AtlasWidth, row * AtlasWidth, AtlasWidth, AtlasWidth);
        }



        // other
        /*
        async void Wiggle() {
            float duration = .1f;
            float angle = 10f;

            float percent = 0;
            while (percent < 1) {
                percent += Time.time / duration;
                float currentAngle = Curve.EvaluatePingPong(percent)*angle;
                await Time.WaitForSeconds(.01f);
            }
        }*/

        struct ButtonCommand {
            public int Index;
            public Vector2 Pos;
            public XboxButtons Button;
        }

    }

}