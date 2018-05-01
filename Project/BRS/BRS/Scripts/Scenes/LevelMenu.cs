using BRS.Engine;
using BRS.Scripts.UI;
using BRS.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BRS.Engine.PostProcessing;

namespace BRS.Scripts.Scenes {
    class LevelMenu : Scene {

        public override void Load() {
           
            MenuScene();

            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                PostProcessingManager.Instance._effects[2].Activate(i, true);
                PostProcessingManager.Instance._effects[3].Activate(i, true);
            }

            CreateManagers();

            // huge ground
            GameObject plane = new GameObject("plane1", File.Load<Model>("models/primitives/plane"));
            plane.material = new Material(new Color(35,35,35));
            plane.transform.position = new Vector3(0,-0.5f,0);
            plane.transform.scale = new Vector3(1000,1,1000);
            

            // @ Simone
            //List<List<Vector3>> policePaths = File.ReadPolicePaths("Load/PolicePaths.txt");
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

        void MenuScene() {
            Material insideMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/menu_inside"));
            GameObject insideScene = new GameObject("menu_inside", File.Load<Model>("Models/scenes/menu_inside"));
            insideScene.material = insideMat;

            Material outsideMat = new Material(File.Load<Texture2D>("Images/textures/polygonCity"), File.Load<Texture2D>("Images/lightmaps/menu_outside"));
            GameObject outsideScene = new GameObject("menu_outside", File.Load<Model>("Models/scenes/menu_outside"));
            outsideScene.material = outsideMat;
        }

    }
}
