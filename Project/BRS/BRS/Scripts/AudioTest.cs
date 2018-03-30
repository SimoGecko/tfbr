// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class AudioTest : Component {
        ////////// DESCRIPTION //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {
            if (Input.GetKeyDown(Keys.H)) {
                Audio.Play("phi", Vector3.Zero);
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}