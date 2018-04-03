// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts;
using System.Threading.Tasks;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Physics.Vehicle;
using Jitter.LinearMath;

namespace BRS.Load {
    class Level1 : Scene {
        private readonly Game1 _game;

        // TODO: game-parameter will be removed as soon as the car is rewritten to our framework
        public Level1(PhysicsManager physics, Game1 game)
            : base(physics) {
            _game = game;
        }



        /// <summary>
        /// Scene setup for level1
        /// </summary>
        protected override void Build() {
            //MANAGER
            GameObject UIManager = new GameObject("UImanager"); // must be before the other manager
            UIManager.AddComponent(new BaseUI());
            UIManager.AddComponent(new PlayerUI());
            UIManager.AddComponent(new PowerupUI());
            UIManager.AddComponent(new GameUI());

            GameObject manager = new GameObject("manager");
            manager.AddComponent(new Elements());
            manager.AddComponent(new GameManager());
            manager.AddComponent(new RoundManager());
            manager.AddComponent(new Spawner());
            manager.AddComponent(new Minimap());


            //TEST lighting
            GameObject monkeyScene = new GameObject("monkeyScene", File.Load<Model>("Models/test/plant"));
            monkeyScene.transform.Scale(3);
            monkeyScene.transform.position += Vector3.Up * .1f;

            GameObject monkeyScene2 = new GameObject("monkeyScene2", File.Load<Model>("Models/test/plant"));
            monkeyScene2.transform.Scale(3);
            monkeyScene2.transform.position += Vector3.Up * .1f + Vector3.Right * 2;


            //GROUND
            /*for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 3; y++) {
                    GameObject groundPlane = new GameObject("groundplane_" + x.ToString() + "_" + y.ToString(), File.Load<Model>("gplane"));
                    groundPlane.transform.position = new Vector3(x * 10-5, 0, -y * 10);
                    groundPlane.transform.SetStatic();
                }
            }*/


            //PLAYER
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject player = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift_tex")); // for some reason the tex is much less shiny
                player.tag = ObjectTag.Player;
                player.AddComponent(new Player(i, i % 2));
                player.transform.position = new Vector3(-5 + 10 * i, 0, 1);
                //player.AddComponent(new SphereCollider(Vector3.Zero, .7f));
                //subcomponents
                player.AddComponent(new PlayerMovement());
                player.AddComponent(new PlayerAttack());
                player.AddComponent(new PlayerInventory());
                player.AddComponent(new PlayerPowerup());
                player.AddComponent(new PlayerStamina());
                player.AddComponent(new PlayerLift());
                player.AddComponent(new MovingRigidBody(PhysicsManager));

                //arrow
                GameObject arrow = new GameObject("arrow_" + i, File.Load<Model>("Models/elements/arrow"));
                arrow.AddComponent(new Arrow(player.transform, null, i));
                arrow.transform.Scale(.1f);
                //player.mat = new EffectMaterial(true, Color.White);

            }



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
            vault.AddComponent(new StaticRigidBody(PhysicsManager));
            //vault.AddComponent(new SphereCollider(Vector3.Zero, 3f));

            //other elements
            GameObject.Instantiate("speedpadPrefab", Vector3.Zero, Quaternion.Identity);

            //LOAD UNITY SCENE
            var task = Task.Run(() => { File.ReadFile("Load/UnitySceneData/lvl" + GameManager.lvlScene + "/ObjectSceneUnity.txt", PhysicsManager); });
            task.Wait();

            var task2 = Task.Run(() => { File.ReadHeistScene("Load/UnitySceneData/export1.txt"); });
            task2.Wait();

            GameObject[] bases = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            //Debug.Assert(bases.Length == 2, "there should be 2 bases");
            for (int i = 0; i < bases.Length; i++) {
                bases[i].AddComponent(new Base(i));
                bases[i].AddComponent(new StaticRigidBody(PhysicsManager));
                //bases[i].AddComponent(new BoxCollider(Vector3.Zero, Vector3.One * 3));
                bases[i].transform.SetStatic();
            }

            //var _car = new CarObject(_game, PhysicsManager);
            //_game.Components.Add(_car);

            //_car.carBody.Position = new JVector(-2, 0.8f, -2);

            //// Dummy object at position (0/0/0) for debug-rendering.
            //GameObject dummy = new GameObject("dummy_object", File.Load<Model>("Models/primitives/cube"));
            //dummy.tag = ObjectTag.Default;
            //dummy.AddComponent(new StaticRigidBody(PhysicsManager, tag: BodyTag.DrawMe));
        }
    }
}