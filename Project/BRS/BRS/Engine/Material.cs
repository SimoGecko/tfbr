// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    public class Material {
        ////////// represents a material which sets lighting parameters //////////

        // --------------------- VARIABLES ---------------------

        //public
        //public bool Lit;
        public Color Diffuse;
        public bool IsTransparent;

        public bool IsAlphaAnimated;
        [Range(0.0f, 1.0f)]
        public float Alpha;

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
            //Lit = false;
        }
        public Material(Texture2D color, Texture2D light) {
            baked = true;
            colorTex = color;
            lightTex = light;
        }
        public Material(Texture2D color, bool isTransparent = false, bool isAlphaAnimated = false, float alpha = 1.0f) {
            baked = false;
            textured = true;
            colorTex = color;
            IsTransparent = isTransparent;
            IsAlphaAnimated = isAlphaAnimated;
            Alpha = alpha;
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
                Alpha = Alpha,
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