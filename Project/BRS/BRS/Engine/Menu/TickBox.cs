// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine.Menu {
    class TickBox : Component {
        ////////// class to create and display a tickBox //////////

        // --------------------- VARIABLES ---------------------

        private MouseState _currentMouse;
        private MouseState _previousMouse;

        private bool _isHovering;
        private readonly Texture2D _textureNotClicked;
        private readonly Texture2D _textureClicked;

        public bool IsClicked;

        public Vector2 Position { get; set; }

        public Rectangle Rectangle {
            get {
                if (IsClicked)
                    return new Rectangle((int)(Position.X / 1920f * Screen.Width), (int)(Position.Y / 1080f * Screen.Height), (int)(_textureClicked.Width / 1920f * Screen.Width), (int)(_textureClicked.Height / 1080f * Screen.Height));
                else
                    return new Rectangle((int)(Position.X / 1920f * Screen.Width), (int)(Position.Y / 1080f * Screen.Height), (int)(_textureNotClicked.Width / 1920f * Screen.Width), (int)(_textureNotClicked.Height / 1080f * Screen.Height));
            }
        }

        // --------------------- BASE METHODS ------------------
        public TickBox(Texture2D tnotC, Texture2D tC) {
            _textureNotClicked = tnotC;
            _textureClicked = tC;
            IsClicked = false;
        }

        public override void Update() {
            base.Update();

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;
            if (mouseRectangle.Intersects(Rectangle)) {
                _isHovering = true;
                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed) {
                    IsClicked = !IsClicked;
                }
            }
        }

        // --------------------- CUSTOM METHODS ----------------
        public override void Draw(int i) {
            if (Active && i == 0) {
                var colour = Color.White;

                if (_isHovering)
                    colour = Color.Gray;

                if (IsClicked)
                    UserInterface.DrawPicture(_textureClicked, Rectangle, col: colour);
                else
                    UserInterface.DrawPicture(_textureNotClicked, Rectangle, col: colour);
            }
        }
    }
}


