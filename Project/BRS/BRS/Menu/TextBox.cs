using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRS.Scripts;
using Microsoft.Xna.Framework;

namespace BRS.Menu {
    class TextBox : Component {
        public string Text { get; set; }

        public Vector2 InitPos { get; set; }
        public Vector2 Position { get { return InitPos - UserInterface.instance.smallFont.MeasureString(Text) / 2; } }

        public override void Draw() {
            base.Draw();
            UserInterface.instance.DrawString(Position, Text, Color.Black);
        }
    }
}
