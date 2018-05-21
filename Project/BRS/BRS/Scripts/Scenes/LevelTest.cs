// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
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
    class LevelTest : Scene {
        //test scene to add whatever tests and try out scene change
        public override void Load() {
            SetupCameras();
            SampleCylinders();
            //PlayerModels();
        }

        void SetupCameras() {
            foreach (Camera c in Screen.Cameras) {
                c.transform.position = new Vector3(0, 2, 8);
                c.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        void SampleCylinders() {
            GameObject plane = new GameObject("plane", File.Load<Model>("models/primitives/plane"));
            GameObject cube  = new GameObject("cube",  File.Load<Model>("models/primitives/cube"));
            GameObject cyl   = new GameObject("cyl",   File.Load<Model>("models/primitives/cylinder"));

            cyl.transform.position = new Vector3(1.5f, 2, 3);
            cyl.transform.eulerAngles = new Vector3(-10, -20, 30);
            cyl.transform.scale = new Vector3(1, .5f, .7f);
        }


        void PlayerModels() {
            Material playerMat = new Material(File.Load<Texture2D>("Images/textures/player_colors"), File.Load<Texture2D>("Images/lightmaps/elements"));
            GameObject forklift = new GameObject("forklift", File.Load<Model>("Models/vehicles/sweeper"));
            forklift.transform.eulerAngles = new Vector3(0, 180 + 90, 0);
            forklift.material = playerMat;
        }

    }
}