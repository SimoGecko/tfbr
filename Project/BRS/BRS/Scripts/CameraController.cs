// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    /// <summary>
    /// Sets the camera position and follows the player smoothly
    /// </summary>
    class CameraController : Component {

        // --------------------- VARIABLES ---------------------

        //public


        //private
        Vector3 offset;
        Vector3[] refVelocity;

        //reference
        Transform[] cams;
        Transform[] players;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            int numPlayers = GameManager.numPlayers;
            cams = new Transform[numPlayers];
            players = new Transform[numPlayers];
            refVelocity = new Vector3[numPlayers];

            for(int i=0; i<numPlayers; i++) {
                cams[i] = Camera.GetCamera(i).transform;
                if (cams[i] != null) {
                    cams[i].position = new Vector3(0, 15, 15);
                    cams[i].eulerAngles = new Vector3(-45, 0, 0);

                    players[i] = GameObject.FindGameObjectWithName("player_"+i).Transform;
                    if (players[i] == null) Debug.LogError("player not found");
                    if(offset == Vector3.Zero)
                        offset = cams[i].position - players[i].position;
                }
            }

        }

        public override void Update() {

            for (int i = 0; i < GameManager.numPlayers; i++) {
                if (cams[i] != null) {
                    Vector3 target = players[i].position + offset;
                    cams[i].position = Utility.SmoothDamp(cams[i].position, target, ref refVelocity[i], .3f);
                }
            }
            

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }

}