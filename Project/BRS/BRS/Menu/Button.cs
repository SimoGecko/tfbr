using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BRS.Scripts;

namespace BRS.Menu {
    class Button : Component {
        ////////// class to create and display a button with arbitrary function when clicked //////////

        // --------------------- VARIABLES ---------------------

        private MouseState currentMouse;
        private MouseState previousMouse;

        public bool isHovering;
        public Texture2D texture;

        public EventHandler Click;
        public Vector2 Position { get; set; }
        public string NameMenuToSwitchTo { get; set; }
        public string Text { get; set; }

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            }
        }

        // --------------------- BASE METHODS ------------------
        public Button(Texture2D t) {
            texture = t;
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

        public override void Draw() {
            var colour = Color.White;
            if (isHovering)
                colour = Color.Gray;

            if (texture != null)
                UserInterface.instance.DrawPicture(Rectangle, texture, colour);

            if (!string.IsNullOrEmpty(Text)) {
                var x = (Rectangle.X + Rectangle.Width / 2) - (UserInterface.instance.smallFont.MeasureString(Text).X/2);
                var y = (Rectangle.Y + Rectangle.Height / 2) - (UserInterface.instance.smallFont.MeasureString(Text).Y / 2);
                                
                UserInterface.instance.DrawString(new Vector2(x,y), Text, Color.Black);   
            }
        }

        // --------------------- CUSTOM METHODS ----------------
    }
}
