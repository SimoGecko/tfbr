﻿using BRS.Engine;
using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Menu {
    class Image : Component {
        private readonly Texture2D _texture;
        public Vector2 Position { get; set; }
        public string NameIdentifier { get; set; }

        public Rectangle Rectangle {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public Image (Texture2D t) {
            _texture = t;
            Active = true;
        }

        public override void Draw(int i) {
            if (Active) {
                base.Draw(i);
                //UserInterface.DrawPictureOLD(Rectangle, _texture);
                UserInterface.DrawPicture(_texture, Rectangle, null, Align.TopLeft, Align.Center, Color.White, false);
            }
        }
    }
}
