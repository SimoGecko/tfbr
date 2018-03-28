using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BRS.Menu {
    class Slider : Component {

        private MouseState currentMouse;
        private MouseState previousMouse;

        public Button buttonSlider;
        bool moveButton;
        private float offset;

        Texture2D textureBar;
        public const int BARWIDTH = 256; public const int BARHEIGHT = 16;

        public Vector2 Position;
        private bool isHovering;

        public Slider(Texture2D bar) {
            textureBar = bar;
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

            isHovering = false;
            if (mouseRectangle.Intersects(buttonSlider.Rectangle)) {
                isHovering = true;
                if (currentMouse.LeftButton == ButtonState.Pressed) {
                    moveButton = true;
                    offset = currentMouse.Position.X - buttonSlider.Position.X;
                }
            }

            if (moveButton) {
                buttonSlider.Position = new Vector2(currentMouse.Position.X - offset, buttonSlider.Position.Y);
                if (buttonSlider.Position.X + buttonSlider.texture.Width / 2 < Position.X)
                    buttonSlider.Position = new Vector2(Position.X - buttonSlider.texture.Width / 2, buttonSlider.Position.Y);
                if (buttonSlider.Position.X > Position.X + BARWIDTH - buttonSlider.texture.Width / 2)
                    buttonSlider.Position = new Vector2(Position.X + BARWIDTH - buttonSlider.texture.Width / 2, buttonSlider.Position.Y);
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);

            var Colour = Color.White;

            if (isHovering)
                Colour = Color.Gray;

            Rectangle fgrect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
            Rectangle bgrect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);

            float percentPosButon =  ((buttonSlider.Position.X - Position.X + buttonSlider.texture.Width/2) / BARWIDTH);
            fgrect.Width = (BARWIDTH * (int)(percentPosButon*100) / 100);

            spriteBatch.Draw(textureBar, new Vector2(Position.X, Position.Y), bgrect, Color.White);
            spriteBatch.Draw(textureBar, new Vector2(Position.X, Position.Y), fgrect, Color.Yellow);

            buttonSlider.Draw(spriteBatch);
        }
        
    }
}
