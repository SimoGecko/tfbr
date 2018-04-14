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

        public float ScaleWidthInside { get; set; } = 1;
        public float ScaleHeightInside { get; set; } = 1;

        public Vector2 OffsetTexture = new Vector2(0, 0);
        public Vector2 InitPos;
        private Vector2 Position { get { return InitPos + new Vector2(Screen.Width / 1920f, Screen.Height / 1080f) /*- OffsetTexture*/; } }

        public string NameMenuToSwitchTo { get; set; }
        public string Text { get; set; }
        public Texture2D InsideImage { get; set; } // instead of text

        public bool IsClicked;
        Texture2D _textureClicked;
        public int Index { get; set; }

        public Button NeighborUp { get; set; }
        public Button NeighborDown { get; set; }
        public Button NeighborLeft { get; set; }
        public Button NeighborRight { get; set; }

        public Color InsideObjectColor = new Color(144, 144, 144);
        public Color ImageColor = new Color(247, 239, 223);

        public List<Button> neighbors;

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)(Position.X - (Texture.Width * Screen.Width / 1920f * (1-ScaleWidth) / 2)), (int)Position.Y - (int)(Texture.Height * Screen.Height / 1080f * (1 - ScaleHeight) / 2), (int)(Texture.Width * Screen.Width / 1920f * ScaleWidth), (int)(Texture.Height * Screen.Height / 1080f * ScaleHeight));
            }
        }

        public Rectangle RectangleNotScaled {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * Screen.Width / 1920f), (int)(Texture.Height * Screen.Height / 1080f));
            }
        }

        public Rectangle RectangleInsideObject {
            get {
                Vector2 offsetInside = new Vector2((Texture.Width - InsideImage.Width) / 2 * (1 - ScaleWidth) * (1 - ScaleWidthInside), (Texture.Height - InsideImage.Height) / 2 * (1 - ScaleHeight) * (1 - ScaleHeightInside));
                return new Rectangle((int)(Position.X + offsetInside.X), (int)(Position.Y + offsetInside.Y), (int)(InsideImage.Width * ScaleWidthInside), (int)(InsideImage.Height * ScaleHeightInside));
            }
        }

        // --------------------- BASE METHODS ------------------
        public Button(Texture2D t, Vector2 pos) {
            Texture = t;
            InitPos = pos;
            //OffsetTexture = new Vector2(t.Width / 2, t.Height / 2);
            IsClicked = false;
            Active = true;
            _textureClicked = File.Load<Texture2D>("Images/UI/buttonClicked");
            neighbors = new List<Button>();
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

        public override void Draw(int i) {
            if (Active) {
                var colour = ImageColor;

                if (IsClicked)
                    colour = Color.DeepSkyBlue;

                if (IsHovering)
                    colour = Color.Gray;

                if (Texture != null)
                    UserInterface.DrawPicture(Texture, Rectangle, null, Align.TopLeft, Align.Center, colour, false);
                //UserInterface.DrawPictureOLD(Rectangle, Texture, colour);


                if (InsideImage != null)
                    UserInterface.DrawPicture(InsideImage, RectangleInsideObject, null, Align.TopLeft, Align.Center, InsideObjectColor, false);
                    //UserInterface.DrawPictureOLD(RectangleInsideObject, InsideImage, InsideObjectColor);
                

                if (!string.IsNullOrEmpty(Text)) {
                    var x = (Rectangle.X + Rectangle.Width / 2) - (UserInterface.comicFont.MeasureString(Text).X / 2);
                    var y = (Rectangle.Y + Rectangle.Height / 2) - (UserInterface.comicFont.MeasureString(Text).Y / 2);

                    UserInterface.DrawString(Text, RectangleNotScaled, Align.TopLeft, Align.Center, Align.Center, InsideObjectColor, false);
                    //UserInterface.DrawStringOLD(new Vector2(x, y), Text, Color.Black);

                }
            }
        }

        // --------------------- CUSTOM METHODS ----------------
    }
}
