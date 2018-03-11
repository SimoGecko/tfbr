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
        public int camIndex;
        const float smoothTime = .3f;

        //private
        static Vector3 offset;
        Vector3 refVelocity;

        //reference
        //Transform cams;
        Transform player;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            /*
            int numPlayers = GameManager.numPlayers;
            cams = new Transform[numPlayers];
            players = new Transform[numPlayers];
            refVelocity = new Vector3[numPlayers];
            
            for(int i=0; i<numPlayers; i++) {
                cams[i] = Camera.GetCamera(i).transform;
                if (cams[i] != null) {
                    cams[i].position = new Vector3(-5, 7, 5);
                    cams[i].eulerAngles = new Vector3(-50, 0, 0);

                    players[i] = GameObject.FindGameObjectWithName("player_"+i).transform;
                    if (players[i] == null) Debug.LogError("player not found");
                    if(offset == Vector3.Zero)
                        offset = cams[i].position - players[i].position;
                }
            }*/
            transform.position = new Vector3(-5, 7, 5);
            transform.eulerAngles = new Vector3(-50, 0, 0);

            player = GameObject.FindGameObjectWithName("player_" + camIndex).transform;
            if (player == null) Debug.LogError("player not found");
            if (offset == Vector3.Zero)
                offset = transform.position - player.position;

        }

        public override void Update() {
            Vector3 target = player.position + offset;
            transform.position = Utility.SmoothDamp(transform.position, target, ref refVelocity, smoothTime);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }

}