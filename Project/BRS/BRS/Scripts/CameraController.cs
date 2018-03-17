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
        ////////// Sets the camera position and follows the player smoothly, allowing also rotation //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float smoothTime = .3f;
        const float mouseSensitivity = .5f;
        const float gamepadSensitivity = 3f;
        static Vector3 offset = new Vector3(0, 10, 10);
        static Vector3 angles = new Vector3(-50, 0, 0);
        const float startXangle = -50;

        //private
        float Xangle = 0, XangleSmooth=0;
        float Yangle = 0, YangleSmooth=0;
        float refVelocityX = 0, refVelocityY = 0;
        public int camIndex;

        //reference
        Transform player;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //Xangle = XangleSmooth = startXangle;

            player = GameObject.FindGameObjectWithName("player_" + camIndex).Transform;
            if (player == null) Debug.LogError("player not found");

            transform.position = player.position + offset;
            transform.eulerAngles = angles;
        }

        public override void LateUpdate() { // after player has moved
            float inputX = ( Input.mouseDelta.X*mouseSensitivity).Clamp(-100, 100); // clamp is to avoid initial weird jump in mouse delta
            float inputY = (-Input.mouseDelta.Y*mouseSensitivity).Clamp(-100, 100);

            inputX += -Input.GetThumbstick("Right", camIndex).X * gamepadSensitivity;
            inputY += -Input.GetThumbstick("Right", camIndex).Y * gamepadSensitivity;

            Xangle = (Xangle + inputY).Clamp(-40, 40);
            XangleSmooth = Utility.SmoothDamp(XangleSmooth, Xangle, ref refVelocityX, smoothTime);

            Yangle += inputX;
            YangleSmooth = Utility.SmoothDamp(YangleSmooth, Yangle, ref refVelocityY, smoothTime);

            transform.position = player.position + offset;
            transform.eulerAngles = angles;

            transform.RotateAround(player.position, Vector3.Up, YangleSmooth);
            transform.RotateAround(player.position, transform.Right, XangleSmooth);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries
        public float YRotation { get { return Yangle; } }



        // other

    }

}