using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRS.Engine.Menu {
    public class MenuComponent : Component {
        public MenuComponent NeighborUp { get; set; }
        public MenuComponent NeighborDown { get; set; }
        public MenuComponent NeighborLeft { get; set; }
        public MenuComponent NeighborRight { get; set; }

        public bool IsCurrentSelection;
        public string nameIdentifier;

        public MenuComponent() {
            IsCurrentSelection = false;
        }
    }
}
