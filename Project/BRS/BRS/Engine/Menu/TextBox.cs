// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Menu {
    class TextBox : Component {
        public string Text { get; set; }
        public string NameIdentifier { get; set; }

        public Vector2 InitPos { get; set; }
        public Vector2 Position { get { return InitPos * new Vector2(Screen.Width / 1920f, Screen.Height / 1080f); } }

        public SpriteFont Font = UserInterface.menuFont;

        public TextBox() {
            Active = true;
        }

        public override void Draw2D(int i) {
            if (Active && i == 0) {
                base.Draw2D(i);
                UserInterface.DrawString(Text, Position, pivot: Align.Center, col: Color.Black, font: UserInterface.menuSmallFont);
            }
        }
    }
}
