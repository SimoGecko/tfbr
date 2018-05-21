using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BRS.Engine {
    public class SpriteSheet {
        Texture2D texture;
        int rows, columns;
        int width, height;

        private int totalFrames;

        public SpriteSheet(Texture2D _texture, int _rows, int _columns) {
            texture = _texture;
            rows = _rows;
            columns = _columns;
            totalFrames = rows * columns;
            width = texture.Width / columns;
            height = texture.Height / rows;
        }

        public SpriteSheet(Texture2D _texture, int size) { // builds assuming every sprite in atlas square and multiple of size
            texture = _texture;
            width = size;
            height = size;
            rows = texture.Height/size;
            columns = texture.Width / size;
            totalFrames = rows * columns;
        }

        public void Draw(Vector2 location, int currentFrame, Color tint, float scale=1) {
            int row = currentFrame / columns;
            int column = currentFrame % columns;
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            UserInterface.DrawPicture(texture, location, sourceRectangle, Align.TopLeft, Align.Center, col:tint, scale: scale);
        }

        public bool FrameEnded(int frame) {
            return frame > totalFrames;
        }
    }
}