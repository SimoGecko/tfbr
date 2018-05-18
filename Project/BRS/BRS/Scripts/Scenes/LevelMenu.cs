using BRS.Engine;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework.Graphics;
using BRS.Engine.PostProcessing;
using BRS.Engine.Rendering;
using Microsoft.Xna.Framework;
using BRS.Scripts.UI;

namespace BRS.Scripts.Scenes {

    /// <summary>
    /// Define the menu as level to facilitate scene changes
    /// </summary>
    class LevelMenu : Scene {

        #region Monogame-methods

        /// <summary>
        /// Monogame Load method
        /// </summary>
        public override void Load() {
            MenuScene();
            SetMenuShaderEffects();
            CreateManagers();
            CreateSkybox();
            //Audio.PlayRandomSong();
        }

        #endregion

        #region Custom methods

        /// <summary>
        /// Load the menu scene
        /// </summary>
        void MenuScene() {
            Material insideMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/menu_inside"));
            Material outsideMat = new Material(File.Load<Texture2D>("Images/textures/polygonCity"), File.Load<Texture2D>("Images/lightmaps/menu_outside"));
            Material groundMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"));

            // Model instanciation
            HardwareRendering.InitializeModel(ModelType.InsideScene, File.Load<Model>("Models/scenes/menu_inside"), insideMat);
            HardwareRendering.InitializeModel(ModelType.OutsideScene, File.Load<Model>("Models/scenes/menu_outside"), outsideMat);
            HardwareRendering.InitializeModel(ModelType.Ground, File.Load<Model>("Models/elements/ground"), groundMat);


            GameObject insideScene = new GameObject("menu_inside", File.Load<Model>("Models/scenes/menu_inside"));
            insideScene.DrawOrder = 1;
            insideScene.material = insideMat;
            insideScene.tag = ObjectTag.Ground;

            GameObject outsideScene = new GameObject("menu_outside", File.Load<Model>("Models/scenes/menu_outside"));
            outsideScene.DrawOrder = 2;
            outsideScene.material = outsideMat;
            outsideScene.tag = ObjectTag.Ground;

            GameObject infinitePlane = new GameObject("infinitePlane", ModelType.Ground, true);
            infinitePlane.DrawOrder = 0;
            infinitePlane.material = groundMat;
            infinitePlane.transform.Scale(1000);
            infinitePlane.transform.position = new Vector3(0, -5.0f, 0);

        }

        /// <summary>
        /// Set the shaders effect for the menu
        /// </summary>
        private void SetMenuShaderEffects() {
            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.Vignette, i, true);
                PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.TwoPassBlur, i, true);
                //PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.ColorGrading, i, true);
            }
        }

        void CreateSkybox() {
            bool useRandomSkybox = false;
            string[] skyboxTextures = new string[] { "daybreak", "midday", "evening", "sunset", "midnight", };
            string skyTexture = useRandomSkybox ? skyboxTextures[MyRandom.Range(0, 5)] : "midday";
            GameObject skybox = new GameObject("skybox", File.Load<Model>("Models/elements/skybox"));
            skybox.transform.Scale(2); // not more than this or it will be culled
            Material skyboxMat = new Material(File.Load<Texture2D>("Images/skyboxes/" + skyTexture));
            skybox.material = skyboxMat;
        }

        /// <summary>
        /// Create the needed managers for the menu
        /// </summary>
        private void CreateManagers() {
            // !! Has to be called before the next manager
            // Store the information needed from for multiple scene => don't get destroyed
            GameObject ScenesCommManager = new GameObject("scenesComManager");
            ScenesCommManager.AddComponent(new ScenesCommunicationManager());
            ScenesCommunicationManager.loadOnlyPauseMenu = false;

            // Define the menu
            GameObject Manager = new GameObject("manager");
            Manager.AddComponent(new Suggestions());
            Manager.AddComponent(new MenuManager());
            
        }

        #endregion

    }
}
