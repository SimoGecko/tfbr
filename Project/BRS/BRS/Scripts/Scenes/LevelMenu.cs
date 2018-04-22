using BRS.Engine;
using BRS.Scripts.UI;
using BRS.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRS.Scripts.Scenes {
    class LevelMenu : Scene {

        public override void Load() {
            CreateManagers();
        }

        private void CreateManagers() {

            // @Simone make sure this doesnt get deleted
            // Has to be called before the next manager
            GameObject ScenesCommManager = new GameObject("scenesComManager");
            ScenesCommManager.AddComponent(new ScenesCommunicationManager());
            ScenesCommunicationManager.loadOnlyPauseMenu = false;

            GameObject Manager = new GameObject("manager");
            Manager.AddComponent(new MenuManager());

        }

    }
}
