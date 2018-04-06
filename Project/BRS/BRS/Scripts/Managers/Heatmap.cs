// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using BRS.Engine;

namespace BRS.Scripts {
    class Heatmap : Component {
        ////////// stores a texture, accesses rgb components and evaluates the distribution //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        CDFdistrib green; // stores green distribution
        //_upperLeftPt = new Vector3(-25, 0, -75); //looked up in unity
        //_lowerRightPt = new Vector3(25, 0, 5);

        Rectangle playArea = new Rectangle(-25, -75, 50, 80);

        //reference
        Texture2D heatmapPic;
        public static Heatmap instance;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            instance = this;
        }

        public override void Start() {
            heatmapPic = File.Load<Texture2D>("Images/heatmap/level1");
            green = new CDFdistrib(heatmapPic, 1);

            //int[,] test = new int[,] { { 7, 8, 4 }, { 2, 6, 1 }, { 0, 5, 3 }, { 4, 1, 0 } };
            //CDFdistrib newdistrib = new CDFdistrib(ref test);
            //Debug.Log("it");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public Vector2 GetMoneyPos() {
            Vector2 pixel = green.Evaluate().ToVector2();
            //Vector2 normalizedCoords = Vector2.Divide(pixel, new Vector2(heatmapPic.Height, heatmapPic.Width));
            Vector2 normalizedCoords = Vector2.Divide(new Vector2(pixel.Y, pixel.X), new Vector2(heatmapPic.Width, heatmapPic.Height));
            return playArea.Evaluate(normalizedCoords);
        }
        


        // queries
        

        

        



        // other
        class CDFdistrib {
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
}