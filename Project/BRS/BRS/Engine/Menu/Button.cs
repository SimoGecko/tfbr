// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.Managers;

namespace BRS.Engine.Menu {
    public class Button : MenuComponent {
        ////////// class to create and display a button with arbitrary function when clicked //////////

        // --------------------- VARIABLES ---------------------
        public bool IsHovering;
        public Texture2D Texture;

        public EventHandler Click;

        public float ScaleWidth { get; set; } = 1;
        public float ScaleHeight { get; set; } = 1;
        public float ScaleWidthInside { get; set; } = 1;
        public float ScaleHeightInside { get; set; } = 1;

        public Vector2 InitPos;
        public Vector2 Position { get { return InitPos * new Vector2(Screen.Width / 1920f, Screen.Height / 1080f); } }

        public string NameMenuToSwitchTo { get; set; }
        public string Text { get; set; }
        public Texture2D InsideImage { get; set; } // instead of text

        public bool IsClicked;
        public int Index { get; set; }

        public Color InsideObjectColor = new Color(144, 144, 144);
        public Color ImageColor = new Color(247, 239, 223);

        public List<Button> neighbors;

        public bool hilightsChoice1 = true;
        public bool hilightsChoice2 = false;

        public int indexAssociatedPlayerScreen = 0;
        public bool deSelectOnMove = false;

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Texture.Width * ScaleWidth / 1920f * Screen.Width), (int)(Texture.Height * ScaleHeight / 1080f * Screen.Height));
            }
        }

        public Rectangle RectangleInsideObject {
            get {
                Vector2 offsetInside = new Vector2((Texture.Width - InsideImage.Width) / 2 * (1 - ScaleWidth) * (1 - ScaleWidthInside), (Texture.Height - InsideImage.Height) / 2 * (1 - ScaleHeight) * (1 - ScaleHeightInside));
                return new Rectangle((int)(Position.X + offsetInside.X), (int)(Position.Y + offsetInside.Y), (int)(InsideImage.Width * ScaleWidthInside / 1920f * Screen.Width), (int)(InsideImage.Height * ScaleHeightInside / 1080f * Screen.Height));
            }
        }

        // --------------------- BASE METHODS ------------------
        public Button() {
            IsClicked = false;
            Active = true;
            neighbors = new List<Button>();
        }

        public Button(Texture2D t, Vector2 pos) {
            Texture = t;
            InitPos = pos;
            IsClicked = false;
            Active = true;
            neighbors = new List<Button>();
        }

        public override void Start() {
            base.Start();
        }

        private void UpdateSelection(Input.Stick state) {
            MenuManager.uniqueFrameInputUsed[indexAssociatedPlayerScreen] = true;
            if (deSelectOnMove) IsClicked = false;
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

                if (IsCurrentSelection && !MenuManager.uniqueFrameInputUsed[indexAssociatedPlayerScreen]) {
                    if (indexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Up) : Input.GetKeyUp(Keys.W) || Input.GetButtonUp(Buttons.LeftThumbstickUp, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadUp, indexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Up);
                    else if (indexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Right) : Input.GetKeyUp(Keys.D) || Input.GetButtonUp(Buttons.LeftThumbstickRight, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadRight, indexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Right);
                    else if (indexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Down) : Input.GetKeyUp(Keys.S) || Input.GetButtonUp(Buttons.LeftThumbstickDown, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadDown, indexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Down);
                    else if (indexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Left) : Input.GetKeyUp(Keys.A) || Input.GetButtonUp(Buttons.LeftThumbstickLeft, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadLeft, indexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Left);
                }

                IsHovering = false;
                if (IsCurrentSelection) {
                    IsHovering = true;

                    if (!MenuManager.uniqueFrameInputUsed[indexAssociatedPlayerScreen] && (indexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Enter) : Input.GetKeyUp(Keys.Space) || Input.GetButtonUp(Buttons.A, indexAssociatedPlayerScreen))) {
                        MenuManager.uniqueFrameInputUsed[indexAssociatedPlayerScreen] = true;
                        Click?.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        public override void Draw2D(int i) {
            if (Active && i==0) {
                var colour = ImageColor;

                if (IsClicked)
                    colour = new Color(250, 203, 104);

                float rotation = 0;
                float scaleOnHovering = 1;
                if (IsHovering) {
                    if (hilightsChoice1) {
                        rotation = 5;
                        scaleOnHovering = 1.2f;   
                    }
                    else if (hilightsChoice2)
                        colour = Color.Gray;                
                }

                if (Texture != null) 
                    UserInterface.DrawPicture(Texture, new Rectangle(Rectangle.X, Rectangle.Y, (int)(Rectangle.Width * scaleOnHovering), (int)(Rectangle.Height * scaleOnHovering)), null, Align.TopLeft, Align.Center, colour, false, rotation);
            
                if (InsideImage != null)
                    UserInterface.DrawPicture(InsideImage, new Rectangle(RectangleInsideObject.X, RectangleInsideObject.Y, (int)(RectangleInsideObject.Width * scaleOnHovering), (int)(RectangleInsideObject.Height * scaleOnHovering)), null, Align.TopLeft, Align.Center, InsideObjectColor, false, rotation);
                
                if (!string.IsNullOrEmpty(Text)) {
                    //var x = (Rectangle.X + Rectangle.Width / 2) - (UserInterface.comicFont.MeasureString(Text).X / 2);
                    //var y = (Rectangle.Y + Rectangle.Height / 2) - (UserInterface.comicFont.MeasureString(Text).Y / 2);

                    UserInterface.DrawString(Text, Rectangle, Align.TopLeft, Align.Center, Align.Center, InsideObjectColor, false, font : IsHovering ? UserInterface.menuHoveringFont : UserInterface.menuFont, rot: rotation);
                }
            }
        }

        // --------------------- CUSTOM METHODS ----------------
    }
}
