// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BRS.Engine.Menu {

    /// <summary>
    /// Class to create and display a tickBox
    /// !!!! STILL OLD VERSION WITH MOUSE CLICK!!!
    /// </summary>
    class TickBox : MenuComponent {

        /// <summary>
        /// Store the actions to perform when pressed
        /// </summary>
        public EventHandler Click;

        public bool IsHovering;
        private readonly Texture2D _textureNotClicked;
        private readonly Texture2D _textureClicked;

        public bool IsClicked;

        //Color colour = new Color(247, 239, 223);
        Color colour = new Color(250, 203, 104);
        Color colourClicked = Color.White;

        public Vector2 Position { get; set; }

        /// <summary>
        /// Index of the associate playerScreen for split screen menu
        /// </summary>
        public int IndexAssociatedPlayerScreen = 0;

        /// <summary>
        /// Scale applied on the tickbox
        /// </summary>
        public float ScaleWidth { get; set; } = 1;
        public float ScaleHeight { get; set; } = 1;
        public float ScaleWidthClicked { get; set; } = 1;
        public float ScaleHeightClicked { get; set; } = 1;

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)(Position.X / 1920f * Screen.Width), (int)(Position.Y / 1080f * Screen.Height), (int)(_textureNotClicked.Width * ScaleWidth / 1920f * Screen.Width), (int)(_textureNotClicked.Height * ScaleHeight/ 1080f * Screen.Height));
            }
        }

        public Rectangle RectangleClicked {
            get {
                return new Rectangle((int)(Position.X / 1920f * Screen.Width), (int)(Position.Y / 1080f * Screen.Height), (int)(_textureClicked.Width * ScaleWidthClicked / 1920f * Screen.Width), (int)(_textureClicked.Height * ScaleHeightClicked / 1080f * Screen.Height));
            }
        }

        public TickBox(Vector2 pos, Texture2D tnotC, Texture2D tC) {
            Position = pos;
            _textureNotClicked = tnotC;
            _textureClicked = tC;
            IsClicked = false;
            Active = true;
        }

        public override void Update() {
            base.Update();

            // Check if user change the selection
            if (IsCurrentSelection && !MenuManager.uniqueFrameInputUsed[IndexAssociatedPlayerScreen]) {
                if ((IndexAssociatedPlayerScreen % 2 == 0 ? Input.GetKeyUp(Keys.Up) : Input.GetKeyUp(Keys.W))
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
                        IsClicked = !IsClicked;
                        Click?.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        public override void Draw2D(int i) {
            if (Active && i == 0) {
                var col = colour;

                if (IsHovering)
                    col = new Color(110, 235, 150);

                UserInterface.DrawPicture(_textureNotClicked, Rectangle, null, Align.TopLeft, Align.Center, col: col);
                if (IsClicked)
                    UserInterface.DrawPicture(_textureClicked, RectangleClicked, null, Align.TopLeft, Align.Center, col: colourClicked);

            }
        }

        #region Custom updates

        /// <summary>
        /// Update selected button based on input
        /// </summary>
        private void UpdateSelection(Input.Stick state) {
            MenuManager.uniqueFrameInputUsed[IndexAssociatedPlayerScreen] = true;

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


