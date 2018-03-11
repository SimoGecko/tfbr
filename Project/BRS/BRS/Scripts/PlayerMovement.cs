﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerMovement : Component {
        ////////// Deals with the movement of the player around the map //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float minSpeed = 2f;
        const float maxSpeed = 4f;
        const float maxTurningRate = 360; // deg/sec
        const float boostSpeedMultiplier = 1.5f;

        //private
        float rotation;
        float smoothMagnitude, refMagnitude;
        float refangle, refangle2;
        float inputAngle;
        float targetRotation;

        //BOOST
        public bool boosting;

        //reference
        PlayerInventory playerInventory;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //transform.Rotate(Vector3.Up, -90);
            //rotation = targetRotation = -90;

            playerInventory = gameObject.GetComponent<PlayerInventory>();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Move(Vector3 input) {
            float magnitude = Utility.Clamp01(input.Length());
            smoothMagnitude = Utility.SmoothDamp(smoothMagnitude, magnitude, ref refMagnitude, .1f);

            //rotate towards desired angle
            if (smoothMagnitude > .05f) { // avoid changing if 0
                inputAngle = MathHelper.ToDegrees((float)Math.Atan2(input.Z, input.X));
                inputAngle = Utility.WrapAngle(inputAngle, targetRotation);
                targetRotation = Utility.SmoothDampAngle(targetRotation, inputAngle - 90, ref refangle, .3f, maxTurningRate * smoothMagnitude);
            } else {
                targetRotation = Utility.SmoothDampAngle(targetRotation, rotation, ref refangle2, .3f, maxTurningRate * smoothMagnitude);
            }

            rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);
            transform.eulerAngles = new Vector3(0, rotation, 0);

            //move forward
            float speedboost = boosting ? boostSpeedMultiplier : 1f;
            transform.Translate(Vector3.Forward * currentSpeed * speedboost * smoothMagnitude * Time.deltatime);
        }


        // queries
        float currentSpeed { get { return MathHelper.Lerp(maxSpeed, minSpeed, playerInventory.MoneyPercent); } }




        // other

    }

}