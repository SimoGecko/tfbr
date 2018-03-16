// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts;
using System.IO;
using System.Threading.Tasks;
using BRS.Engine.Physics;

namespace BRS.Load {
    class Level1 : Scene {
        public Level1(PhysicsManager physics)
            : base(physics) { }

        public List<GameObject> ReadFile(string pathName, string prefabToUse, string nameObj)
        {
            List<GameObject> objects = new List<GameObject>();
            using (StreamReader reader = new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string firstLine = reader.ReadLine();
                int n = int.Parse(firstLine);

                for (int i = 0; i < n; i++)
                {
                    string p = reader.ReadLine();
                    string r = reader.ReadLine();
                    string s = reader.ReadLine();

                    string[] pSplit = p.Split(' ');
                    string[] rSplit = r.Split(' ');
                    string[] sSplit = s.Split(' ');

                    Vector3 position = new Vector3(float.Parse(pSplit[2]), float.Parse(pSplit[1]), float.Parse(pSplit[0]));
                    Quaternion rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(float.Parse(pSplit[1])), MathHelper.ToRadians(float.Parse(pSplit[0])), MathHelper.ToRadians(float.Parse(pSplit[2])));
                    Vector3 scale = new Vector3(float.Parse(sSplit[2]), float.Parse(sSplit[1]), float.Parse(sSplit[0]));

                    GameObject playerBase = new GameObject(nameObj + "_" + i.ToString(), Content.Load<Model>(prefabToUse));
                    playerBase.Transform.position = position;
                    playerBase.Transform.scale = scale;
                    //playerBase.transform.rotation = rotation; // rotation not parsed correctly

                    objects.Add(playerBase);
                }
                return objects;
            }
        }

        protected override void Build() {
            ////////// scene setup for level1 //////////

            //MANAGER
            GameObject manager = new GameObject("manager");
            //manager.AddComponent(new CameraController());
            manager.AddComponent(new GameManager());
            manager.AddComponent(new Spawner());


            //TRANSFORM TEST
            //GameObject testCube = new GameObject("testcube", Content.Load<Model>("cube"));
            //testCube.AddComponent(new TransformTest());


            //GROUND
            /*for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 3; y++) {
                    GameObject groundPlane = new GameObject("groundplane_" + x.ToString() + "_" + y.ToString(), Content.Load<Model>("gplane"));
                    groundPlane.transform.position = new Vector3(x * 10-5, 0, -y * 10);
                    groundPlane.transform.SetStatic();
                }
            }*/


            //PLAYER
            for(int i=0; i<GameManager.numPlayers; i++) {
                GameObject forklift = new GameObject("player_"+i.ToString(), Content.Load<Model>("forklift"));
                forklift.Type = ObjectType.Player;
                forklift.AddComponent(new Player(i));
                forklift.GetComponent<Player>().playerIndex = i;
                forklift.GetComponent<Player>().teamIndex = i%2;

                forklift.Transform.position = new Vector3(-5 + 10 * i, 0, 0);

                forklift.AddComponent(new SphereCollider(Vector3.Zero, .7f));
                //subcomponents
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());

            }


            //BASE
            /*for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_"+i.ToString(), Content.Load<Model>("cube"));
                playerBase.tag = "base";
                playerBase.AddComponent(new Base());
                playerBase.GetComponent<Base>().baseIndex = i;
                playerBase.transform.position = new Vector3(-5 + 10 * i, 0, 1);
                playerBase.transform.scale = new Vector3(3, 1, 1);
                playerBase.transform.SetStatic();
                playerBase.AddComponent(new BoxCollider(playerBase));

            }*/


            //LOAD UNITY SCENE
            var task = Task.Run(() =>
            {
                //GROUND
                List<GameObject> groundPlane = ReadFile("Load/UnityScenes/lvl" + GameManager.lvlScene.ToString() + "/Ground.txt", "gplane", "groundplane");

                //BASES
                List<GameObject> bases = ReadFile("Load/UnityScenes/lvl" + GameManager.lvlScene.ToString() + "/Bases.txt", "cube", "playerBase");

                for (int i = 0; i < GameManager.numPlayers; i++)
                {
                    bases[i].Type = ObjectType.Base;
                    bases[i].AddComponent(new Base(i));
                    //bases[i].GetComponent<Base>().player = GameObject.FindGameObjectWithName("player_" + i).GetComponent<Player>();
                    bases[i].AddComponent(new BoxCollider(bases[i]));
                    bases[i].Transform.SetStatic();
                }

                //OBSTACLES
                List<GameObject> obstacles = ReadFile("Load/UnityScenes/lvl" + GameManager.lvlScene.ToString() + "/Obstacles.txt", "cube", "obstacle");
                foreach (GameObject go in obstacles)
                    go.Type = ObjectType.Obstacle;

                //BOUNDARIES
                List<GameObject> boundaries = ReadFile("Load/UnityScenes/lvl" + GameManager.lvlScene.ToString() + "/Boundaries.txt", "cube", "boundary");
                foreach (GameObject go in boundaries)
                    go.Type = ObjectType.Boundary;
            });
            task.Wait();
        }
    }

}