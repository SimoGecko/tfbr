// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class TransformTest : Component {
        ////////// test out transform math //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            transform.position = new Vector3(-7, 2, -1);
        }

        public override void Update() {
            transform.RotateAround(new Vector3(-5, 0, -1), Vector3.Up, 180 * Time.deltaTime);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }

}