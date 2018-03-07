// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class CameraController : Component {
        ////////// Sets the camera position and follows the player smoothly //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        Vector3 offset;
        Vector3 refVelocity, refVelocity2;

        //reference
        Transform cam, cam2;
        Transform player, player2;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            cam = Camera.main.transform;
            cam2 = (Camera.main2!=null)? Camera.main2.transform : null; // THIS is null!!

            if (cam != null) {
                cam.position = new Vector3(0, 10, 7);
                cam.eulerAngles = new Vector3(-45, 0, 0);

                player = GameObject.FindGameObjectWithName("player_0").transform;
                if (player == null) Debug.LogError("player not found");
                offset = cam.position - player.position;
            }

            
            if (cam2 != null) {
                cam2.position = new Vector3(30, 10, 7);
                cam2.eulerAngles = new Vector3(-45, 0, 0);

                player2 = GameObject.FindGameObjectWithName("player_1").transform;
                if (player2 == null) Debug.LogError("player2 not found");
                
            }

        }

        public override void Update() {
            if (cam != null) {
                Vector3 target = player.position + offset;
                cam.position = Utility.SmoothDamp(cam.position, target, ref refVelocity, .3f);
            }
            
            if (cam2 != null) {
                Vector3 target = player2.position + offset;
                cam2.position = Utility.SmoothDamp(cam2.position, target, ref refVelocity2, .3f);
            }

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }

}