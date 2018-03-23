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

                        GameObject go =  new GameObject(tagName + "_" + i.ToString(), Content.Load<Model>("Models/primitives/" + prefabName));
                        
                        go.transform.position = position;
                        go.transform.scale = scale;
                        //go.transform.rotation = rotation; // rotation not parsed correctly

                        if (tagName == "Ground")
                            go.Type = ObjectType.Ground;
                        else if (tagName == "Base") {
                            go.Type = ObjectType.Base;
                            go.myTag = "base";
                        }
                        else if (tagName == "Obstacle")
                            go.Type = ObjectType.Obstacle;
                        else if (tagName == "Boundary")
                            go.Type = ObjectType.Boundary;
                        else if (tagName == "VaultDoor")
                            go.myTag = "VaultDoor";
                    }

                    nameContent = reader.ReadLine();
                }
            }
        }

        protected override void Build() {
            ////////// scene setup for level1 //////////

            //MANAGER
            GameObject manager = new GameObject("manager");
            manager.AddComponent(new Elements());
            manager.AddComponent(new GameManager());
            manager.AddComponent(new Spawner());
            manager.AddComponent(new Minimap());
            //manager.AddComponent(new GamepadTest());


            //TRANSFORM TEST
            GameObject testCube = new GameObject("testcube", Content.Load<Model>("Models/primitives/cube"));
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
                GameObject forklift = new GameObject("player_"+i.ToString(), Content.Load<Model>("Models/vehicles/forklift"));
                forklift.Type = ObjectType.Player;
                forklift.myTag = "player";
                forklift.AddComponent(new Player(i, i%2));

                forklift.transform.position = new Vector3(-5 + 10 * i, 0, 0);

                forklift.AddComponent(new SphereCollider(Vector3.Zero, .7f));
                //subcomponents
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());
                forklift.AddComponent(new PlayerPowerup());
                forklift.AddComponent(new PlayerStamina());
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

            //VAULT
            GameObject vault = new GameObject("vault", Content.Load<Model>("Models/primitives/cylinder"));
            vault.AddComponent(new Vault());
            vault.transform.position = new Vector3(5 , 1.5f, -62);
            vault.transform.scale = new Vector3(3, .5f, 3);
            vault.transform.eulerAngles = new Vector3(90, 0, 0);
            vault.AddComponent(new SphereCollider(Vector3.Zero, 3f));

            //LOAD UNITY SCENE

            var task = Task.Run(() => {
                ReadFile("Load/UnitySceneData/lvl" + GameManager.lvlScene.ToString() + "/ObjectSceneUnity.txt");
            });
            task.Wait();

            GameObject[] bases = GameObject.FindGameObjectsByType(ObjectType.Base);
            Debug.Assert(bases.Length == 2, "there should be 2 bases");
            for (int i = 0; i < bases.Length; i++) {
                bases[i].AddComponent(new Base(i));
                //bases[i].AddComponent(new BoxCollider(bases[i]));
                bases[i].AddComponent(new BoxCollider(Vector3.Zero, Vector3.One*3));
                bases[i].transform.SetStatic();
            }
        }
    }
}