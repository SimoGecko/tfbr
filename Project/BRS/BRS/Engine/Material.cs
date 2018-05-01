// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    public class Material {
        ////////// represents a material which sets lighting parameters //////////

        // --------------------- VARIABLES ---------------------

        //public
        public bool Lit;
        public Color Diffuse;
        public bool IsTransparent;

        public bool baked;
        public bool textured;
        public Texture2D colorTex;
        public Texture2D lightTex;

        //Texture2D Texture;
        //EffectTechnique tt;

        public static Material Default = new Material(new Color(140, 140, 140), true);
           


        //private


        //reference
        public Material() {
            Diffuse = Color.White;
            Lit = baked = false;
        }
        public Material(Texture2D color, Texture2D light) {
            baked = true;
            colorTex = color;
            lightTex = light;
        }
        public Material(Texture2D color, bool isTransparent = false) {
            baked = false;
            textured = true;
            colorTex = color;
            IsTransparent = isTransparent;
        }

        public Material(Color color, bool lit=true) {
            Lit = lit; Diffuse = color;
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public Vector3 DiffuseColor { get { return Diffuse.ToVector3(); } }


        // queries



        // other

    }
}