using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BRS.Menu {
    class Button : Component {
        ////////// class to create and display a button with arbitrary function when clicked //////////

        // --------------------- VARIABLES ---------------------

        private MouseState _currentMouse;
        private MouseState _previousMouse;

        public bool IsHovering;
        public Texture2D Texture;

        public EventHandler Click;

        public float ScaleWidth { get; set; } = 1;
        public float ScaleHeight { get; set; } = 1;

        public Vector2 OffsetTexture = new Vector2(0, 0);
        public Vector2 InitPos;
        private Vector2 Position { get { return InitPos - OffsetTexture; } }

        public string NameMenuToSwitchTo { get; set; }
        public string Text { get; set; }

        public bool IsClicked;
        Texture2D _textureClicked;
        public int Index { get; set; }

        public List<Button> Neighbors;

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * ScaleWidth), (int)(Texture.Height * ScaleHeight));
            }
        }

        // --------------------- BASE METHODS ------------------
        public Button(Texture2D t, Vector2 pos) {
            Texture = t;
            InitPos = pos;
            OffsetTexture = new Vector2(t.Width / 2, t.Height / 2);
            IsClicked = false;
            Active = true;
            Neighbors = new List<Button>();
            _textureClicked = File.Load<Texture2D>("Images/UI/buttonClicked");
        }

        public override void Start() {
            base.Start();
        }

        public override void Update() {
            if (Active) {
                base.Update();

                _previousMouse = _currentMouse;
                _currentMouse = Mouse.GetState();

                var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

                IsHovering = false;
                if (mouseRectangle.Intersects(Rectangle)) {
                    IsHovering = true;
                    if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed
                        || Input.GetButtonUp(Buttons.A)) {
                        Click?.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        public override void Draw() {
            if (Active) {
                var colour = Color.White;
                if (IsHovering)
                    colour = Color.Gray;

                if (IsClicked && _textureClicked != null)
                    UserInterface.Instance.DrawPicture(Rectangle, _textureClicked, colour);
                else if (Texture != null)
                    UserInterface.Instance.DrawPicture(Rectangle, Texture, colour);

                if (!string.IsNullOrEmpty(Text)) {
                    var x = (Rectangle.X + Rectangle.Width / 2) - (UserInterface.Instance.SmallFont.MeasureString(Text).X / 2);
                    var y = (Rectangle.Y + Rectangle.Height / 2) - (UserInterface.Instance.SmallFont.MeasureString(Text).Y / 2);

                    UserInterface.Instance.DrawString(new Vector2(x, y), Text, Color.Black);
                }
            }
        }

        // --------------------- CUSTOM METHODS ----------------
    }
}
