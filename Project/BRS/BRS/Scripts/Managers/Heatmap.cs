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
        bool showHeatMapOnScreen = false;

        //private
        CDFdistrib green, yellow, white; // stores yellow distribution

        float pixelSize;

        int[,] playerHeatmap;
        int distribWidth, distribHeight;

        //reference
        Texture2D heatmapPic;
        public static Heatmap instance;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            instance = this;
        }

        public override void Start() {
            //DISTRIB   
            Texture2D moneyPic = BRS.Engine.File.Load<Texture2D>("Images/heatmap/level1_green");
            Texture2D goldPic = BRS.Engine.File.Load<Texture2D>("Images/heatmap/level1_yellow");
            Texture2D uniformPic = BRS.Engine.File.Load<Texture2D>("Images/heatmap/level1_white");

            green = new CDFdistrib(moneyPic, 1);
            yellow = new CDFdistrib(goldPic, 1);
            white = new CDFdistrib(uniformPic, 1);

            distribWidth = moneyPic.Width;
            distribHeight = moneyPic.Height;
            pixelSize = (float)PlayArea.SpawnArea.Width / moneyPic.Width;

            //HEATMAP
            heatmapPic = BRS.Engine.File.Load<Texture2D>("Images/heatmap/level1_heatmap");
            //playerHeatmap = new int[distribWidth, distribHeight];
            StartComputingHeatmap();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public Vector2 GetCashPos() {
            return EvaluateCDFAndGetPos(green);
        }

        public Vector2 GetGoldPos() {
            return EvaluateCDFAndGetPos(yellow);
        }

        public Vector2 GetUniformPos() {
            return EvaluateCDFAndGetPos(white);
        }

        Vector2 EvaluateCDFAndGetPos(CDFdistrib distrib) {
            Vector2 pixel = distrib.Evaluate().ToVector2();
            Vector2 normalizedCoords = Vector2.Divide(new Vector2(pixel.Y, pixel.X), new Vector2(distribWidth, distribHeight));
            return PlayArea.MapArea.Evaluate(normalizedCoords) + MyRandom.InsideUnitCircle() * pixelSize;
        }



        // queries


        async void StartComputingHeatmap() {
            int refreshFps = 40;
            //checks continuously to see where the players are and increases a counter to get a heatmap in the end
            playerHeatmap = new int[distribWidth, distribHeight];
            while (true) {
                foreach (Player p in ElementManager.Instance.Players()) {
                    Vector2 normCoord = PlayArea.Pos3DNormalized(p.transform.position);
                    Point coord = new Point((int)(distribWidth * normCoord.X), (int)(distribHeight * normCoord.Y));
                    coord.X = MathHelper.Clamp(coord.X, 0, distribWidth-1);
                    coord.Y = MathHelper.Clamp(coord.Y, 0, distribHeight-1);
                    playerHeatmap[coord.X, coord.Y]+=10;
                    //Texture2D heatmapResult = Graphics.ColorToTexture(Graphics.IntToColor(playerHeatmap));
                    Color[] colors = Graphics.Color2DToColor1D(Graphics.IntToColor(playerHeatmap));
                    heatmapPic.SetData(colors);

                }
                await Time.WaitForSeconds(1f / refreshFps);
            }
        }

        public void SaveHeatMap() { // TODO have nico save this to array/picture
            //when game is closed

            Color[,] colorData = Graphics.TextureTo2DArray(heatmapPic);
            int[,] array2Dint = Graphics.ColorToInt(colorData, 0);
            Engine.File.Write2DArrayIntToFile("Load/saved_heatmap_level1.txt", array2Dint);

            //Stream stream = System.IO.File.Create("Load/saved_heatmap_level1.png");
            //heatmapPic.SaveAsPng(stream, heatmapPic.Width, heatmapPic.Height);
            //stream.Dispose();
            //heatmapPic.Dispose();
        }

        public override void Draw2D(int index) {
            if(showHeatMapOnScreen) UserInterface.DrawPicture(heatmapPic, Vector2.One * 100);
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