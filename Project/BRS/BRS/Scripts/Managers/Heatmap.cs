// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using System.IO;

namespace BRS.Scripts {
    class Heatmap : Component {
        ////////// stores a texture, accesses rgb components and evaluates the distribution //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        CDFdistrib green; // stores green distribution
        CDFdistrib yellow; // stores yellow distribution
        //_upperLeftPt = new Vector3(-25, 0, -75);
        //_lowerRightPt = new Vector3(25, 0, 5);

        float pixelSize;

        int[,] playerHeatmap;
        int width, height;

        //reference
        Texture2D moneyPic;
        Texture2D goldPic;
        Texture2D heatmapPic;
        public static Heatmap instance;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            instance = this;
        }

        public override void Start() {
            moneyPic = BRS.Engine.File.Load<Texture2D>("Images/heatmap/level1_green");
            goldPic  = BRS.Engine.File.Load<Texture2D>("Images/heatmap/level1_yellow");
            heatmapPic = BRS.Engine.File.Load<Texture2D>("Images/heatmap/level1_heatmap");
            green = new CDFdistrib(moneyPic, 1);
            yellow = new CDFdistrib(goldPic, 1);

            width = moneyPic.Width;
            height = moneyPic.Height;

            pixelSize = (float)PlayArea.SpawnArea.Width / moneyPic.Width;

            //int[,] test = new int[,] { { 7, 8, 4 }, { 2, 6, 1 }, { 0, 5, 3 }, { 4, 1, 0 } };
            //CDFdistrib newdistrib = new CDFdistrib(ref test);
            //Debug.Log("it");

            StartComputingHeatmap();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public Vector2 GetCashPos() {
            Vector2 pixel = green.Evaluate().ToVector2();
            Vector2 normalizedCoords = Vector2.Divide(new Vector2(pixel.Y, pixel.X), new Vector2(moneyPic.Width, moneyPic.Height));
            return PlayArea.SpawnArea.Evaluate(normalizedCoords) + MyRandom.InsideUnitCircle() * pixelSize;
        }

        public Vector2 GetGoldPos() {
            Vector2 pixel = yellow.Evaluate().ToVector2();
            Vector2 normalizedCoords = Vector2.Divide(new Vector2(pixel.Y, pixel.X), new Vector2(goldPic.Width, goldPic.Height));
            return PlayArea.SpawnArea.Evaluate(normalizedCoords) + MyRandom.InsideUnitCircle() * pixelSize;
        }



        // queries


        async void StartComputingHeatmap() {
            int refreshFps = 40;
            //checks continuously to see where the players are and increases a counter to get a heatmap in the end
            playerHeatmap = new int[width, height];
            while (true) {
                foreach (Player p in ElementManager.Instance.Players()) {
                    Vector2 normCoord = PlayArea.Pos3DNormalized(p.transform.position);
                    Point coord = new Point((int)(width * normCoord.X), (int)(height * normCoord.Y));
                    coord.X = MathHelper.Clamp(coord.X, 0, width-1);
                    coord.Y = MathHelper.Clamp(coord.Y, 0, height-1);
                    playerHeatmap[coord.X, coord.Y]+=10;
                    //Texture2D heatmapResult = Graphics.ColorToTexture(Graphics.IntToColor(playerHeatmap));
                    Color[] colors = Graphics.Color2DToColor1D(Graphics.IntToColor(playerHeatmap));
                    heatmapPic.SetData(colors);

                }
                await Time.WaitForSeconds(1f / refreshFps);
            }
            //TODO call save
        }

        public void SaveHeatMap() {
            //when game is closed
            Stream stream = System.IO.File.Create("Images/heatmap/level1_heatmap.png");
            heatmapPic.SaveAsPng(stream, heatmapPic.Width, heatmapPic.Height);
            stream.Dispose();
            heatmapPic.Dispose();
        }

        public override void Draw(int index) {
            //UserInterface.DrawPicture(heatmapPic, Vector2.One * 100);

        }


    }
    // other
    public class CDFdistrib {
        //represents a distribution on a grid, initialized with simple CDF
        public int[] colsum; // partial sums of whole rows
        public int[][] rowsum; // partial sums of single row
        int rows, cols;

        //can be created via array or texture
        public CDFdistrib(ref int[,] distrib) {
            MakeCDF(ref distrib);
        }

        public CDFdistrib(Texture2D heatmapPic, int channel) {
            Color[,] texArray = Graphics.TextureTo2DArray(heatmapPic);
            int[,] distrib = Utility.Flip(Graphics.ColorToInt(texArray, channel));
            MakeCDF(ref distrib);
        }

        void MakeCDF(ref int[,] distrib) {
            rows = distrib.GetLength(0);
            cols = distrib.GetLength(1);

            colsum = new int[rows];
            rowsum = new int[rows][];

            for (int x = 0; x < rows; x++) {
                colsum[x] = 0;
                rowsum[x] = new int[cols];
                for (int y = 0; y < cols; y++) {
                    rowsum[x][y] = rowsum[x][MathHelper.Max(y - 1, 0)] + distrib[x, y];
                }
                colsum[x] = colsum[MathHelper.Max(x - 1, 0)] + rowsum[x][cols - 1];
            }
        }

        public Point Evaluate() {
            int maxX = Sum();
            int sampleX = MyRandom.Range(0, maxX);
            int indexX = Array.BinarySearch(colsum, sampleX);
            if (indexX < 0) indexX = ~indexX; // if not found exactly, this is index of first number bigger than sampleX

            int maxY = RowSum(indexX);
            int sampleY = MyRandom.Range(0, maxY);
            int indexY = Array.BinarySearch(rowsum[indexX], sampleY);
            if (indexY < 0) indexY = ~indexY;

            return new Point(indexX, indexY); // this is pixel, now normalize (row, col)
        }

        public int Sum() { return colsum[rows - 1]; }
        public int RowSum(int i) { return rowsum[i][cols - 1]; }
    }
}