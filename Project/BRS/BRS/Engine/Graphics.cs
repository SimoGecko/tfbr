// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine.Rendering;
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
        //COLORS USED FOR PLAYERS AND STUFF
        public static Color Yellow = new Color(255, 198, 13);
        public static Color Green = new Color(109, 202, 35);
        public static Color Blue = new Color(0, 158, 255);
        public static Color Purple = new Color(162, 86, 255);
        public static Color Pink = new Color(254, 68, 212);
        public static Color Red = new Color(234, 67, 53);
        public static Color Orange = new Color(234, 142,53);



        public static Color White = new Color(255, 255, 255);


        public static Color Clear = new Color(255, 255, 255, 0);
        public static Color StreetGray = new Color(69, 69, 69, 0);
        public static Color SkyBlue = new Color(50, 206, 244, 0);
        //private

        public static GraphicsDeviceManager gDM;
        public static GraphicsDevice gD { get { return gDM.GraphicsDevice; } }

        public static Effect texlightEffect;
        public static Effect textureEffect;
        public static Effect skyboxEffect;
        //public static Texture2D lightMap;
        //public static Texture2D textureCol;

        //reference

        public static void Start() {
            texlightEffect = File.Load<Effect>("Other/shaders/baked");
            //texlightEffect = File.Load<Effect>("Other/shaders/lightmap");
            textureEffect = File.Load<Effect>("Other/shaders/textured");
            skyboxEffect = File.Load<Effect>("Other/effects/Skybox");
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //GRAPHICS METHODS
        public static void DrawModel(Model model, Matrix view, Matrix proj, Matrix world, Material mat = null) {
            //DrawModelInstanciated(model, world, view, proj, instances);
            //selects which effect to use based on material
            if (mat == null) DrawModelSimple(model, view, proj, world);
            else if (mat.baked) DrawModelBaked(model, mat.colorTex, mat.lightTex, view, proj, world);
            else if (mat.textured) DrawModelTextured(model, mat.colorTex, view, proj, world, mat.IsTransparent, mat.IsAlphaAnimated);
            else DrawModelMaterial(model, view, proj, world, mat);
        }


        //if there is no material attached
        static void DrawModelSimple(Model model, Matrix view, Matrix proj, Matrix world) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (Effect effect in mesh.Effects) {
                    if (effect is BasicEffect) {
                        BasicEffect beff = (BasicEffect)effect;
                        beff.EnableDefaultLighting();

                        beff.World = world;
                        beff.View = view;
                        beff.Projection = proj;
                    }

                }
                mesh.Draw(); // outside, not inside
            }
        }

        static void DrawModelMaterial(Model model, Matrix view, Matrix proj, Matrix world, Material mat) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    //use base effect with diffuse color and alpha (and ev texture)
                    effect.EnableDefaultLighting();
                    //effect.LightingEnabled = mat.Lit;
                    effect.DiffuseColor = mat.DiffuseColor;
                    //effect.Alpha = mat.Diffuse.A;
                    //effect.CurrentTechnique = EffectTechnique

                    //effects
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = proj;
                }
                mesh.Draw(); // outside, not inside
            }
        }

        static void DrawModelTextured(Model model, Texture2D colorTex, Matrix view, Matrix proj, Matrix world, bool isTransparent, bool isAlphaAnimated) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = textureEffect;
                    textureEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    textureEffect.Parameters["View"].SetValue(view);
                    textureEffect.Parameters["Projection"].SetValue(proj);

                    textureEffect.Parameters["ColorTexture"].SetValue(colorTex);
                    textureEffect.Parameters["IsTransparent"].SetValue(isTransparent); // why the fuck would you modify this
                    textureEffect.Parameters["IsAlphaAnimated"].SetValue(isAlphaAnimated);
                }
                mesh.Draw();
            }
        }

        static void DrawModelBaked(Model model, Texture2D colorTex, Texture2D lightTex, Matrix view, Matrix proj, Matrix world) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = texlightEffect;
                    texlightEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    texlightEffect.Parameters["View"].SetValue(view);
                    texlightEffect.Parameters["Projection"].SetValue(proj);

                    texlightEffect.Parameters["ColorTexture"].SetValue(colorTex);
                    texlightEffect.Parameters["LightmapTexture"].SetValue(lightTex);
                }
                mesh.Draw();
            }
        }


        //----------------------------------------------------------------------------------------------


        //TEXTURE METHODS
        public static Texture2D TextureTint(Texture2D tex, Color tint) {
            //returns a copy of tex tinted with the color
            Texture2D result = new Texture2D(gD, tex.Width, tex.Height);
            Color[] data = new Color[tex.Width * tex.Height];
            tex.GetData(data);

            for (int j = 0; j < data.Length; ++j) {
                if (data[j].A > 0) {
                    data[j].R = tint.R;
                    data[j].G = tint.G;
                    data[j].B = tint.B;
                }
            }
            result.SetData(data);
            return result;
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

        public static Color[] TextureTo1DArray(Texture2D texture) {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            return colors1D;
            //return FlipRB(colors1D);
        }

        public static Color[] FlipRB(Color[] colors) {
            Color[] result = new Color[colors.Length];
            for(int i=0; i<colors.Length; i++) {
                result[i] = new Color(colors[i].B, colors[i].G, colors[i].R);
            }
            return result;
        }

        public static Texture2D ColorToTexture(Color[,] color) {
            int width = color.GetLength(0);
            int height = color.GetLength(1);
            Texture2D result = new Texture2D(gD, width, height);

            Color[] res = new Color[width * height];
            for (int i = 0; i < width * height; i++) {
                res[i] = color[i % width, i / width]; // TODO sure?
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
                    colors1D[x + y * width] = colors2D[x, y];
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
            int width = val.GetLength(0);
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