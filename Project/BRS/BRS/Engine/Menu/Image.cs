// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Menu {

    /// <summary>
    /// Class to create and display an image
    /// </summary>
    class Image : Component {

        #region Properties and attributes

        /// <summary>
        /// Scale applied on the image
        /// </summary>
        public float ScaleWidth { get; set; } = 1;
        public float ScaleHeight { get; set; } = 1;

        /// <summary>
        /// Visual Aspect of the content of the image
        /// </summary>
        public Color colour = Color.White;

        /// <summary>
        /// Positon of the image
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Content of the image
        /// </summary>
        private readonly Texture2D _texture;

        /// <summary>
        /// Name to identify the image
        /// </summary>
        public string NameIdentifier { get; set; }

        /// <summary>
        /// Image texture area
        /// </summary>
        public Rectangle Rectangle {
            get {
                return new Rectangle((int)(Position.X / 1920f * Screen.Width), (int)(Position.Y / 1080f * Screen.Height), (int)(_texture.Width * ScaleWidth / 1920f * Screen.Width), (int)(_texture.Height * ScaleHeight / 1080f * Screen.Height));
            }
        }

        #endregion

        #region Constructor

        public Image (Texture2D t) {
            _texture = t;
            Active = true;
        }

        #endregion

        #region Monogame-methods
        public override void Draw2D(int i) {
            if (Active && i==0) {
                base.Draw2D(i);
                UserInterface.DrawPicture(_texture, Rectangle, null, Align.TopLeft, Align.Center, colour, false);
            }
        }
        #endregion
    }
}
