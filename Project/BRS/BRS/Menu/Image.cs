using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Menu {
    class Image : Component {
        Texture2D texture;
        public Vector2 Position { get; set; }

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            }
        }

        public Image (Texture2D t) {
            texture = t;
        }

        public override void Draw() {
            base.Draw();
            UserInterface.instance.DrawPicture(Rectangle, texture);
        }
    }
}
