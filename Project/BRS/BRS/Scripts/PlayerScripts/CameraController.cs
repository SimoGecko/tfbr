﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Utilities;
using Microsoft.Xna.Framework;
using Curve = BRS.Engine.Utilities.Curve;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Sets the camera position and follows the player smoothly, also allowing rotation
    /// </summary>
    class CameraController : Component {

        // --------------------- VARIABLES ---------------------

        //public
        public int CamIndex;

        static readonly Vector3 Offset = new Vector3(0, 10, 10);
        static readonly Vector3 StartAngle = new Vector3(-45, 0, 0);
        static Vector2 _angleRange = new Vector2(-AngleVariation, AngleVariation); // -40, 40

        //private
        static Vector2 _mouseSensitivity = new Vector2(-.3f, -.3f); // set those (also with sign) into options menu
        static Vector2 _gamepadSensitivity = new Vector2(-2f, -2f);
        float _xAngle, _xAngleSmooth;
        float _yAngle, _yAngleSmooth;
        float _refVelocityX, _refVelocityY;
        Vector3 _targetPosRef;


        float _shakeDuration = 0;
        Vector3 _shake;

        // const
        private const float SmoothTime = .2f;
        private const float PositionSmoothTime = .1f;
        private const int AngleVariation = 5;
        private const float ShakeAmount = .0f;

        //reference
        private Transform _player;

        public CameraController() {
            _yAngle = 0;
        }


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _xAngle = _xAngleSmooth = _yAngle = _yAngleSmooth = _refVelocityX = _refVelocityY = 0;

            _player = GameObject.FindGameObjectWithName("player_" + CamIndex).transform;
            if (_player == null) {
                Debug.LogError("player not found");
                //return;
            }

            transform.position = _player.position + Offset;
            transform.eulerAngles = StartAngle;
        }

        public override void LateUpdate() { // after player has moved
            ProcessInput();
            FollowSmoothAndRotate();
            ProcessShake();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void ProcessInput() {
            float inputX = (Input.MouseDelta.X * _mouseSensitivity.X).Clamp(-20, 20); // clamp is to avoid initial weird jump in mouse delta // TODO FIX
            float inputY = (Input.MouseDelta.Y * _mouseSensitivity.Y).Clamp(-100, 100);

            inputX += Input.GetThumbstick(Input.Stick.Right, CamIndex).X * _gamepadSensitivity.X;
            inputY += Input.GetThumbstick(Input.Stick.Right, CamIndex).Y * _gamepadSensitivity.Y;

            _xAngle = (_xAngle + inputY).Clamp(_angleRange.X, _angleRange.Y);
            _xAngleSmooth = Utility.SmoothDamp(_xAngleSmooth, _xAngle, ref _refVelocityX, SmoothTime);

            _yAngle += inputX;
            _yAngleSmooth = Utility.SmoothDamp(_yAngleSmooth, _yAngle, ref _refVelocityY, SmoothTime);

        }

        void FollowSmoothAndRotate() {
            // Todo: used? yes
            Vector3 currentPosition = transform.position;

            transform.position = _player.position + Offset;
            transform.eulerAngles = StartAngle;

            transform.RotateAround(_player.position, Vector3.Up, _yAngleSmooth);
            transform.RotateAround(_player.position, transform.Right, _xAngleSmooth);

            // Todo: used? yes
            Vector3 targetPos = transform.position;
            //transform.position = Utility.SmoothDamp(currentPosition, targetPos, ref targetPosRef, positionSmoothTime);

        }

        void ProcessShake() {
            if (_shakeDuration > 0) {
                _shake = MyRandom.InsideUnitSphere() * ShakeAmount * Curve.EvaluateC(_shakeDuration);
                _shakeDuration -= Time.DeltaTime;
                transform.position += _shake;
            } else {
                _shake = Vector3.Zero;
                _shakeDuration = 0;
            }
        }

        public void Shake(float d) { // call this when you want to shake the camera for some duration
            _shakeDuration = d;
        }



        // queries
        public float YRotation { get { return _yAngleSmooth; } }



        // other

    }

}