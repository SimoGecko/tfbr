using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using BRS.Scripts;

namespace BRS.Menu {
    class Slider : Component {
        ////////// class to create and display a slider object (with the bar texture => see UserInterface) //////////

        // --------------------- VARIABLES ---------------------
        private MouseState currentMouse;
        private MouseState previousMouse;

        public Button buttonSlider;
        bool moveButton;
        private float offset;

        public Vector2 Position;

        // --------------------- BASE METHODS ------------------
        public Slider() {
            moveButton = false;
        }

        public override void Start() {
            base.Start();
        }

        public override void Update() {
            base.Update();

            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed && moveButton)
                moveButton = false;

            var mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);

            buttonSlider.isHovering = false;
            if (mouseRectangle.Intersects(buttonSlider.Rectangle)) {
                buttonSlider.isHovering = true;
                if (currentMouse.LeftButton == ButtonState.Pressed) {
                    moveButton = true;
                    offset = currentMouse.Position.X - buttonSlider.Position.X;
                }
            }

            if (moveButton) {
                buttonSlider.isHovering = true;
                buttonSlider.Position = new Vector2(currentMouse.Position.X - offset, buttonSlider.Position.Y);
                if (buttonSlider.Position.X + buttonSlider.texture.Width / 2 < Position.X)
                    buttonSlider.Position = new Vector2(Position.X - buttonSlider.texture.Width / 2, buttonSlider.Position.Y);
                if (buttonSlider.Position.X > Position.X + UserInterface.BARBIGWIDTH - buttonSlider.texture.Width / 2)
                    buttonSlider.Position = new Vector2(Position.X + UserInterface.BARBIGWIDTH - buttonSlider.texture.Width / 2, buttonSlider.Position.Y);
            }
        }

        public override void Draw() {
            base.Draw();

            float percentPosButon =  ((buttonSlider.Position.X - Position.X + buttonSlider.texture.Width/2) / UserInterface.BARBIGWIDTH);
            UserInterface.instance.DrawBarBig(new Vector2(Position.X, Position.Y), percentPosButon,  Color.Yellow);

            buttonSlider.Draw();
        }

        // --------------------- CUSTOM METHODS ----------------
    }
}
