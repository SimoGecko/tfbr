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

        public void ReadFile(string pathName) {
            using (StreamReader reader = new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                string nameContent;
                while ((nameContent = reader.ReadLine()) != null) {
                    while (nameContent == "")
                        nameContent = reader.ReadLine();
                    if (nameContent == null)
                        break;

                    string tagName = reader.ReadLine().Split(' ')[1];
                    string prefabName = reader.ReadLine().Split(' ')[1];
      
                    string lineNoObj = reader.ReadLine();
                    int n = int.Parse(lineNoObj.Split(' ')[1]);

                    for (int i = 0; i < n; i++) {
                        string p = reader.ReadLine();
                        string r = reader.ReadLine();
                        string s = reader.ReadLine();

                        string[] pSplit = p.Split(' '); // pos: x y z in unity coord. system
                        string[] rSplit = r.Split(' '); // rot: x y z in unity coord. system
                        string[] sSplit = s.Split(' '); // sca: x y z in unity coord. system

                        Vector3 position = new Vector3(float.Parse(pSplit[3]), float.Parse(pSplit[2]), float.Parse(pSplit[1]));
                        Quaternion rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(float.Parse(pSplit[2])), MathHelper.ToRadians(float.Parse(pSplit[1])), MathHelper.ToRadians(float.Parse(pSplit[3])));
                        Vector3 scale = new Vector3(float.Parse(sSplit[3]), float.Parse(sSplit[2]), float.Parse(sSplit[1]));

                        GameObject go =  new GameObject(tagName + "_" + i.ToString(), Content.Load<Model>(prefabName));
                        
                        go.Transform.position = position;
                        go.Transform.scale = scale;
                        //go.transform.rotation = rotation; // rotation not parsed correctly
                        
                        if (tagName == "Ground")
                            go.Type = ObjectType.Ground;
                        else if (tagName == "Base")
                            go.Type = ObjectType.Base;
                        else if (tagName == "Obstacle")
                            go.Type = ObjectType.Obstacle;
                        else if (tagName == "Boundary")
                            go.Type = ObjectType.Boundary;
                    }

                    nameContent = reader.ReadLine();
                }
            }
        }

        protected override void Build() {
            ////////// scene setup for level1 //////////

            //MANAGER
            GameObject manager = new GameObject("manager");
            //manager.AddComponent(new CameraController());
            manager.AddComponent(new GameManager());
            manager.AddComponent(new Spawner());
            manager.AddComponent(new Minimap());
            manager.AddComponent(new GamepadTest());


            //TRANSFORM TEST
            GameObject testCube = new GameObject("testcube", Content.Load<Model>("cube"));
            testCube.AddComponent(new TransformTest());


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
                forklift.myTag = "player";
                forklift.AddComponent(new Player());
                forklift.GetComponent<Player>().playerIndex = i;
                forklift.GetComponent<Player>().teamIndex = i%2;

                forklift.Transform.position = new Vector3(-5 + 10 * i, 0, 0);

                forklift.AddComponent(new SphereCollider(Vector3.Zero, .7f));
                //subcomponents
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());
                forklift.AddComponent(new PlayerPowerup());

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
                    bases[i].myTag = "base";
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

            GameObject[] bases = GameObject.FindGameObjectsWithTag(ObjectType.Base);
            for (int i = 0; i < GameManager.numPlayers; i++) {
                bases[i].AddComponent(new Base(i));
                bases[i].AddComponent(new BoxCollider(bases[i]));
                bases[i].Transform.SetStatic();
            }
        }
    }
}