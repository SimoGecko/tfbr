// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Utilities;
using BRS.Scripts;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.Particles3D;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace BRS.Scripts.Scenes {
    class Level1 : Scene {
        ////////// first game level, loads all the things //////////

        public List<Vector3> StartPositions;


        public override int GetNumCameras() { return GameManager.NumPlayers; }

        public override void Load() {
            LoadBlenderBakedScene();
            LoadUnityScene();
            SetStartPositions();
            CreateManagers();
            CreatePlayers();
            CreateCameraControllers();
            CreateBases();
            CreateSpecialObjects();
        }


        void LoadBlenderBakedScene() {
            Material insideMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/lightmapInside"));
            GameObject insideScene = new GameObject("insideScene", File.Load<Model>("Models/scenes/inside"));
            insideScene.material = insideMat;

            Material outsideMat = new Material(File.Load<Texture2D>("Images/textures/polygonCity"), File.Load<Texture2D>("Images/lightmaps/lightmapOutside"));
            GameObject outsideScene = new GameObject("outside", File.Load<Model>("Models/scenes/outside"));
            outsideScene.material = outsideMat;
        }

        void LoadUnityScene() {
            var task1 = Task.Run(() => { File.ReadStatic("Load/UnitySceneData/export_scene_level" + GameManager.LvlScene + "_staticObjects.txt"); });
            task1.Wait();
            var task2 = Task.Run(() => { File.ReadDynamic("Load/UnitySceneData/export_scene_level" + GameManager.LvlScene + "_dynamicObjects.txt"); });
            task2.Wait();
        }

        void SetStartPositions() {
            StartPositions = new List<Vector3>();
            for (int i = 0; i < GameManager.NumPlayers; i++) {
                int offset = i > 1 ? 1 : 0;
                StartPositions.Add(new Vector3(-5 + 10 * (i % 2 == 0 ? 0 : 1) + offset, 0, 10));
            }
        }

        void CreateManagers() {
            GameObject UiManager = new GameObject("UImanager"); // must be before the other manager
            UiManager.AddComponent(new BaseUI());
            UiManager.AddComponent(new PlayerUI());
            UiManager.AddComponent(new PowerupUI());
            UiManager.AddComponent(new GameUI());
            UiManager.AddComponent(new Suggestions());
            UiManager.AddComponent(new MoneyUI());
            UiManager.AddComponent(new ParticleUI());
            UiManager.AddComponent(new SpeechUI());
            UiManager.AddComponent(new RoundUI());

            GameObject Manager = new GameObject("manager");
            Manager.AddComponent(new ElementManager());
            //Manager.AddComponent(new GameManager());
            Manager.AddComponent(new RoundManager());
            Manager.AddComponent(new Heatmap());
            Manager.AddComponent(new Spawner());
            Manager.AddComponent(new Minimap());
            Manager.AddComponent(new AudioTest());
            Manager.AddComponent(new PoliceManager());


            Manager.AddComponent(new MenuManager()); // For pause menu only (not whole menu)
            ScenesCommunicationManager.loadOnlyPauseMenu = true;
            //Add(Manager);         

            //new MenuManager().LoadContent(); // TODO add as component to manager
        }

        void CreatePlayers() {

            //Material playerMat = new Material(File.Load<Texture2D>("Images/textures/player_colors"), File.Load<Texture2D>("Images/lightmaps/elements"));

            for (int i = 0; i < GameManager.NumPlayers; i++) {
                Vector3 startPos = StartPositions[i];
                GameObject player = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift"));
                player.tag = ObjectTag.Player;
                player.transform.position = startPos;
                player.transform.Scale(1.0f);

                player.material = new Material(File.Load<Texture2D>("Images/textures/player_colors_p" + (i+1).ToString()), File.Load<Texture2D>("Images/lightmaps/elements"));

                player.AddComponent(new Player(i, i % 2, startPos));
                player.AddComponent(new MovingRigidBody());
                //subcomponents
                player.AddComponent(new PlayerMovement());
                player.AddComponent(new PlayerAttack());
                player.AddComponent(new PlayerInventory());
                player.AddComponent(new PlayerPowerup());
                player.AddComponent(new PlayerStamina());
                player.AddComponent(new PlayerLift());
                player.AddComponent(new PlayerCollider());
                player.AddComponent(new PlayerParticles());
                player.AddComponent(new SpeechManager(i));

                // Modify player's name and model and color(choosen by user during menu)
                if (MenuManager.Instance != null)
                    MenuManager.Instance.ChangeModelNameColorPlayer(player, i);


                //Add(player);
                ElementManager.Instance.Add(player.GetComponent<Player>());

                //arrow for base
                //TODO add correct materials
                GameObject arrow = new GameObject("arrow_" + i, File.Load<Model>("Models/elements/arrow"));
                arrow.material = new Material(Graphics.Green);
                arrow.AddComponent(new Arrow(player, false, i, player.GetComponent<PlayerInventory>().IsFull));
                arrow.transform.Scale(.2f);

                //arrow for enemy
                GameObject arrow2 = new GameObject("arrow2_" + i, File.Load<Model>("Models/elements/arrow"));
                arrow2.material = new Material(Graphics.Red);
                arrow2.AddComponent(new Arrow(player, true, i, () => true));
                arrow2.transform.Scale(.08f);
            }
        }

        void CreateCameraControllers() {
            int i = 0;
            foreach (Camera c in Screen.Cameras) {
                GameObject camObject = c.gameObject;
                camObject.AddComponent(new CameraController()); // TODO move out this creation code
                camObject.GetComponent<CameraController>().CamIndex = i++;
            }
        }

        void CreateBases() {
            for (int i = 0; i < GameManager.NumTeams; i++) {
                //TODO base object
                GameObject playerBase = new GameObject("base_" + i.ToString(), File.Load<Model>("Models/primitives/cube"));
                playerBase.tag = ObjectTag.Base;
                playerBase.AddComponent(new Base(i));
                playerBase.transform.Scale(2);
                playerBase.transform.position = StartPositions[i];
                playerBase.transform.scale = new Vector3(3, 1, 1);
                playerBase.transform.SetStatic();
                //playerBase.AddComponent(new BoxCollider(playerBase));
                playerBase.AddComponent(new StaticRigidBody(pureCollider: true));
                playerBase.transform.SetStatic();
                ElementManager.Instance.Add(playerBase.GetComponent<Base>());
            }


        }

        void CreateSpecialObjects() {
            Material playerMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/elements"));
            //VAULT
            GameObject vault = new GameObject("vault", File.Load<Model>("Models/elements/vault"));
            vault.AddComponent(new Vault());
            //vault.transform.position = new Vector3(5, 1.5f, -62);
            //vault.transform.scale = new Vector3(3, .5f, 3);
            //vault.transform.eulerAngles = new Vector3(90, 0, 0);

            vault.transform.position = new Vector3(1.2f, 1.39f, -64.5f);
            vault.AddComponent(new AnimatedRigidBody());
            vault.AddComponent(new Smoke());
            vault.material = playerMat;
            //vault.AddComponent(new SphereCollider(Vector3.Zero, 3f));
            //Add(vault);


            //other elements
            GameObject speedpad = GameObject.Instantiate("speedpadPrefab", new Vector3(0, 0, -20), Quaternion.Identity);
            //Add(speedpad);
        }
    }
}