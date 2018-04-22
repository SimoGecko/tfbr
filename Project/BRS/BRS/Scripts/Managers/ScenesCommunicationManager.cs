using BRS.Engine;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRS.Scripts.Managers {
    public class ScenesCommunicationManager : Component {

        public Dictionary<string, Tuple<string, Model>> PlayersInfo; // playerName -> userName, Model 
        public static ScenesCommunicationManager Instance;

        public static bool loadOnlyPauseMenu;


        public override void Start() {
            Instance = this;
            PlayersInfo = new Dictionary<string, Tuple<string, Model>>();


            //if (GameManager.state == GameManager.State.Menu) loadOnlyPauseMenu = false;
            //else loadOnlyPauseMenu = true;
        }

        public override void Update() {

        }

        public void Draw() {

        }
    }
}

