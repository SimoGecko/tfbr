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

            foreach (Camera c in Screen.Cameras) {
                c.transform.position = new Vector3(0, 10, 12);
                c.transform.eulerAngles = new Vector3(-35, 0, 0);
            }
            PostProcessingManager.Instance._effects[3].Active = true;

            CreateManagers();

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
