// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Engine {
    public class EffectMaterial {
        ////////// represents a material which sets lighting parameters //////////

        // --------------------- VARIABLES ---------------------

        //public
        public bool Lit;
        public Color Diffuse;


        //private


        //reference
        public EffectMaterial(bool lit, Color color) {
            Lit = lit; Diffuse = color;
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}