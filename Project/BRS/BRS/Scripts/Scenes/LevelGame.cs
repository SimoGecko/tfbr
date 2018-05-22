// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.Particles3D;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using BRS.Engine.PostProcessing;
using BRS.Engine.Rendering;
using BRS.Scripts.Elements.Lighting;

namespace BRS.Scripts.Scenes {
    class LevelGame : Scene {
        ////////// first game level, loads all the things //////////

        private List<Vector3> _startPositions;
        private List<Vector3> _basePositions;
        private List<Vector3> _policeStartPositions;


        public override int GetNumCameras() { return GameManager.NumPlayers; }

        public override void Load() {
            LoadBlenderBakedScene();
            LoadUnityScene(); // TODO rename to loadColliders and loadDynamicObjects
            CreateSkybox();
            SetStartPositions();
            CreateManagers();
            CreatePlayers();
            CreateCameraControllers();
            CreateBases();
            CreateSpecialObjects();
            SetMenuShaderEffects();
        }


        void LoadBlenderBakedScene() {
            string level = "level" + GameManager.LvlScene;

            Material insideMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/" + level + "_inside"));
            Material outsideMat = new Material(File.Load<Texture2D>("Images/textures/polygonCity"), File.Load<Texture2D>("Images/lightmaps/" + level + "_outside"));
            Material groundMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"));

            HardwareRendering.InitializeModel(ModelType.InsideScene, File.Load<Model>("Models/scenes/" + level + "_inside"), insideMat);
            HardwareRendering.InitializeModel(ModelType.OutsideScene, File.Load<Model>("Models/scenes/" + level + "_outside"), outsideMat);
            HardwareRendering.InitializeModel(ModelType.Ground, File.Load<Model>("Models/elements/ground"), groundMat);

            GameObject insideScene = new GameObject("insideScene", ModelType.InsideScene, true);
            insideScene.tag = ObjectTag.Ground;

            GameObject outsideScene = new GameObject("outside", ModelType.OutsideScene, true);
            outsideScene.tag = ObjectTag.Ground;

            GameObject infinitePlane = new GameObject("infinitePlane", ModelType.Ground, true);
            infinitePlane.tag = ObjectTag.Ground;
            infinitePlane.transform.Scale(1000);
            infinitePlane.transform.position = new Vector3(0, -.1f, 0);
        }

        void LoadUnityScene() {
            var task1 = Task.Run(() => { File.ReadStatic("Load/UnitySceneData/export_scene_level" + GameManager.LvlScene + "_staticObjects.txt"); });
            task1.Wait();
            var task2 = Task.Run(() => { File.ReadDynamic("Load/UnitySceneData/export_scene_level" + GameManager.LvlScene + "_dynamicObjects.txt"); });
            task2.Wait();
        }

        void CreateSkybox() {
            bool useRandomSkybox = true;

            string[] skyboxTextures = { "daybreak", "midday", "evening", "sunset", "midnight", };
            string skyTexture = useRandomSkybox ? skyboxTextures[MyRandom.Range(0, 5)] : "midday";
            Material skyboxMat = new Material(File.Load<Texture2D>("Images/skyboxes/" + skyTexture));

            // Hardware instancing
            HardwareRendering.InitializeModel(ModelType.Skybox, File.Load<Model>("Models/elements/skybox"), skyboxMat);

            // Visible skybox to render normaly
            GameObject skybox = new GameObject("skybox", ModelType.Skybox, true);
            skybox.transform.Scale(2); // not more than this or it will be culled
            skybox.material = skyboxMat;
        }

        void CreateManagers() {
            GameObject UiManager = new GameObject("UImanager"); // must be before the other manager
            UiManager.AddComponent(new LenseFlareManager(new Vector3(-25f, 10f, 25f)));
            UiManager.AddComponent(new BaseUI());
            UiManager.AddComponent(new ButtonsUI());
            UiManager.AddComponent(new GameUI());
            //minimap
            UiManager.AddComponent(new MoneyUI());
            UiManager.AddComponent(new ParticleUI());
            UiManager.AddComponent(new PlayerUI());
            UiManager.AddComponent(new PowerupUI());
            UiManager.AddComponent(new RoundUI());
            UiManager.AddComponent(new SpeechUI());
            //tutorial

            GameObject ScenesCommManager = new GameObject("scenesComManager");
            ScenesCommManager.AddComponent(new ScenesCommunicationManager());
            ScenesCommunicationManager.loadOnlyPauseMenu = true;

            GameObject Manager = new GameObject("manager");
            Manager.AddComponent(new ElementManager());
            Manager.AddComponent(new RoundManager());
            Manager.AddComponent(new Heatmap());
            Manager.AddComponent(new Spawner());
            Manager.AddComponent(new Minimap());
            Manager.AddComponent(new PoliceManager(_policeStartPositions));
            Manager.AddComponent(new MenuManager()); // For pause menu only (not whole menu)
        }

        void SetStartPositions() {
            _startPositions = new List<Vector3>();
            _basePositions = new List<Vector3>();
            _policeStartPositions = new List<Vector3>();
            int overallOffset = GameManager.NumPlayers > 2 ? -1 : 0;

            for (int i = 0; i < GameManager.NumPlayers; i++) {
                int offset = i > 1 ? 2 : 0;
                int xBase = -5 + 10 * (i % 2 == 0 ? 0 : 1) + offset;
                int xPlayer = xBase + overallOffset;
                int zPolice = 10 + offset;

                _startPositions.Add(new Vector3(xPlayer, 0, 10));
                _basePositions.Add(new Vector3(xBase, 0, 10));
                _policeStartPositions.Add(new Vector3(5 * xBase, 0, zPolice));
            }
        }

        void CreatePlayers() {
            //Material playerMat = new Material(File.Load<Texture2D>("Images/textures/player_colors"), File.Load<Texture2D>("Images/lightmaps/elements"));

            for (int i = 0; i < GameManager.NumPlayers; i++) {
                Vector3 startPos = _startPositions[i];
                GameObject player = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift"));
                player.tag = ObjectTag.Player;
                player.transform.position = startPos;

                player.material = new Material(File.Load<Texture2D>("Images/textures/player_colors_p" + (i + 1).ToString()), File.Load<Texture2D>("Images/lightmaps/elements"));

                player.AddComponent(new Player(i, i % 2, startPos));
                player.AddComponent(new MovingRigidBody());
                //subcomponents
                player.AddComponent(new PlayerAttack());
                player.AddComponent(new PlayerInventory());
                player.AddComponent(new PlayerMovement());
                player.AddComponent(new PlayerParticles());
                player.AddComponent(new PlayerPowerup());
                player.AddComponent(new PlayerStamina());

                player.AddComponent(new DynamicShadow());
                player.AddComponent(new SpeechManager(i));
                player.AddComponent(new TutorialUI(i));

                // Modify player's name and model and color(choosen by user during menu)
                if (MenuManager.Instance != null) {
                    MenuManager.Instance.ChangeModelNameColorPlayer(player, i);

                    int modelIndex = 0;
                    if (ScenesCommunicationManager.Instance != null && ScenesCommunicationManager.Instance.PlayersInfo != null && ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey("player_" + i))
                        modelIndex = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item2;

                    player.AddComponent(new FrontLight(FrontLight.Type.FrontAndBack, modelIndex));
                    player.AddComponent(new AnimatedWheels(AnimatedWheels.Type.BackOnly, 5, modelIndex));
                } else {
                    player.AddComponent(new FrontLight(FrontLight.Type.FrontAndBack, 0));
                    player.AddComponent(new AnimatedWheels(AnimatedWheels.Type.BackOnly, 5, 0));
                }


                ElementManager.Instance.Add(player.GetComponent<Player>());

                // Model instanciation
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), "player" + i, true);

                player.UseHardwareInstanciation = true;
                player.ModelType = modelType;

                HardwareRendering.InitializeModel(modelType, player.Model, player.material);
                HardwareRendering.AddInstance(modelType, player);

                //ARROWS

                //arrow for base
                GameObject arrow = new GameObject("arrow_" + i, ModelType.ArrowBase, true);
                arrow.AddComponent(new Arrow(player, false, i, player.GetComponent<PlayerInventory>().IsAlmostFull));
                arrow.transform.Scale(.6f);

                //arrow for enemy
                if (GameManager.NumPlayers > 1) {
                    GameObject arrow2 = new GameObject("arrow2_" + i, ModelType.ArrowEnemy, true);
                    arrow2.AddComponent(new Arrow(player, true, i, () => true));
                    arrow2.transform.Scale(.3f);
                }
            }
        }

        void CreateCameraControllers() {
            int i = 0;
            foreach (Camera c in Screen.Cameras) {
                c.gameObject.AddComponent(new CameraController(i++)); // TODO move out this creation code
            }
        }

        void CreateBases() {
            for (int i = 0; i < GameManager.NumTeams; i++) {
                // Load the texture and create a copy for the colored base
                Texture2D baseTexture = File.Load<Texture2D>("Images/textures/base");
                Color teamColor = (i == 0) ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor;
                Texture2D coloredBase = Graphics.TextureTint(baseTexture, teamColor);

                GameObject playerBase = new GameObject("base_" + i.ToString(), File.Load<Model>("Models/primitives/plane"));
                playerBase.tag = ObjectTag.Base;
                playerBase.transform.Scale(0.5f);
                playerBase.transform.position = _basePositions[i] + 0.001f * Vector3.Up;
                playerBase.material = new Material(coloredBase, true);
                playerBase.AddComponent(new Base(i));
                playerBase.AddComponent(new BaseParticles());
                playerBase.AddComponent(new StaticRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
                playerBase.AddComponent(new BaseParticles());
                ElementManager.Instance.Add(playerBase.GetComponent<Base>());

                // Model instanciation
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), "base" + i, true);

                playerBase.UseHardwareInstanciation = true;
                playerBase.ModelType = modelType;

                HardwareRendering.InitializeModel(modelType, playerBase.Model, playerBase.material);
                HardwareRendering.AddInstance(modelType, playerBase);
            }
        }

        void CreateSpecialObjects() {
            //VAULT
            GameObject vault = new GameObject("vault", ModelType.Vault, true);
            vault.transform.position = new Vector3(1.2f, 1.39f, -64.5f);
            vault.DrawOrder = 1;

            vault.AddComponent(new Vault());
            vault.AddComponent(new AnimatedRigidBody());
            vault.AddComponent(new Smoke());
            vault.AddComponent(new FlyingCash());

            // Model instanciation

            //other elements
            GameObject speedpad = GameObject.Instantiate("speedpadPrefab", new Vector3(0, 0, -18), Quaternion.Identity);
            speedpad.transform.eulerAngles = Vector3.Up * 90;
        }

        void SetMenuShaderEffects() {
            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.Vignette, i, true);
                PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.ColorGrading, i, true);
                //PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.Chromatic, i, true); // why is this disabled?
                PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.TwoPassBlur, i, false);
            }

        }
    }
}