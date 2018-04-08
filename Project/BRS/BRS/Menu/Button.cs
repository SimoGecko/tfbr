using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using BRS.Engine;

namespace BRS.Menu {
    class Button : Component {
        ////////// class to create and display a button with arbitrary function when clicked //////////

        // --------------------- VARIABLES ---------------------

        private MouseState _currentMouse;
        private MouseState _previousMouse;

        public bool IsHovering;
        public bool IsCurrentSelection;
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

        public Button NeighborUp { get; set; }
        public Button NeighborDown { get; set; }
        public Button NeighborLeft { get; set; }
        public Button NeighborRight { get; set; }

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
            _textureClicked = File.Load<Texture2D>("Images/UI/buttonClicked");
        }

        public override void Start() {
            base.Start();
        }

        private void UpdateSelection(Input.Stick state) {
            Input.uniqueFrameInputUsed = true;
            switch (state) {
                case Input.Stick.Up:
                    if (NeighborUp != null) {
                        NeighborUp.IsCurrentSelection = true;
                        IsCurrentSelection = false;
                    }
                    break;
                case Input.Stick.Right:
                    if (NeighborRight != null) {
                        NeighborRight.IsCurrentSelection = true;
                        IsCurrentSelection = false;
                    }
                    break;
                case Input.Stick.Down:
                    if (NeighborDown != null) {
                        NeighborDown.IsCurrentSelection = true;
                        IsCurrentSelection = false;
                    }
                    break;
                case Input.Stick.Left:
                    if (NeighborLeft != null) {
                        NeighborLeft.IsCurrentSelection = true;
                        IsCurrentSelection = false;
                    }
                    break;
            }
        }

        public override void Update() {
            if (Active) {
                base.Update();

                //_previousMouse = _currentMouse;
                //_currentMouse = Mouse.GetState();
                //var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

                if (IsCurrentSelection && !Input.uniqueFrameInputUsed) {
                    if (Input.GetKeyUp(Keys.Up) || Input.GetButtonUp(Buttons.LeftThumbstickUp))
                        UpdateSelection(Input.Stick.Up);
                    else if (Input.GetKeyUp(Keys.Right) || Input.GetButtonUp(Buttons.LeftThumbstickRight)) UpdateSelection(Input.Stick.Right);
                    else if (Input.GetKeyUp(Keys.Down) || Input.GetButtonUp(Buttons.LeftThumbstickDown))
                        UpdateSelection(Input.Stick.Down);
                    else if (Input.GetKeyUp(Keys.Left) || Input.GetButtonUp(Buttons.LeftThumbstickLeft)) UpdateSelection(Input.Stick.Left);
                }

                IsHovering = false;
                if (IsCurrentSelection /*mouseRectangle.Intersects(Rectangle)*/) {
                    IsHovering = true;
                    if (!Input.uniqueFrameInputUsed && (Input.GetKeyUp(Keys.Enter) || Input.GetButtonUp(Buttons.A))) {
                        Input.uniqueFrameInputUsed = true;
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
