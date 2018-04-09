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
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;

namespace BRS.Scripts.Scenes {
    class Level1 : Scene {
        /*
        public Level1(PhysicsManager physics)
            : base(physics) {
        }*/




        /// <summary>
        /// Scene setup for level1
        /// </summary>
        public override void Load() {

            //MANAGERS
            GameObject UiManager = new GameObject("UImanager"); // must be before the other manager
            UiManager.AddComponent(new BaseUI());
            UiManager.AddComponent(new PlayerUI());
            UiManager.AddComponent(new PowerupUI());
            UiManager.AddComponent(new GameUI());
            UiManager.AddComponent(new Suggestions());
            UiManager.AddComponent(new MoneyUI());
            Add(UiManager);

            GameObject Manager = new GameObject("manager");
            Manager.AddComponent(new ElementManager());
            Manager.AddComponent(new GameManager());
            Manager.AddComponent(new RoundManager());
            Manager.AddComponent(new Heatmap());
            Manager.AddComponent(new Spawner());
            Manager.AddComponent(new Minimap());
            Manager.AddComponent(new AudioTest());
            Add(Manager);


            //UiManager.Start();
            //Manager.Start();



            //TEST lighting
            /*
            GameObject monkeyScene = new GameObject("monkeyScene", File.Load<Model>("Models/test/plant"));
            monkeyScene.transform.Scale(3);
            monkeyScene.transform.position += Vector3.Up * .1f;

            GameObject monkeyScene2 = new GameObject("monkeyScene2", File.Load<Model>("Models/test/plant"));
            monkeyScene2.transform.Scale(3);
            monkeyScene2.transform.position += Vector3.Up * .1f + Vector3.Right*2;
            */

            //GROUND
            /*for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 3; y++) {
                    GameObject groundPlane = new GameObject("groundplane_" + x.ToString() + "_" + y.ToString(), File.Load<Model>("gplane"));
                    groundPlane.transform.position = new Vector3(x * 10-5, 0, -y * 10);
                    groundPlane.transform.SetStatic();
                }
            }*/

            //BASE // TODO have this code make the base
            /*for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_"+i.ToString(), File.Load<Model>("cube"));
                playerBase.tag = "base";
                playerBase.AddComponent(new Base());
                playerBase.GetComponent<Base>().baseIndex = i;
                playerBase.transform.position = new Vector3(-5 + 10 * i, 0, 1);
                playerBase.transform.scale = new Vector3(3, 1, 1);
                playerBase.transform.SetStatic();
                playerBase.AddComponent(new BoxCollider(playerBase));

            }*/

            //VAULT
            GameObject vault = new GameObject("vault", File.Load<Model>("Models/primitives/cylinder"));
            vault.AddComponent(new Vault());
            vault.transform.position = new Vector3(5, 1.5f, -62);
            vault.transform.scale = new Vector3(3, .5f, 3);
            vault.transform.eulerAngles = new Vector3(90, 0, 0);
            vault.AddComponent(new StaticRigidBody(PhysicsManager.Instance));
            //vault.AddComponent(new SphereCollider(Vector3.Zero, 3f));
            Add(vault);

            // Todo: refactor
            vault.Start();

            //other elements
            GameObject speedpad = GameObject.Instantiate("speedpadPrefab", new Vector3(0, 0, -20), Quaternion.Identity);
            Add(speedpad);

            //LOAD UNITY SCENE
            //var task = Task.Run(() => { File.ReadFile("Load/UnitySceneData/lvl" + GameManager.lvlScene.ToString() + "/ObjectSceneUnity.txt"); });
            var task = Task.Run(() => { File.ReadFile("Load/UnitySceneData/ObjectSceneUnity.txt", PhysicsManager.Instance); });
            task.Wait();

            var task2 = Task.Run(() => { File.ReadHeistScene("Load/UnitySceneData/export1.txt"); });
            task2.Wait();


            //CREATE CAMERA CONTROLLERS
            int i = 0;
            foreach(Camera c in Screen.Cameras) {
                GameObject camObject = c.gameObject;
                Add(camObject);
                camObject.AddComponent(new CameraController()); // TODO move out this creation code
                camObject.GetComponent<CameraController>().CamIndex = i++;
            }
            

            CreatePlayers();
        }

        void CreatePlayers() {
            List<GameObject> objects = new List<GameObject>();

            for (int i = 0; i < GameManager.NumPlayers; i++) {
                GameObject player = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift_tex")); // for some reason the tex is much less shiny
                player.tag = ObjectTag.Player;
                player.transform.Scale(1.0f);
                Vector3 startPos =  new Vector3(-5 + 10 * i, 0, 0);

                player.AddComponent(new Player(i, i % 2, startPos, "Player " +(i+1)));
                player.AddComponent(new MovingRigidBody(PhysicsManager.Instance));
                //subcomponents
                player.AddComponent(new PlayerMovement());
                player.AddComponent(new PlayerAttack());
                player.AddComponent(new PlayerInventory());
                player.AddComponent(new PlayerPowerup());
                player.AddComponent(new PlayerStamina());
                player.AddComponent(new PlayerLift());

                Add(player);

                /*
                if (MenuManager.Instance.PlayersInfo.ContainsKey("player_" + i)) {
                    string userName = MenuManager.Instance.PlayersInfo["player_" + i].Item1;
                    Model userModel = MenuManager.Instance.PlayersInfo["player_" + i].Item2;

                    if (userName != null) player.GetComponent<Player>().PlayerName = userName;
                    if (userModel != null) player.Model = userModel;
                }*/

                ElementManager.Instance.Add(player.GetComponent<Player>());

                //arrow
                GameObject arrow = new GameObject("arrow_" + i, File.Load<Model>("Models/elements/arrow"));
                arrow.AddComponent(new Arrow(player, null, i));
                arrow.transform.Scale(.1f);

                Add(arrow);


                objects.Add(player); //why???
                objects.Add(arrow);
            }

            GameObject[] bases = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            Debug.Assert(bases.Length == 2, "there should be 2 bases");
            for (int i = 0; i < bases.Length; i++) {
                bases[i].AddComponent(new Base(i));
                bases[i].transform.Scale(2);
                bases[i].AddComponent(new StaticRigidBody(PhysicsManager.Instance, pureCollider: true));
                //bases[i].AddComponent(new BoxCollider(Vector3.Zero, Vector3.One * 3));
                bases[i].transform.SetStatic();
                ElementManager.Instance.Add(bases[i].GetComponent<Base>());

                Add(bases[i]);
                objects.Add(bases[i]);
            }

            // todo: (andy) is this necessary here? // SIMO surely NOT!!
            foreach (GameObject go in objects) {
                //go.Start();
            }

        }
    }
}