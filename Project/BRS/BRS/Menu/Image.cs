﻿using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Menu {
    class Image : Component {
        private readonly Texture2D _texture;
        public Vector2 Position { get; set; }

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public Image (Texture2D t) {
            _texture = t;
        }

        public override void Draw() {
            base.Draw();
            UserInterface.Instance.DrawPicture(Rectangle, _texture);
        }
    }
}
