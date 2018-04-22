// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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