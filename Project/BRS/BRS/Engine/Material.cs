// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    public class EffectMaterial {
        ////////// represents a material which sets lighting parameters //////////

        // --------------------- VARIABLES ---------------------

        //public
        public bool lit;
        public Color diffuse;


        //private


        //reference
        public EffectMaterial(bool _lit, Color _color) {
            lit = _lit; diffuse = _color;
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}