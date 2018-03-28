using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Menu {
    class Button : Component {
        // --------------------- VARIABLES ---------------------

        private MouseState currentMouse;
        private MouseState previousMouse;

        private SpriteFont font;
        private bool isHovering;
        public Texture2D texture;

        public EventHandler Click;

        public Color PenColor { get; set; }
        public Vector2 Position { get; set; }

        public string NameMenuToSwitchTo { get; set; }

        public int index { get; set; } //unique

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            }
        }

        public string Text { get; set; }


        // --------------------- BASE METHODS ------------------
        public Button(Texture2D t, SpriteFont sf) {
            texture = t;
            font = sf;
            PenColor = Color.Black;
        }


        public override void Start() {
            base.Start();
        }

        public override void Update() {
            base.Update();

            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);

            isHovering = false;
            if (mouseRectangle.Intersects(Rectangle)) {
                isHovering = true;
                if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed) {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        // --------------------- CUSTOM METHODS ----------------
        public override void Draw(SpriteBatch spriteBatch) {
            var Colour = Color.White;

            if (isHovering)
                Colour = Color.Gray;
            if (texture != null)
                spriteBatch.Draw(texture, new Vector2(Position.X, Position.Y), Color.White);

            if (!string.IsNullOrEmpty(Text)) {
                var x = (Rectangle.X + Rectangle.Width / 2) - (font.MeasureString(Text).X/2);
                var y = (Rectangle.Y + Rectangle.Height / 2) - (font.MeasureString(Text).Y / 2);
                                
                spriteBatch.DrawString(font, Text, new Vector2(x,y), PenColor);
                
            }
        }

    }
}
