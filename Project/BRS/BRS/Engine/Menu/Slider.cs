// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.UI;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BRS.Engine.Menu {
    public class Slider : MenuComponent {
        ////////// class to create and display a slider object (with the bar texture => see UserInterface) //////////

        // --------------------- VARIABLES ---------------------

        public Button ButtonSlider;
        public float percentPosButon;

        public Vector2 Position;

        public Texture2D Texture;
        public EventHandler OnReleaseSlider;
        private float lengthSlider = 300f;

        public int indexAssociatedPlayerScreen = 0;

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)(Position.X / 1920f * Screen.Width), (int)(Position.Y / 1080f * Screen.Height), (int)(lengthSlider / 1920f * Screen.Width), (int)((ButtonSlider.Texture.Height - 25) * ButtonSlider.ScaleHeight/ 1080f * Screen.Height));
            }
        }

        // --------------------- BASE METHODS ------------------
        public Slider(Vector2 pos, Texture2D t) {
            IsCurrentSelection = false;

            Position = pos;

            ButtonSlider = new Button(t, new Vector2(pos.X + lengthSlider - t.Width * 0.5f / 2, pos.Y));
            ButtonSlider.ScaleHeight = 0.5f;
            ButtonSlider.ScaleWidth = 0.5f;
            ButtonSlider.hilightsChoice1 = false;
            ButtonSlider.hilightsChoice2 = true;
            Active = true;
        }

        private void UpdateSelection(Input.Stick state) {
            MenuManager.uniqueFrameInputUsed[indexAssociatedPlayerScreen] = true;
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
            base.Update();

            if (IsCurrentSelection) {
                ButtonSlider.IsHovering = true;
                if (!MenuManager.uniqueFrameInputUsed[indexAssociatedPlayerScreen] && (Input.GetKey(Keys.Enter) || Input.GetButton(Buttons.A, indexAssociatedPlayerScreen))) {
                    ButtonSlider.IsHovering = true;

                    if (Input.GetKey(Keys.Right) || Input.GetButton(Buttons.LeftThumbstickRight, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadRight, indexAssociatedPlayerScreen)) {
                        ButtonSlider.InitPos = ButtonSlider.InitPos + new Vector2(2, 0);
                    }
                    else if (Input.GetKey(Keys.Left) || Input.GetButton(Buttons.LeftThumbstickLeft, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadLeft, indexAssociatedPlayerScreen)) {
                        ButtonSlider.InitPos = ButtonSlider.InitPos - new Vector2(2, 0);
                    }

                    if (ButtonSlider.InitPos.X - ButtonSlider.Texture.Width * ButtonSlider.ScaleWidth / 2 < Position.X)
                        ButtonSlider.InitPos = new Vector2(Position.X + ButtonSlider.Texture.Width * ButtonSlider.ScaleWidth / 2, ButtonSlider.InitPos.Y);
                    if (ButtonSlider.InitPos.X + ButtonSlider.Texture.Width * ButtonSlider.ScaleWidth / 2> Position.X + lengthSlider /*/ 1920f * Screen.Width */)
                        ButtonSlider.InitPos = new Vector2(Position.X + lengthSlider /*/ 1920f * Screen.Width*/ - ButtonSlider.Texture.Width * ButtonSlider.ScaleWidth / 2, ButtonSlider.InitPos.Y);
                }    
                else {
                    if (IsCurrentSelection && !MenuManager.uniqueFrameInputUsed[indexAssociatedPlayerScreen]) {
                        if (Input.GetKeyUp(Keys.Up) || Input.GetButtonUp(Buttons.LeftThumbstickUp, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadUp, indexAssociatedPlayerScreen))
                            UpdateSelection(Input.Stick.Up);
                        else if (Input.GetKeyUp(Keys.Right) || Input.GetButtonUp(Buttons.LeftThumbstickRight, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadRight, indexAssociatedPlayerScreen))
                            UpdateSelection(Input.Stick.Right);
                        else if (Input.GetKeyUp(Keys.Down) || Input.GetButtonUp(Buttons.LeftThumbstickDown, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadDown, indexAssociatedPlayerScreen))
                            UpdateSelection(Input.Stick.Down);
                        else if (Input.GetKeyUp(Keys.Left) || Input.GetButtonUp(Buttons.LeftThumbstickLeft, indexAssociatedPlayerScreen) || Input.GetButtonUp(Buttons.DPadLeft, indexAssociatedPlayerScreen))
                            UpdateSelection(Input.Stick.Left);
                    }
                }

                if (Input.GetKeyUp(Keys.Enter) || Input.GetButtonUp(Buttons.A, indexAssociatedPlayerScreen)) {
                    OnReleaseSlider?.Invoke(this, new EventArgs());
                }
            }
            else
                ButtonSlider.IsHovering = false;
        }

        public override void Draw2D(int i) {

            if (Active && i == 0) {
                base.Draw2D(i);

                Rectangle dest = Rectangle;

                // normal 
                UserInterface.DrawPicture(Texture, dest, null, Align.TopLeft, pivot: Align.Left, col: Color.LightGray);

                // percent relative to button
                percentPosButon = ((ButtonSlider.InitPos.X - Position.X) / (lengthSlider));
                dest.Width = (int)Math.Round(dest.Width * percentPosButon);
                UserInterface.DrawPicture(Texture, dest, null, Align.TopLeft, pivot: Align.Left, col: Color.MediumBlue);

                ButtonSlider.Draw2D(i);
            }
        }

        // --------------------- CUSTOM METHODS ----------------
    }
}
