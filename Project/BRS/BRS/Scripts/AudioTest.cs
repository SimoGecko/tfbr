// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class AudioTest : Component {
        ////////// DESCRIPTION //////////

        // --------------------- VARIABLES ---------------------

        //public
        Vector3 position = new Vector3(5, 0, -5);

        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            GameObject sound = new GameObject("sphereaudio", File.Load<Model>("models/primitives/sphere"));
            sound.transform.position = position;
        }

        public override void Update() {
            if (Input.GetKeyDown(Keys.H)) {
                Audio.Play("mono/phi", position);
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}