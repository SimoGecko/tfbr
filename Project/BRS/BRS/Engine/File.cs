// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BRS.Scripts {
    class File : Component {
        ////////// class used to load files from disk and providing safeguard agains null files //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference
        static ContentManager content;


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public static void Load(string s) {
            //content.Load<T>(s);
        }


        // queries



        // other

    }

}