// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    public class Material {
        ////////// represents a material which sets lighting parameters //////////

        // --------------------- VARIABLES ---------------------

        public RenderingType RenderingType;
        //simple = basic effect without anything
        //textured = one single texture
        //baked = texture for color and for lightmap

        //public
        //public bool Lit;
        public Color Diffuse;
        public bool IsTransparent;

        public bool IsAlphaAnimated;

        public bool baked;
        public bool textured;
        public bool skybox;
        public Texture2D colorTex;
        public Texture2D lightTex;

        //Texture2D Texture;
        //EffectTechnique tt;

        public static Material Default = new Material(new Color(140, 140, 140), true);



        //private


        //reference
        public Material() {
            Diffuse = Color.White;
            baked = false;
            textured = false;
            //Lit = false;
        }
        public Material(Texture2D color, Texture2D light) {
            RenderingType = RenderingType.HIBaked;
            baked = true;
            textured = false;
            colorTex = color;
            lightTex = light;
        }
        public Material(Texture2D color, bool isTransparent = false, bool isAlphaAnimated = false) {
            if (isAlphaAnimated) {
                RenderingType = RenderingType.HITextureAlphaAnimated;
            } else if (isTransparent) {
                RenderingType = RenderingType.HITextureTransparent;
            } else {
                RenderingType = RenderingType.HITexture;
            }

            baked = false;
            textured = true;
            colorTex = color;
            IsTransparent = isTransparent;
            IsAlphaAnimated = isAlphaAnimated;
        }
        public Material(string type) {
            if (type == "skybox") skybox = true;
        }

        public Material(Color color, bool lit = true) {
            Diffuse = color;
            //Lit = lit;
        }


        // --------------------- CUSTOM METHODS ----------------

        public Material Clone() {
            return new Material {
                Diffuse = Diffuse,
                IsTransparent = IsTransparent,
                IsAlphaAnimated = IsAlphaAnimated,
                baked = baked,
                textured = textured,
                colorTex = colorTex,
                lightTex = lightTex
            };
        }


        // commands
        public Vector3 DiffuseColor { get { return Diffuse.ToVector3(); } }


        // queries



        // other

    }
}