// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Menu;
using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;
using BRS.Engine;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;

namespace BRS.Scripts.Scenes {
    class Level2 : Scene {

        public override void Load() {
            foreach (Camera c in Screen.Cameras) {
                c.transform.position = new Vector3(0, 2, 8);
                c.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            //SampleCylinders();
            //MenuScene();
            PlayerModels();
        }

        void SampleCylinders() {
            //simple scene to tryout change scene
            GameObject plane = new GameObject("plane1", File.Load<Model>("models/primitives/plane"));
            GameObject cyl = new GameObject("cube1", File.Load<Model>("models/primitives/cylinder"));
            cyl.material = new Material(Color.Red, false);

            GameObject cube = new GameObject("cube1", File.Load<Model>("models/primitives/cube"));
            cube.material = new Material(Color.Green);
            cube.AddComponent(new MaterialTest(cube.material));

            cyl.transform.position = new Vector3(1.5f, 2, 3);
            cyl.transform.eulerAngles = new Vector3(-10, -20, 30);
            cyl.transform.scale = new Vector3(1, .5f, .7f);
            
        }

        void MenuScene() {
            Material insideMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/menu_inside"));
            GameObject insideScene = new GameObject("menu_inside", File.Load<Model>("Models/scenes/menu_inside"));
            insideScene.material = insideMat;

            Material outsideMat = new Material(File.Load<Texture2D>("Images/textures/polygonCity"), File.Load<Texture2D>("Images/lightmaps/menu_outside"));
            GameObject outsideScene = new GameObject("menu_outside", File.Load<Model>("Models/scenes/menu_outside"));
            outsideScene.material = outsideMat;
        }

        void PlayerModels() {
            Material playerMat = new Material(File.Load<Texture2D>("Images/textures/player_colors"), File.Load<Texture2D>("Images/lightmaps/elements"));
            GameObject forklift = new GameObject("forklift", File.Load<Model>("Models/vehicles/sweeper"));
            forklift.transform.eulerAngles = new Vector3(0, 180 + 90, 0);
            forklift.material = playerMat;
        }

    }
}