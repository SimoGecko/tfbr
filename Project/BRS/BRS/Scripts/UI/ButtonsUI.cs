// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    public enum XboxButtons { A, X, Y, B, LB, RB, LT, RT, DD, DL, DR, DU, D, L, R, M, Null };

    class ButtonsUI : Component {
        ////////// shows on screen tips for the player //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private Texture2D _xboxButtons;
        private List<ButtonCommand> _commands;

        // const
        private const int AtlasWidth = 105;
        private const int ButWidth = 50;

        //reference
        public static ButtonsUI Instance;
        public Transform Player;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _xboxButtons = File.Load<Texture2D>("Images/UI/xbox_buttons");
            _commands = new List<ButtonCommand>();

            Player p = ElementManager.Instance.Player(0);
            if (p != null) Player = p.transform;
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) {
            if (index == 0) return;
            index--;

            foreach (ButtonCommand c in _commands) {
                if (c.Index == index) {
                    //Rectangle destination = new Rectangle((int)c.Pos.X, (int)c.Pos.Y, ButWidth, ButWidth);
                    bool wiggle = (int)Time.CurrentTime % 5 == 0;
                    float angle = (wiggle) ? (float)System.Math.Sin(Time.CurrentTime * 40) : 0;
                    UserInterface.DrawPicture(_xboxButtons, c.dest, SourceRectangle(c.Button), c.anchor, Align.Center, rot: 10 * angle);
                }
            }
            _commands.Clear();
        }

        public void GiveCommand(int _index, Rectangle _dest, XboxButtons _button, Align _anchor, bool flip=false) {
            if (flip) {
                _anchor = UserInterface.Flip(_anchor);
                _dest.X *= -1;
            }
            _commands.Add(new ButtonCommand() { Index = _index, dest = _dest, Button = _button, anchor = _anchor });
        }


        // queries
        public static Rectangle SourceRectangle(XboxButtons button) {
            int column = (int)button % 4;
            int row = (int)button / 4;
            return new Rectangle(column * AtlasWidth, row * AtlasWidth, AtlasWidth, AtlasWidth);
        }



        // other

        struct ButtonCommand {
            public int Index;
            public Rectangle dest;
            public Align anchor;
            public XboxButtons Button;
        }

    }
}