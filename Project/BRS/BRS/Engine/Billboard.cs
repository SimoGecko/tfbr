// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Billboard : Component {
        ////////// simple billboard that looks at the camera //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference
        public Transform parent;
        public static Billboard instance;

        // --------------------- BASE METHODS ------------------
        public Billboard(Transform t) {
            parent = t;
            instance = this;
        }

        public override void Start() {

        }

        public override void Update() {
            transform.position = parent.position;

            transform.LookAt(Camera.main.transform.position);
            //Quaternion extraRot = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(90));
            //transform.localRotation* extraRot;
            
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw() {
            Vector2 position = Camera.main.WorldToScreenPoint(transform.position);
            //Debug.Log("pos=" + position);
            UserInterface.instance.DrawString(position, "OOO");
        }


        // queries



        // other

    }

}