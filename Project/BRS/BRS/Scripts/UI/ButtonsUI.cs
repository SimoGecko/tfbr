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
        ////////// shows on screen the buttons with wiggle effect //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private Texture2D _xboxButtons;
        private List<ButtonCommand> _commands;

        // const
        private const int ButtonSize = 105;

        //reference
        public static ButtonsUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _xboxButtons = File.Load<Texture2D>("Images/UI/xbox_buttons");
            _commands = new List<ButtonCommand>();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) {
            if (index == -1) return;

            foreach (ButtonCommand c in _commands) {
                if (c.Index == index) {
                    bool wiggle = (int)Time.CurrentTime % 5 == 0;
                    float angle = (wiggle) ? (float)System.Math.Sin(Time.CurrentTime * 40) : 0;
                    UserInterface.DrawPicture(_xboxButtons, c.dest, SourceRectangle(c.Button), c.anchor, Align.Center, rot: 10 * angle);
                }
            }
            if(index==GameManager.NumPlayers-1)
                _commands.Clear(); // at end of all calls
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
            return new Rectangle(column * ButtonSize, row * ButtonSize, ButtonSize, ButtonSize);
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