// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class GamepadTest : Component {
        ////////// test out gamepad functions //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        List<Buttons> allButtons;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            allButtons = new List<Buttons>(); allButtons.Clear();

            allButtons.Add(Buttons.A);
            allButtons.Add(Buttons.B);
            allButtons.Add(Buttons.X);
            allButtons.Add(Buttons.Y);

            allButtons.Add(Buttons.DPadUp);
            allButtons.Add(Buttons.DPadDown);
            allButtons.Add(Buttons.DPadLeft);
            allButtons.Add(Buttons.DPadRight);

            allButtons.Add(Buttons.Back);
            allButtons.Add(Buttons.Start);
            allButtons.Add(Buttons.BigButton);

            allButtons.Add(Buttons.LeftShoulder);
            allButtons.Add(Buttons.RightShoulder);
            allButtons.Add(Buttons.LeftTrigger);
            allButtons.Add(Buttons.RightTrigger);

            allButtons.Add(Buttons.LeftStick);
            allButtons.Add(Buttons.RightStick);

            /*
            allButtons.Add(Buttons.LeftThumbstickDown);
            allButtons.Add(Buttons.LeftThumbstickUp);
            allButtons.Add(Buttons.LeftThumbstickLeft);
            allButtons.Add(Buttons.LeftThumbstickRight);

            allButtons.Add(Buttons.RightThumbstickDown);
            allButtons.Add(Buttons.RightThumbstickUp);
            allButtons.Add(Buttons.RightThumbstickLeft);
            allButtons.Add(Buttons.RightThumbstickRight);
            */
        }

        public override void Update() {
            //TestInput();

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void TestInput() {
            foreach (Buttons b in allButtons) {
                if (Input.GetButtonDown(b)) Debug.Log("down " + b.ToString()); // don't work
                if (Input.GetButtonUp(b)) Debug.Log("up " + b.ToString()); // don't work
                if (Input.GetButton(b)) Debug.Log("press " + b.ToString());
            }

            if (Input.GetThumbstick("Left").LengthSquared() > 0) Debug.Log("L " + Input.GetThumbstick("Left").ToString());
            if (Input.GetThumbstick("Right").LengthSquared() > 0) Debug.Log("R " + Input.GetThumbstick("Right").ToString());

            if (Input.GetTrigger("Left") > 0) Debug.Log("TL " + Input.GetTrigger("Left"));
            if (Input.GetTrigger("Right") > 0) Debug.Log("TR " + Input.GetTrigger("Right"));
        }


        // queries



        // other

    }

}