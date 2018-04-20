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

            GameObject Manager = new GameObject("manager");
            Manager.AddComponent(new ScenesCommunicationManager());
            Manager.AddComponent(new MenuManager());

        }

    }
}
