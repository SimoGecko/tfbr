// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using BRS.Scripts.Managers;

namespace BRS.Engine.Menu {

    /// <summary>
    /// Class to create and display a button with arbitrary function when clicked
    /// </summary>
    public class Button : MenuComponent {

        #region Properties and attributes

        /// <summary>
        /// Store the actions to perform when pressed
        /// </summary>
        public EventHandler Click;

        /// <summary>
        /// Positon of the button
        /// </summary>
        public Vector2 InitPos;
        public Vector2 Position { get { return InitPos * new Vector2(Screen.Width / 1920f, Screen.Height / 1080f); } }

        /// <summary>
        /// Content of the button
        /// </summary>
        public Texture2D Texture; // Texture for the whole button
        public Texture2D InsideImage { get; set; } // Texture of an image inside the button area
        public string Text { get; set; } // Text to be displayed inside the button area

        /// <summary>
        /// Visual Aspect of the content of the button
        /// </summary>
        public Color ImageColor = new Color(247, 239, 223); // Default color of the button texture
        public Color InsideObjectColor = new Color(144, 144, 144); // Default color of the inside texture
        public SpriteFont Font = UserInterface.menuFont; // Default font for the inside text 

        /// <summary>
        /// Scale applied on the button texture and the inside texture
        /// </summary>
        public float ScaleWidth { get; set; } = 1;
        public float ScaleHeight { get; set; } = 1;
        public float ScaleWidthInside { get; set; } = 1;
        public float ScaleHeightInside { get; set; } = 1;

        /// <summary>
        /// States of the button
        /// </summary>
        public bool IsHovering; 
        public bool IsClicked;
        public bool DeSelectOnMove = false;
        public bool HilightsChoice1 = true;
        public bool HilightsChoice2 = false;

        /// <summary>
        /// Name of the menu panel to switch to when pressed
        /// </summary>
        public string NameMenuToSwitchTo { get; set; }

        /// <summary>
        /// Index of the associate playerScreen for split screen menu
        /// </summary>
        public int IndexAssociatedPlayerScreen = 0;

        /// <summary>
        /// Index used for specific indexing in MenuManager
        /// </summary>
        public int Index { get; set; } 

        /// <summary>
        /// List of buttons where only 1 can be selected simulatenously
        /// </summary>
        public List<Button> neighbors;

        /// <summary>
        /// Button texture area
        /// </summary>
        public Rectangle Rectangle {
            get {
                return new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Texture.Width * ScaleWidth / 1920f * Screen.Width), (int)(Texture.Height * ScaleHeight / 1080f * Screen.Height));
            }
        }

        /// <summary>
        /// Inside texture area
        /// </summary>
        public Rectangle RectangleInsideObject {
            get {
                Vector2 offsetInside = new Vector2((Texture.Width - InsideImage.Width) / 2 * (1 - ScaleWidth) * (1 - ScaleWidthInside), (Texture.Height - InsideImage.Height) / 2 * (1 - ScaleHeight) * (1 - ScaleHeightInside));
                return new Rectangle((int)(Position.X + offsetInside.X), (int)(Position.Y + offsetInside.Y), (int)(InsideImage.Width * ScaleWidthInside / 1920f * Screen.Width), (int)(InsideImage.Height * ScaleHeightInside / 1080f * Screen.Height));
            }
        }

        #endregion

        #region Constructors

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

        #endregion

        #region Monogame-methods

        /// <summary>
        /// Monogame Start method
        /// </summary>
        public override void Start() {
            base.Start();
        }
        

        /// <summary>
        /// Monogame Update method
        /// </summary>
        public override void Update() {
            if (Active) {
                base.Update();

                // Check if user change the selection
                if (IsCurrentSelection && !MenuManager.uniqueFrameInputUsed[IndexAssociatedPlayerScreen]) {
                    if ( (IndexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Up) : Input.GetKeyUp(Keys.W)) 
                        || Input.GetButtonUp(Buttons.LeftThumbstickUp, IndexAssociatedPlayerScreen) 
                        || Input.GetButtonUp(Buttons.DPadUp, IndexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Up);
                    else if ((IndexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Right) : Input.GetKeyUp(Keys.D)) 
                        || Input.GetButtonUp(Buttons.LeftThumbstickRight, IndexAssociatedPlayerScreen) 
                        || Input.GetButtonUp(Buttons.DPadRight, IndexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Right);
                    else if ((IndexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Down) : Input.GetKeyUp(Keys.S))
                        || Input.GetButtonUp(Buttons.LeftThumbstickDown, IndexAssociatedPlayerScreen)
                        || Input.GetButtonUp(Buttons.DPadDown, IndexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Down);
                    else if ((IndexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Left) : Input.GetKeyUp(Keys.A))
                        || Input.GetButtonUp(Buttons.LeftThumbstickLeft, IndexAssociatedPlayerScreen) 
                        || Input.GetButtonUp(Buttons.DPadLeft, IndexAssociatedPlayerScreen))
                        UpdateSelection(Input.Stick.Left);
                }

                // If this button is selected, perform action on button pressed
                IsHovering = false;
                if (IsCurrentSelection) {
                    IsHovering = true;

                    if (!MenuManager.uniqueFrameInputUsed[IndexAssociatedPlayerScreen]) {
                        if ((IndexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Enter) : Input.GetKeyUp(Keys.Space))
                            || Input.GetButtonUp(Buttons.A, IndexAssociatedPlayerScreen)) {
                            MenuManager.uniqueFrameInputUsed[IndexAssociatedPlayerScreen] = true;
                            Click?.Invoke(this, new EventArgs());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Monogame Draw method
        /// </summary>
        public override void Draw2D(int i) {
            if (Active && i==0) {
                // Update default color/scale/rotation depending on the states of the button
                var colour = ImageColor;

                if (IsClicked)
                    colour = new Color(250, 203, 104);

                float rotation = 0;
                float scaleOnHovering = 1;
                if (IsHovering) {
                    if (HilightsChoice1) {
                        rotation = 5;
                        scaleOnHovering = 1.2f;
                        colour = new Color(110,235,150);
                    }
                    else if (HilightsChoice2)
                        colour = new Color(110, 235, 150);
                }

                // Draw the textures and text
                if (Texture != null) 
                    UserInterface.DrawPicture(Texture, new Rectangle(Rectangle.X, Rectangle.Y, (int)(Rectangle.Width * scaleOnHovering), (int)(Rectangle.Height * scaleOnHovering)), null, Align.TopLeft, Align.Center, colour, false, rotation);
            
                if (InsideImage != null)
                    UserInterface.DrawPicture(InsideImage, new Rectangle(RectangleInsideObject.X, RectangleInsideObject.Y, (int)(RectangleInsideObject.Width * scaleOnHovering), (int)(RectangleInsideObject.Height * scaleOnHovering)), null, Align.TopLeft, Align.Center, InsideObjectColor, false, rotation);
                
                if (!string.IsNullOrEmpty(Text)) 
                    UserInterface.DrawString(Text, Rectangle, Align.TopLeft, Align.Center, Align.Center, InsideObjectColor, false, font : IsHovering ? UserInterface.menuHoveringFont : Font, rot: rotation);              
            }
        }

        #endregion

        #region Custom updates

        /// <summary>
        /// Update selected button based on input
        /// </summary>
        private void UpdateSelection(Input.Stick state) {
            MenuManager.uniqueFrameInputUsed[IndexAssociatedPlayerScreen] = true;
            if (DeSelectOnMove) IsClicked = false;

            // Set selected button from 4 possible choices
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

        #endregion
    }
}
