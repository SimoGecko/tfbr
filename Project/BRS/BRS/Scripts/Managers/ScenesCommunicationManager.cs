using BRS.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace BRS.Scripts.Managers {
    public class ScenesCommunicationManager : Component {

        public Dictionary<string, Tuple<string, int, Color>> PlayersInfo; // playerName -> userName, Model, Color
        public static ScenesCommunicationManager Instance;

        public static bool loadOnlyPauseMenu;

        public Dictionary<string, Texture2D> textureColorPlayers;

        public static ModelsStatsStruct maxModelStats = new ModelsStatsStruct(27, 7, 6, 4);
        public static ModelsStatsStruct[] ValuesStats = { new ModelsStatsStruct(18, 4.667f, 6, 4), new ModelsStatsStruct(27, 4.667f, 4, 2.667f), new ModelsStatsStruct(18, 7, 4, 2.667f) };
        public static string[] NameStats = { "Capacity", "Distance of attack", "Speed (min-max)" };

        public static Color[] ColorModel = { new Color(215, 173, 35), Graphics.Red, Graphics.Green, Graphics.Blue, Graphics.Yellow, Color.Violet };
        public static Color TeamAColor = new Color(215, 173, 35);
        public static Color TeamBColor = Graphics.Red;

        public static string[] ModesName = { "default", "bomber", "crateonly", "survival" };
        public static string[] ModesDescription = { "Normal", "Bomber", "Crate Only", "Gold Only" };

        public List<Model> ModelCharacter;

        public override void Start() {
            Instance = this;
            PlayersInfo = new Dictionary<string, Tuple<string, int, Color>>();

            ModelCharacter = new List<Model> {
                File.Load<Model>("Models/vehicles/forklift"),
                File.Load<Model>("Models/vehicles/sweeper"),
                File.Load<Model>("Models/vehicles/bulldozer")
            };

            textureColorPlayers = new Dictionary<string, Texture2D>();
            Texture2D player0Color = File.Load<Texture2D>("Images/textures/player_colors_p1");
            Texture2D player1Color = File.Load<Texture2D>("Images/textures/player_colors_p2");
            Texture2D player2Color = File.Load<Texture2D>("Images/textures/player_colors_p3");
            Texture2D player3Color = File.Load<Texture2D>("Images/textures/player_colors_p4");

            textureColorPlayers.Add("player_0", player0Color);
            textureColorPlayers.Add("player_1", player1Color);
            textureColorPlayers.Add("player_2", player2Color);
            textureColorPlayers.Add("player_3", player3Color);

            Color test = GetPixelColor(5, 10, player0Color);
        }

        public override void Update() {

        }

        public void Draw() {

        }

        public Color GetPixelColor(int x, int y, Texture2D texture) {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colorData);
            return colorData[x + y * texture.Width];
        }

        public void SetPixelColor(int x, int y, Color color, Texture2D texture) {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colorData);
            colorData[x + y * texture.Width] = color;
            texture.SetData<Color>(colorData);
        }

        public void SetRectanglePixelColor(Rectangle rec, Color color, Texture2D texture) {
            for (int x = rec.X; x < rec.X + rec.Width; ++x)
                for (int y = rec.Y; y < rec.Y + rec.Height; ++y)
                    SetPixelColor(x, y, color, texture);
        }

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
    }
}

