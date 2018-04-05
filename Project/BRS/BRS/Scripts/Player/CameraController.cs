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
        ////////// Sets the camera position and follows the player smoothly, also allowing rotation //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float smoothTime = .2f;
        const float positionSmoothTime = .1f;
        static Vector2 mouseSensitivity =new Vector2(-.3f, -.3f); // set those (also with sign) into options menu
        static Vector2 gamepadSensitivity = new Vector2(-2f, -2f);
        static Vector3 offset = new Vector3(0, 10, 10);
        static Vector3 startAngle = new Vector3(-45, 0, 0);
        const int angleVariation = 5;
        static Vector2 angleRange = new Vector2(-angleVariation, angleVariation); // -40, 40

        const float shakeAmount = .3f;

        //private
        float Xangle = 0, XangleSmooth=0;
        float Yangle = 0, YangleSmooth=0;
        float refVelocityX = 0, refVelocityY = 0;
        public int camIndex;
        Vector3 targetPosRef;


        float shakeDuration = 0;
        Vector3 shake;

        //reference
        Transform player;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Xangle = XangleSmooth = Yangle = YangleSmooth = refVelocityX = refVelocityY = 0;

            player = GameObject.FindGameObjectWithName("player_" + camIndex).transform;
            if (player == null) Debug.LogError("player not found");

            transform.position = player.position + offset;
            transform.eulerAngles = startAngle;
        }

        public override void LateUpdate() { // after player has moved
            ProcessInput();
            FollowSmoothAndRotate();
            ProcessShake();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void ProcessInput() {
            float inputX = (Input.mouseDelta.X * mouseSensitivity.X).Clamp(-20, 20); // clamp is to avoid initial weird jump in mouse delta // TODO FIX
            float inputY = (Input.mouseDelta.Y * mouseSensitivity.Y).Clamp(-100, 100);

            inputX += Input.GetThumbstick("Right", camIndex).X * gamepadSensitivity.X;
            inputY += Input.GetThumbstick("Right", camIndex).Y * gamepadSensitivity.Y;

            Xangle = (Xangle + inputY).Clamp(angleRange.X, angleRange.Y);
            XangleSmooth = Utility.SmoothDamp(XangleSmooth, Xangle, ref refVelocityX, smoothTime);

            Yangle += inputX;
            YangleSmooth = Utility.SmoothDamp(YangleSmooth, Yangle, ref refVelocityY, smoothTime);

        }

        void FollowSmoothAndRotate() {
            Vector3 currentPosition = transform.position;

            transform.position = player.position + offset;
            transform.eulerAngles = startAngle;

            transform.RotateAround(player.position, Vector3.Up, YangleSmooth);
            transform.RotateAround(player.position, transform.Right, XangleSmooth);
            Vector3 targetPos = transform.position;
            //transform.position = Utility.SmoothDamp(currentPosition, targetPos, ref targetPosRef, positionSmoothTime);

        }

        void ProcessShake() {
            if (shakeDuration > 0) {
                shake = MyRandom.insideUnitSphere() * shakeAmount * Curve.EvaluateC(shakeDuration);
                shakeDuration -= Time.deltaTime;
                transform.position += shake;
            } else {
                shake = Vector3.Zero;
                shakeDuration = 0;
            }
        }

        public void Shake(float d) { // call this when you want to shake the camera for some duration
            shakeDuration = d;
        }



        // queries
        public float YRotation { get { return YangleSmooth; } }



        // other

    }

}