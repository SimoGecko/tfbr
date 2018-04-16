// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    ////////// Collection of various methods that have to do with color and graphics //////////
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
        /*
        private static readonly Color Red    = new Color(234, 67, 53);
        private static readonly Color Green  = new Color(52, 168, 83);
        private static readonly Color Blue   = new Color(66, 133, 244);
        private static readonly Color Yellow = new Color(251, 188, 5);*/

            //default colors from unity
        public static Color Green = new Color(109, 202, 35);
        public static Color Blue = new Color(0, 158, 255);
        public static Color Yellow = new Color(255, 198, 13);
        public static Color Red = new Color(234, 67, 53);


        public static Color Clear = new Color(255, 255, 255, 0);
        //private

        public static GraphicsDeviceManager gDM;
        public static GraphicsDevice gD { get { return gDM.GraphicsDevice; } }


        public static Effect texlightEffect;
        public static Texture2D lightMap;
        public static Texture2D textureCol;
        //reference



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //GRAPHICS METHODS
        public static void DrawModel(Model model, Matrix view, Matrix proj, Matrix world, Material mat = null) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    if (mat == null) mat = Material.Default;

                    effect.EnableDefaultLighting();
                    effect.LightingEnabled = mat.Lit;
                    effect.DiffuseColor = mat.Color;
                    //effect.Alpha = mat.Diffuse.A;
                    //effect.CurrentTechnique = EffectTechnique
                    //effect.Texture

                    //effects
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = proj;
                }
                mesh.Draw(); // outside, not inside
            }
        }

        public static void DrawModelWithEffect(Model model, Matrix view, Matrix proj, Matrix world) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = texlightEffect;
                    texlightEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    texlightEffect.Parameters["View"].SetValue(view);
                    texlightEffect.Parameters["Projection"].SetValue(proj);
                    texlightEffect.Parameters["ColorTexture"].SetValue(textureCol);
                    texlightEffect.Parameters["LightmapTexture"].SetValue(lightMap);
                }
                mesh.Draw();
            }
        }

        //COLOR METHODS
        public static Color[,] TextureTo2DArray(Texture2D texture) {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];
            return colors2D;
        }

        public static Texture2D ColorToTexture(Color[,] color) {
            int width = color.GetLength(0);
            int height = color.GetLength(1);
            Texture2D result = new Texture2D(gD, width, height);

            Color[] res = new Color[width * height];
            for (int i = 0; i < width * height; i++) {
                res[i] = color[i%width, i/width]; // TODO sure?
            }

            result.SetData(res);
            return result;
        }

        public static Color[] Color2DToColor1D(Color[,] colors2D) {
            int width = colors2D.GetLength(0);
            int height = colors2D.GetLength(1);
            Color[] colors1D = new Color[width * height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    colors1D[x + y * width]= colors2D[x, y];
            return colors1D;

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

        public static int[,] ColorToInt(Color[,] color, int channel) {
            int width = color.GetLength(0);
            int height = color.GetLength(1);
            int[,] result = new int[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    result[x, y] = (channel == 0) ? color[x, y].R : (channel == 1) ? color[x, y].G : color[x, y].B;
                }
            }
            return result;
        }

        public static Color[,] IntToColor(int[,] val) {
            int width  = val.GetLength(0);
            int height = val.GetLength(1);
            Color[,] result = new Color[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int v = MathHelper.Clamp(val[x, y], 0, 255);
                    result[x, y] = new Color(v, v, v);
                }
            }
            return result;
        }

    }

}