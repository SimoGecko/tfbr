// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine {
    ////////// Static class that provides easy access to input (Mouse, Keyboard, Gamepad) as well as vibration. //////////
    static class Input {

        public enum Axis { Horizontal, Vertical }
        public enum Stick { Right, Left, Up, Down }

        private static KeyboardState _kState, _oldKstate;
        private static MouseState _mState, _oldMstate;
        private static GamePadState[] _gState, _oldGstate;

        private static bool[] _vibrating = new bool[4];

        public static void Start() {
            _gState  = new GamePadState[4];
            _oldGstate = new GamePadState[4];

            _kState = Keyboard.GetState();
            _mState = Mouse.GetState();
            for(int i=0; i<4; i++)
                _gState[i] = GamePad.GetState(i);
        }

        public static void Update() {
            _oldKstate = _kState;
            _oldMstate = _mState;

            _kState = Keyboard.GetState();
            _mState = Mouse.GetState();

            for (int i = 0; i < 4; i++) {
                _oldGstate[i] = _gState[i];
                _gState[i] = GamePad.GetState(i);
            }

            //check for vibration stop
            /*
            for (int i = 0; i < 4; i++) {
                if (vibrating[i]) {
                    timer[i] -= Time.deltatime;
                    if(timer[i] <= 0) {
                        StopVibration(i);
                    }
                }
            }*/
        }

        //AXIS
        private static bool LeftAxis()  { return _kState.IsKeyDown(Keys.Left)  || _kState.IsKeyDown(Keys.A);  }
        private static bool RightAxis() { return _kState.IsKeyDown(Keys.Right) || _kState.IsKeyDown(Keys.D);  }
        private static bool UpAxis()    { return _kState.IsKeyDown(Keys.Up)    || _kState.IsKeyDown(Keys.W);  }
        private static bool DownAxis()  { return _kState.IsKeyDown(Keys.Down)  || _kState.IsKeyDown(Keys.S); }


        internal static float GetAxisRaw(Axis axis) { // WASD, ARROWS and gamepad all work
            if (axis == Axis.Horizontal) {
                return LeftAxis() && !RightAxis() ? -1 : !LeftAxis() && RightAxis() ? 1 : 0 + GetThumbstick(Stick.Left).X;
            }
            if (axis == Axis.Vertical) {
                return DownAxis() && !UpAxis() ? -1 : !DownAxis() && UpAxis() ? 1 : 0 + GetThumbstick(Stick.Left).Y;
            }
            return 0f;
        }

        //CROSS PLATFORM (allows to have same input from gamepad and keyboard)
        internal static float GetAxisRaw0(Axis axis) { // WASD and gamepad 0
            if (axis == Axis.Horizontal) {
                return (GetKey(Keys.A) && !GetKey(Keys.D)) ? -1 : (!GetKey(Keys.A) && GetKey(Keys.D)) ? 1 : 0 + GetThumbstick(Stick.Left, 0).X;
            }
            if (axis == Axis.Vertical) {
                return (GetKey(Keys.S) && !GetKey(Keys.W)) ? -1 : (!GetKey(Keys.S) && GetKey(Keys.W)) ? 1 : 0 + GetThumbstick(Stick.Left, 0).Y;
            }
            return 0f;
        }

        internal static float GetAxisRaw1(Axis axis) { // ARROWS and gamepad 1
            if (axis == Axis.Horizontal) {
                return (GetKey(Keys.Left) && !GetKey(Keys.Right)) ? -1 : (!GetKey(Keys.Left) && GetKey(Keys.Right)) ? 1 : 0 + GetThumbstick(Stick.Left, 1).X;
            }
            if (axis == Axis.Vertical) {
                return (GetKey(Keys.Down) && !GetKey(Keys.Up)) ? -1 : (!GetKey(Keys.Down) && GetKey(Keys.Up)) ? 1 : 0 + GetThumbstick(Stick.Left, 1).Y;
            }
            return 0f;
        }


        //KEYS
        public static bool GetKey(Keys k)     { return _kState.IsKeyDown(k); }
        public static bool GetKeyDown(Keys k) { return _kState.IsKeyDown(k) && _oldKstate.IsKeyUp(k); }
        public static bool GetKeyUp(Keys k)   { return _kState.IsKeyUp(k)   && _oldKstate.IsKeyDown(k); }


        //MOUSE
        public static Vector2 MousePosition { get { return new Vector2(_mState.X, _mState.Y); } }
        public static Vector2 MouseDelta    { get { return new Vector2(_mState.X - _oldMstate.X, _mState.Y - _oldMstate.Y); } }
        public static void SetMousePosition(Vector2 v) {
            Mouse.SetPosition((int)v.X, (int)v.Y);
        }
        public static int MouseWheel      { get { return _mState.ScrollWheelValue; } }
        public static int MouseWheelDelta { get { return _mState.ScrollWheelValue-_oldMstate.ScrollWheelValue; } }

        static bool GetMouseButton(int index, MouseState state) {
            switch (index) {
                case 0: return state.LeftButton   == ButtonState.Pressed;
                case 1: return state.RightButton  == ButtonState.Pressed;
                case 2: return state.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }
        public static bool GetMouseButton    (int index) { return  GetMouseButton(index, _mState); }
        public static bool GetMouseButtonDown(int index) { return  GetMouseButton(index, _mState) && !GetMouseButton(index, _oldMstate); }
        public static bool GetMouseButtonUp  (int index) { return !GetMouseButton(index, _mState) &&  GetMouseButton(index, _oldMstate); }


        //GAMEPAD
        //assume gState.IsConnected
        public static bool IsConnected(int i=0) {
            return GamePad.GetState(i).IsConnected;
        }

        //includes dpad -> see schematic
        public static bool GetButton    (Buttons b, int i = 0) { return  _gState[i].IsButtonDown(b); }
        public static bool GetButtonDown(Buttons b, int i = 0) { return  _gState[i].IsButtonDown(b) && !_oldGstate[i].IsButtonDown(b); }
        public static bool GetButtonUp  (Buttons b, int i = 0) { return !_gState[i].IsButtonDown(b) &&  _oldGstate[i].IsButtonDown(b); }

        internal static Vector2 GetThumbstick(Stick stick, int i=0) {
            if (stick == Stick.Left)  { return _gState[i].ThumbSticks.Left; }
            if (stick == Stick.Right) { return _gState[i].ThumbSticks.Right; }
            return Vector2.Zero;
        }

        public static float GetTrigger(Stick stick, int i = 0) {
            if (stick == Stick.Left)  { return _gState[i].Triggers.Left; }
            if (stick == Stick.Right) { return _gState[i].Triggers.Right; }
            return 0f;
        }

        /*
        const float amountDown = 0.5f; // how much to press to be considered down
        public static bool IsTriggerDown(string v, int i = 0) {
            if (v == "Left")  { return gState[i].Triggers.Left >=amountDown; }
            if (v == "Right") { return gState[i].Triggers.Right>=amountDown; }
            return false;
        }*/

        //VIBRATION
        public static void Vibrate(float amount, float time, int i = 0) {
            Vibrate(amount, amount, time, i);
        }
        public static void Vibrate(float left, float right, float time, int i=0) {
            GamePad.SetVibration(i, left, right);
            new Timer(time, () => StopVibration(i));
            _vibrating[i] = true;
        }

        private static void StopVibration(int i=0) {
            GamePad.SetVibration(i, 0f, 0f);
            _vibrating[i] = false;
        }

        public static bool IsVibrating(int i=0) {
            return _vibrating[i];
        }

    }
}
