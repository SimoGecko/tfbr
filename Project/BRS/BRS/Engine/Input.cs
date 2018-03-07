// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace BRS {
    static class Input {
        //Static class that provides easy access to input (Mouse, Keyboard, Gamepad) as well as vibration

        static KeyboardState kState, oldKstate;
        static MouseState mState, oldMstate;
        static GamePadState[] gState, oldGstate;

        static bool[] vibrating = new bool[4];
        static float[] timer = new float[4];

        public static void Start() {
            gState = oldGstate = new GamePadState[4];

            kState = Keyboard.GetState();
            mState = Mouse.GetState();
            for(int i=0; i<4; i++)
                gState[i] = GamePad.GetState(i);
        }

        public static void Update() {
            oldKstate = kState;
            oldMstate = mState;
            oldGstate = gState;

            kState = Keyboard.GetState();
            mState = Mouse.GetState();
            for (int i = 0; i < 4; i++)
                gState[i] = GamePad.GetState(i);

            //check for vibration stop
            for (int i = 0; i < 4; i++) {
                if (vibrating[i]) {
                    timer[i] -= Time.deltatime;
                    if(timer[i] <= 0) {
                        StopVibration(i);
                    }
                }
            }
        }



        //AXIS
        static bool LeftAxis()  { return kState.IsKeyDown(Keys.Left)  || kState.IsKeyDown(Keys.A);  }
        static bool RightAxis() { return kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D);  }
        static bool UpAxis()    { return kState.IsKeyDown(Keys.Up)    || kState.IsKeyDown(Keys.W);  }
        static bool DownAxis()  { return kState.IsKeyDown(Keys.Down)  || kState.IsKeyDown(Keys.S); }


        static internal float GetAxisRaw(string v) { // WASD, ARROWS and gamepad all work
            if (v == "Horizontal") {
                return LeftAxis() && !RightAxis() ? -1 : !LeftAxis() && RightAxis() ? 1 : 0 + GetThumbstick("Left").X;
            }
            if (v == "Vertical") {
                return DownAxis() && !UpAxis() ? -1 : !DownAxis() && UpAxis() ? 1 : 0 + GetThumbstick("Left").Y;
            }
            return 0f;
        }

        //CROSS PLATFORM (allows to have same input from gamepad and keyboard
        static internal float GetAxisRaw0(string v) { // WASD and gamepad 0
            if (v == "Horizontal") {
                return (GetKey(Keys.A) && !GetKey(Keys.D)) ? -1 : (!GetKey(Keys.A) && GetKey(Keys.D)) ? 1 : 0 + GetThumbstick("Left", 0).X;
            }
            if (v == "Vertical") {
                return (GetKey(Keys.S) && !GetKey(Keys.W)) ? -1 : (!GetKey(Keys.S) && GetKey(Keys.W)) ? 1 : 0 + GetThumbstick("Left", 0).Y;
            }
            return 0f;
        }

        static internal float GetAxisRaw1(string v) { // ARROWS and gamepad 1
            if (v == "Horizontal") {
                return (GetKey(Keys.Left) && !GetKey(Keys.Right)) ? -1 : (!GetKey(Keys.Left) && GetKey(Keys.Right)) ? 1 : 0 + GetThumbstick("Left", 1).X;
            }
            if (v == "Vertical") {
                return (GetKey(Keys.Down) && !GetKey(Keys.Up)) ? -1 : (!GetKey(Keys.Down) && GetKey(Keys.Up)) ? 1 : 0 + GetThumbstick("Left", 1).Y;
            }
            return 0f;
        }

        public static bool Fire1() {
            return GetKeyDown(Keys.Space) || GetButtonDown(Buttons.A);
        }

        //KEYS
        public static bool GetKey(Keys k)     { return kState.IsKeyDown(k); }
        public static bool GetKeyDown(Keys k) { return kState.IsKeyDown(k) && oldKstate.IsKeyUp(k); }
        public static bool GetKeyUp(Keys k)   { return kState.IsKeyUp(k)   && oldKstate.IsKeyDown(k); }


        //MOUSE
        public static Vector2 mousePosition { get { return new Vector2(mState.X, mState.Y); } }
        public static Vector2 mouseDelta    { get { return new Vector2(mState.X - oldMstate.X, mState.Y - oldMstate.Y); } }
        public static void setMousePosition(Vector2 v) {
            Mouse.SetPosition((int)v.X, (int)v.Y);
        }
        public static int mouseWheel      { get { return mState.ScrollWheelValue; } }
        public static int mouseWheelDelta { get { return mState.ScrollWheelValue-oldMstate.ScrollWheelValue; } }

        public static bool GetMouseButton(int index) {
            switch (index) {
                case 0: return mState.LeftButton   == ButtonState.Pressed;
                case 1: return mState.RightButton  == ButtonState.Pressed;
                case 2: return mState.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }


        //GAMEPAD
        //assume gState.IsConnected
        public static bool IsConnected(int i) {
            return GamePad.GetState(i).IsConnected;
        }

        //includes dpad -> see schematic
        public static bool GetButton    (Buttons b, int i = 0) { return gState[i].IsButtonDown(b); }
        public static bool GetButtonDown(Buttons b, int i = 0) { return gState[i].IsButtonDown(b) && oldGstate[i].IsButtonUp(b); }
        public static bool GetButtonUp  (Buttons b, int i = 0) { return gState[i].IsButtonUp(b)   && oldGstate[i].IsButtonDown(b); }

        static internal Vector2 GetThumbstick(string v, int i=0) {
            if (v == "Left")  { return gState[i].ThumbSticks.Left; }
            if (v == "Right") { return gState[i].ThumbSticks.Right; }
            return Vector2.Zero;
        }

        public static float GetTrigger(string v, int i = 0) {
            if (v == "Left")  { return gState[i].Triggers.Left; }
            if (v == "Right") { return gState[i].Triggers.Right; }
            return 0f;
        }

        static float amountDown = 0.5f; // how much to press to be considered down
        public static bool IsTriggerDown(string v, int i = 0) {
            if (v == "Left")  { return gState[i].Triggers.Left >=amountDown; }
            if (v == "Right") { return gState[i].Triggers.Right>=amountDown; }
            return false;
        }

        //vibration
        public static void Vibrate(float left, float right, float time, int i=0) {
            GamePad.SetVibration(i, left, right);
            vibrating[i] = true;
            timer[i] = time;
        }

        static void StopVibration(int i=0) {
            GamePad.SetVibration(i, 0f, 0f);
            vibrating[i] = false;
        }

    }
}
