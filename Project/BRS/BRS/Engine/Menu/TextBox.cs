// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Menu {

    /// <summary>
    /// Class to create and display a text
    /// </summary>
    class TextBox : Component {

        #region Properties and attributes

        /// <summary>
        /// Positon of the textbox
        /// </summary>
        public Vector2 InitPos { get; set; }
        private Vector2 offset = new Vector2(0, 5f * Screen.Height / 1080f);
        public Vector2 Position { get { return InitPos * new Vector2(Screen.Width / 1920f, Screen.Height / 1080f) + offset; } }

        /// Content of the textbox
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Visual Aspect of the content of the textbox
        /// </summary>
        public SpriteFont Font = UserInterface.menuFont;
        public Color Colour = Color.Black;

        /// <summary>
        /// Name to identify the textbox
        /// </summary>
        public string NameIdentifier { get; set; }

        #endregion

        #region Constructor

        public TextBox() {
            Active = true;
            
        }

        #endregion

        #region Monogame-methods

        public override void Draw2D(int i) {
            if (Active && i == 0) {
                base.Draw2D(i);
                UserInterface.DrawString(Text, Position, pivot: Align.Center, col: Colour, font: Font);
            }
        }

        #endregion
    }
}
