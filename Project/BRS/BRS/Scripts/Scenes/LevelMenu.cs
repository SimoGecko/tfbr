// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

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
            Material insideMat  = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/menu_inside"));
            Material outsideMat = new Material(File.Load<Texture2D>("Images/textures/polygonCity"), File.Load<Texture2D>("Images/lightmaps/menu_outside"));
            Material groundMat  = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"));

            // Model instanciation -> not wokring with these models, but not to bad since we have not many models in the menu-level
            HardwareRendering.InitializeModel(ModelType.InsideScene, File.Load<Model>("Models/scenes/menu_inside"), insideMat);
            HardwareRendering.InitializeModel(ModelType.OutsideScene, File.Load<Model>("Models/scenes/menu_outside"), outsideMat);
            HardwareRendering.InitializeModel(ModelType.Ground, File.Load<Model>("Models/elements/ground"), groundMat);

            GameObject insideScene = new GameObject("menu_inside", ModelType.InsideScene, true);
            insideScene.tag = ObjectTag.Ground;

            GameObject outsideScene = new GameObject("menu_outside", ModelType.OutsideScene, true);
            outsideScene.tag = ObjectTag.Ground;

            GameObject infinitePlane = new GameObject("infinitePlane", ModelType.Ground, true);
            infinitePlane.tag = ObjectTag.Ground;
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
                //PostProcessingManager.Instance.SetShaderStatus(PostprocessingType.ColorGrading, i, true); // why disabled?
            }
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

        /// <summary>
        /// Create the needed managers for the menu
        /// </summary>
        private void CreateManagers() {
            // !! Has to be called before the next manager
            // Store the information needed from for multiple scene => don't get destroyed
            GameObject scenesCommManager = new GameObject("scenesComManager");
            scenesCommManager.AddComponent(new ScenesCommunicationManager());
            ScenesCommunicationManager.loadOnlyPauseMenu = false;

            // Define the menu
            GameObject manager = new GameObject("manager");
            manager.AddComponent(new ButtonsUI());
            manager.AddComponent(new MenuManager());
        }

        #endregion

    }
}
