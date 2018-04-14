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
            //GameObject cube = new GameObject("cube1", File.Load<Model>("models/primitives/cube"));
            GameObject cyl = new GameObject("cube1", File.Load<Model>("models/primitives/cylinder"));
            cyl.transform.position = new Vector3(1.5f, 2, 3);
            cyl.transform.eulerAngles = new Vector3(-10, -20, 30);
            cyl.transform.scale = new Vector3(1, .5f, .7f);

            foreach (Camera c in Screen.Cameras) {
                c.transform.position = new Vector3(0, 10, 10);
                c.transform.eulerAngles = new Vector3(-45, 0, 0);
            }

        }

    }
}