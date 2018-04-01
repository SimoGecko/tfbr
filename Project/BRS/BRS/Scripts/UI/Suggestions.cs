// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    public enum XboxButtons { A, X, Y, B, LB, RB, LT, RT, DD, DL, DR, DU, D, L, R, M };

    class Suggestions : Component {
        ////////// shows on screen tips for the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        const int ATLASWIDTH = 105;
        const int BUTWIDTH = 50;

        List<ButtonCommand> commands;


        //private
        Texture2D xboxButtons;


        //reference
        public static Suggestions instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            xboxButtons = File.Load<Texture2D>("Images/UI/xbox_buttons");
            commands = new List<ButtonCommand>();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(int index) {
            foreach(ButtonCommand c in commands) {
                if(c.index == index) {
                    Rectangle destination = new Rectangle((int)c.pos.X, (int)c.pos.Y, BUTWIDTH, BUTWIDTH);
                    bool wiggle = (int)Time.time % 5 == 0;
                    float angle = (wiggle) ? (float)System.Math.Sin(Time.time * 40) : 0;
                    UserInterface.instance.DrawPicture(destination, xboxButtons, SourceRectangle(c.button), 10 * angle);
                }
            }

            commands.Clear();
        }

        public void GiveCommand(int _index, Vector2 _position, XboxButtons _button) {
            commands.Add(new ButtonCommand() { index = _index, pos = _position, button=_button });
        }



        // queries
        Rectangle SourceRectangle(XboxButtons button) {
            int column = (int)button % 4;
            int row = (int)button / 4;
            return new Rectangle(column * ATLASWIDTH, row * ATLASWIDTH, ATLASWIDTH, ATLASWIDTH);
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
            public int index;
            public Vector2 pos;
            public XboxButtons button;
        }

    }

}