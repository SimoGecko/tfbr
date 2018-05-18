// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BRS.Scripts.Managers {

    /// <summary>
    /// Store information need from the levels but set through the menu that are not destroyed when changing scene
    /// </summary>
    public class ScenesCommunicationManager : Component {

        #region Properties and attributes

        /// <summary>
        /// Instance of ScenesCommunicationManager
        /// </summary>
        public static ScenesCommunicationManager Instance;

        /// <summary>
        /// Store if the whole menu or only the pause menu sould be loaded
        /// </summary>
        public static bool loadOnlyPauseMenu;

        /// <summary>
        /// Store each player's name, model and color chose in the menu
        /// </summary>
        public Dictionary<string, Tuple<string, int, Color>> PlayersInfo; // playerName -> userName, Model, Color

        /// <summary>
        /// Store the possible models (meshes + UI images)
        /// </summary>
        public List<Model> ModelCharacter;
        public List<Texture2D> ModelImages;
        public List<Texture2D> ModelImagesColorPart;

        /// <summary>
        /// Store the model texture for each player
        /// </summary>
        public Dictionary<string, Texture2D> textureColorPlayers;

        /// <summary>
        /// Store the color of each team
        /// </summary>
        public static Color[] ColorModel = { Graphics.Yellow, Graphics.Green, Graphics.Blue, Graphics.Purple, Graphics.Pink, Graphics.Red, Graphics.Orange };
        public static Color TeamAColor = Graphics.Yellow;
        public static Color TeamBColor = Graphics.Green;

        /// <summary>
        /// Structure that store the statistic of a model
        /// </summary>
        public struct ModelsStatsStruct {
            public int Capacity;
            public float AttackDistance;
            public float MaxSpeed, MinSpeed;

            public ModelsStatsStruct(int cap, float att, float maxSp, float minSp) {
                Capacity = cap;
                AttackDistance = att;
                MaxSpeed = maxSp;
                MinSpeed = minSp;
            }
        }

        /// <summary>
        /// Define the possible statistic of each model
        /// </summary>
        public static ModelsStatsStruct maxModelStats = new ModelsStatsStruct(27, 7, 7, 3);
        public static ModelsStatsStruct[] ValuesStats = { new ModelsStatsStruct(18, 4.667f, 7, 3), new ModelsStatsStruct(27, 4.667f, 4.667f, 2), new ModelsStatsStruct(18, 7, 4.667f, 2) };
        public static string[] NameStats = { "Capacity", "Distance of attack", "Speed" };

        /// <summary>
        /// Define the possible game modes name
        /// </summary>
        public static string[] ModesName = { "default", "bomber", "crateonly", "survival" };
        public static string[] ModesDescription = { "Normal", "Bomber", "Crate Only", "Gold Only" };

        #endregion

        #region Monogame-methods

        /// <summary>
        /// Monogame Start function
        /// </summary>
        public override void Start() {
            Instance = this;

            PlayersInfo = new Dictionary<string, Tuple<string, int, Color>>();

            // Load models
            ModelCharacter = new List<Model> {
                File.Load<Model>("Models/vehicles/forklift"),
                File.Load<Model>("Models/vehicles/sweeper"),
                File.Load<Model>("Models/vehicles/bulldozer")
            };

            // Load player's textures
            textureColorPlayers = new Dictionary<string, Texture2D>();
            Texture2D player0Color = File.Load<Texture2D>("Images/textures/player_colors_p1");
            Texture2D player1Color = File.Load<Texture2D>("Images/textures/player_colors_p2");
            Texture2D player2Color = File.Load<Texture2D>("Images/textures/player_colors_p3");
            Texture2D player3Color = File.Load<Texture2D>("Images/textures/player_colors_p4");

            // Set default player's color
            textureColorPlayers.Add("player_0", player0Color);
            textureColorPlayers.Add("player_1", player1Color);
            textureColorPlayers.Add("player_2", player2Color);
            textureColorPlayers.Add("player_3", player3Color);

            ModelImages = new List<Texture2D>();
            ModelImagesColorPart = new List<Texture2D>();

            ModelImages.Add(File.Load<Texture2D>("Images/vehicles_menu_pics/fl_back"));
            ModelImages.Add(File.Load<Texture2D>("Images/vehicles_menu_pics/sw_back"));
            ModelImages.Add(File.Load<Texture2D>("Images/vehicles_menu_pics/bz_back"));
            ModelImagesColorPart.Add(File.Load<Texture2D>("Images/vehicles_menu_pics/fl_color"));
            ModelImagesColorPart.Add(File.Load<Texture2D>("Images/vehicles_menu_pics/sw_color"));
            ModelImagesColorPart.Add(File.Load<Texture2D>("Images/vehicles_menu_pics/bz_color"));
        }

        /// <summary>
        /// Monogame Update function
        /// </summary>
        public override void Update() {

        }

        /// <summary>
        /// Monogame Draw function
        /// </summary>
        public void Draw() {

        }

        #endregion Monogame-methods

    }
}

