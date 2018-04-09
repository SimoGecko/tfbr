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

            //simple scene to tryout change scene
            GameObject plane = new GameObject("plane1", File.Load<Model>("models/primitives/plane"));
            GameObject cube = new GameObject("cube1", File.Load<Model>("models/primitives/cube"));

            foreach (Camera c in Screen.Cameras) {
                c.transform.position = new Vector3(0, 10, 10);
                c.transform.eulerAngles = new Vector3(-45, 0, 0);
            }

        }

    }
}