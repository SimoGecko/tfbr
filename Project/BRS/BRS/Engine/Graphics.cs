// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS {
    /// <summary>
    /// Collection of various methods that have to do with color.
    /// </summary>
    class Graphics {
        // --------------------- VARIABLES ---------------------

        //public
        //some default colors (same as microsoft)
        /*
        public static Color red    = new Color(246, 83, 20);
        public static Color green  = new Color(124, 187, 0);
        public static Color blue   = new Color(0, 161, 241);
        public static Color yellow = new Color(255, 187, 0);
        */
        //some default colors (same as google)
        private static readonly Color Red    = new Color(234, 67, 53);
        private static readonly Color Green  = new Color(52, 168, 83);
        private static readonly Color Blue   = new Color(66, 133, 244);
        private static readonly Color Yellow = new Color(251, 188, 5);


        //private


        //reference



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //GRAPHICS METHODS
        public static void DrawModel(Model model, Matrix view, Matrix proj, Matrix world, EffectMaterial mat = null) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    if (mat == null) {
                        //default settings
                        effect.EnableDefaultLighting();
                    } else {
                        effect.EnableDefaultLighting();
                        //effect.LightingEnabled = mat.lit;
                        //effect.DiffuseColor = mat.diffuse.ToVector3();
                        effect.Alpha = mat.Diffuse.A;
                        //effect.CurrentTechnique = EffectTechnique
                        //effect.Texture
                    }
                    //effect.Alpha = .5f;
                    //effect.di
                    //effect.EnableDefaultLighting();

                    //effects
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = proj;
                }
                mesh.Draw(); // outside, not inside
            }
        }

        public static Color[,] TextureTo2DArray(Texture2D texture) {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];
            return colors2D;
        }

        public static Color ColorIndex(int i) {
            if (i == 0) return Red;
            if (i == 1) return Green;
            if (i == 2) return Blue;
            return Yellow;
        }
        /*
        public static Vector3 ColorTo3(this Color c) {
            return new Vector3((float)c.R/255, (float)c.G/255, (float)c.B/255);
        }*/


    }

}