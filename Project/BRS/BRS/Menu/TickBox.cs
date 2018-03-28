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
    class TickBox : Component {
        ////////// class to create and display a tickBox //////////

        // --------------------- VARIABLES ---------------------

        private MouseState currentMouse;
        private MouseState previousMouse;

        private bool isHovering;
        private Texture2D textureNotClicked;
        private Texture2D textureClicked;

        public bool isClicked;

        public Vector2 Position { get; set; }

        public Rectangle Rectangle {
            get {
                if (isClicked)
                    return new Rectangle((int)Position.X, (int)Position.Y, textureClicked.Width, textureClicked.Height);
                else
                    return new Rectangle((int)Position.X, (int)Position.Y, textureNotClicked.Width, textureNotClicked.Height);
            }
        }

        // --------------------- BASE METHODS ------------------
        public TickBox(Texture2D tnotC, Texture2D tC) {
            textureNotClicked = tnotC;
            textureClicked = tC;
            isClicked = false;
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
                    isClicked = !isClicked;
                }
            }
        }

        // --------------------- CUSTOM METHODS ----------------
        public override void Draw() {
            var colour = Color.White;

            if (isHovering)
                colour = Color.Gray;

            if (isClicked)
                UserInterface.instance.DrawPicture(Rectangle, textureClicked, colour);
            else
                UserInterface.instance.DrawPicture(Rectangle, textureNotClicked, colour);
        }
    }
}


