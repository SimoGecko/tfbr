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
        //Texture2D Texture;
        //EffectTechnique tt;

        public static Material Default = new Material(new Color(140, 140, 140), true);
           


        //private


        //reference
        public Material(Color color, bool lit=true) {
            Lit = lit; Diffuse = color;
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public Vector3 Color { get { return Diffuse.ToVector3(); } }


        // queries



        // other

    }
}