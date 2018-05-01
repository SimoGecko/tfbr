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

        public Dictionary<string, Tuple<string, Model, Color>> PlayersInfo; // playerName -> userName, Model 
        public static ScenesCommunicationManager Instance;

        public static bool loadOnlyPauseMenu;

        public Dictionary<string, Texture2D> textureColorPlayers;

        public static ModelsStatsStruct[] ValuesStats = { new ModelsStatsStruct(5, 5, 5), new ModelsStatsStruct(10, 10, 10), new ModelsStatsStruct(15, 15, 15) };
        public static string[] NameStats = { "Capacity", "AttackDistance", "Speed" };

        public static Color[] ColorModel = { new Color(215, 173, 35), Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Violet };
        public static Color TeamAColor = new Color(215, 173, 35);
        public static Color TeamBColor = Color.Red;

        public override void Start() {
            Instance = this;
            PlayersInfo = new Dictionary<string, Tuple<string, Model, Color>>();

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
            public float Speed;

            public ModelsStatsStruct(int c, float a, float m) {
                Capacity = c;
                AttackDistance = a;
                Speed = m;
            }
        }
    }
}

