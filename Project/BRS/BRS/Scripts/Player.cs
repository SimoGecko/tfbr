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
    class Player : Component {
        ////////// player class that allows the user to control the in game avatar //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        public int playerIndex = 0; // player index - to select input and camera

        //MOVEMENT
        float smoothMagnitude, refMagnitude;
        float maxSpeed = 7f;
        float minSpeed = 4f;
        float maxTurningRate = 360; // degrees/sec
        float rotation;
        float refangle, refangle2;
        float inputAngle; // store it
        float targetRotation;

        //MONEY
        int capacity = 10;
        public int carryingmoney = 0;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            transform.Rotate(Vector3.Up, -90);
            rotation = targetRotation = -90;
        }

        public override void Update() {
            MoveInput();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void MoveInput() {
            //based on index
            Vector3 input;
            if(playerIndex==0)
                input = new Vector3(Input.GetAxisRaw0("Horizontal"), 0, Input.GetAxisRaw0("Vertical"));
            else
                input = new Vector3(Input.GetAxisRaw1("Horizontal"), 0, Input.GetAxisRaw1("Vertical"));

            float magnitude = Utility.Clamp01(input.Length());
            smoothMagnitude = Utility.SmoothDamp(smoothMagnitude, magnitude, ref refMagnitude, .1f);

            //rotate towards desired angle
            if (smoothMagnitude > .05f) { // avoid changing if 0
                inputAngle = MathHelper.ToDegrees((float)Math.Atan2(input.Z, input.X)); 
                inputAngle = Utility.WrapAngle(inputAngle, targetRotation);
                targetRotation = Utility.SmoothDampAngle(targetRotation, inputAngle-90, ref refangle, .3f, maxTurningRate*smoothMagnitude);
            } else {
                targetRotation = Utility.SmoothDampAngle(targetRotation, rotation, ref refangle2, .3f, maxTurningRate*smoothMagnitude);
            }

            rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);
            transform.eulerAngles = new Vector3(0, rotation, 0);

            //move forward
            transform.Translate(Vector3.Forward * currentSpeed * smoothMagnitude * Time.deltatime);
        }

        public void CollectMoney(int amount) {
            carryingmoney += Math.Min(amount, capacity-carryingmoney);
            UserInterface.instance.SetPlayerMoneyPercent(MoneyPercent, playerIndex);
        }

        public void Deload() {
            carryingmoney = 0;
            UserInterface.instance.SetPlayerMoneyPercent(MoneyPercent, playerIndex);
        }


        // queries
        public float MoneyPercent { get { return (float)carryingmoney / capacity; } }
        public bool CanPickUp { get { return carryingmoney < capacity; } }

        float currentSpeed { get { return MathHelper.Lerp(maxSpeed, minSpeed, MoneyPercent); } }
        // other

    }

}