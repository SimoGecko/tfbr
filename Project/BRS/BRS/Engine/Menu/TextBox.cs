using BRS.Engine;
using BRS.Scripts;
using Microsoft.Xna.Framework;

namespace BRS.Menu {
    class TextBox : Component {
        public string Text { get; set; }
        public string NameIdentifier { get; set; }

        public Vector2 InitPos { get; set; }
        public Vector2 Position { get { return InitPos - UserInterface.Instance.comicFont.MeasureString(Text) / 2; } }

        public TextBox() {
            Active = true;
        }

        public override void Draw() {
            base.Draw();
            UserInterface.Instance.DrawStringOLD(Position, Text, Color.Black);
        }
    }
}
