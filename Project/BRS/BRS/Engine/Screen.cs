// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    class Screen {
        ////////// deals with window issues, such as screen size, title, fullscreen and splitscreen //////////
        ////////// creates all the cameras that will be in the game //////////


        // --------------------- VARIABLES ---------------------

        //public
       // public const int Width = 1920; // 1920x1080, 2560x1440
        //public const int Height = 1080;
        public const int Width = 1366; // 1920x1080, 2560x1440
        public const int Height = 720;
        public const string Title = "GAME TITLE";

        public static int SplitWidth, SplitHeight;

        //private
        public static Viewport FullViewport;
        static Viewport[] _splitViewport;

        //reference
        public static Camera[] Cameras;


        // --------------------- BASE METHODS ------------------
        public void Start() { }

        public void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public static void Setup(GraphicsDeviceManager graphics, Game game) {
            SetupWindow(graphics, game);
            //SetupViewports(graphics);
            //SetupCameras();
        }

        public static void AdditionalSetup(GraphicsDeviceManager graphics, Game game) {
            //SetupWindow(graphics, game);
            SetupViewports(graphics);
            SetupCameras();
        }

        static void SetupWindow(GraphicsDeviceManager graphics, Game game) {
            //window size
            //graphics.PreferredBackBufferWidth = WIDTH;
            //graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();
            game.Window.Title = "New Title";
            game.IsMouseVisible = true;
        }

        static void SetupViewports(GraphicsDeviceManager graphics) {
            int numPlayers = GameManager.NumPlayers;
            //make viewports
            FullViewport = graphics.GraphicsDevice.Viewport;
            _splitViewport = new Viewport[numPlayers];
            SplitWidth = Width; SplitHeight = Height;

            if (numPlayers == 1) {
                _splitViewport[0] = new Viewport(0, 0, Width, Height, 0, 1);
            } else if (numPlayers == 2) {
                SplitWidth = Width / 2;
                _splitViewport[0] = new Viewport(0, 0, Width / 2, Height, 0, 1);
                _splitViewport[1] = new Viewport(Width / 2, 0, Width / 2, Height, 0, 1);
            } else if (numPlayers == 4) {
                int h2 = SplitHeight = Height / 2;
                int w2 = SplitWidth = Width / 2;
                _splitViewport[0] = new Viewport(0, 0, w2, h2, 0, 1);
                _splitViewport[1] = new Viewport(w2, 0, w2, h2, 0, 1);
                _splitViewport[2] = new Viewport(0, h2, w2, h2, 0, 1);
                _splitViewport[3] = new Viewport(w2, h2, w2, h2, 0, 1);
            }
        }

        static void SetupCameras() {
            Cameras = new Camera[GameManager.NumPlayers];

            for (int i = 0; i < GameManager.NumPlayers; i++) {
                GameObject camObject = new GameObject("camera_" + i);
                camObject.AddComponent(new Camera(_splitViewport[i]));
                camObject.AddComponent(new CameraController()); // TODO move out this creation code
                camObject.GetComponent<CameraController>().CamIndex = i;
                Cameras[i] = camObject.GetComponent<Camera>();
            }
        }

        // queries
        static float AspectRatio {
            get { return (float)Width / Height; }
        }



        // other

    }

}