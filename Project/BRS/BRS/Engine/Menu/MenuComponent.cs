
namespace BRS.Engine.Menu {
    /// <summary>
    /// Class to a component of the menu which can selected
    /// </summary>
    public class MenuComponent : Component {

        #region Properties and attributes

        /// <summary>
        /// The 4 possible components that can be selected from this component
        /// </summary>
        public MenuComponent NeighborUp { get; set; }
        public MenuComponent NeighborDown { get; set; }
        public MenuComponent NeighborLeft { get; set; }
        public MenuComponent NeighborRight { get; set; }

        /// <summary>
        /// Name to identify the component
        /// </summary>
        public bool IsCurrentSelection;

        /// <summary>
        /// Name to identify the component
        /// </summary>
        public string nameIdentifier;

        /// <summary>
        /// Index of the associate playerScreen for split screen menu
        /// </summary>
        public int IndexAssociatedPlayerScreen = 0;

        #endregion

        #region Constructor

        public MenuComponent() {
            IsCurrentSelection = false;
        }
        #endregion
    }
}
