// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Menu {
    class TextBox : Component {
        public string Text { get; set; }
        public string NameIdentifier { get; set; }

        public Vector2 InitPos { get; set; }
        public Vector2 Position { get { return InitPos * new Vector2(Screen.Width / 1920f, Screen.Height / 1080f) - UserInterface.menuFont.MeasureString(Text) / 2; } }

        public TextBox() {
            Active = true;
        }

        public override void Draw(int i) {
            base.Draw(i);
            UserInterface.DrawString(Text, Position,col: Color.Black, font : UserInterface.menuFont);
        }
    }
}
