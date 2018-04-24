// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;

namespace BRS.Scripts {
    class PlayArea : Component {
        ////////// stores map size and converts between 3d positions to normalized coordinates as well as texture coords //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        public static Rectangle SpawnArea = new Rectangle(-25, -75, 50, 80);
        public static Rectangle MapArea = new Rectangle(-45, -95, 90, 115); // for the minimap, including unreachable area

        static Vector3 _upperLeftPt = new Vector3(-45, 0, -95); //looked up in unity
        static Vector3 _lowerRightPt = new Vector3(45, 0, 20); // corners of physical map


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries
        public static Vector2 Pos3DNormalized(Vector3 pos) { // converts 3d position of object to normalized playArea coordinates
            Vector3 L = _upperLeftPt, R = _lowerRightPt;
            float x0 = (pos.X - L.X) / (R.X - L.X);
            float y0 = (pos.Z - L.Z) / (R.Z - L.Z);
            return new Vector2(x0, y0);
        }


        // other

    }
}