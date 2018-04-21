// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;

namespace BRS.Scripts {
    class MaterialTest : Component {
        ////////// updates material color //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference
        public Material mat;


        // --------------------- BASE METHODS ------------------
        public MaterialTest(Material _mat) {
            mat = _mat;
        }

        public override void Start() {

        }

        public override void Update() {
            mat.Diffuse = Color.Lerp(Color.Green, Color.Blue, (float) System.Math.Abs(System.Math.Sin(Time.CurrentTime * 3)));
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}