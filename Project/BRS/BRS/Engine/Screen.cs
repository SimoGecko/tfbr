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
        ////////// also creates all the cameras that will be in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        public const int Width = 1920; // 1920x1080, 2560x1440
        public const int Height = 1080;
        public const string Title = "GAME TITLE";

        public static int SplitWidth, SplitHeight;

        //private
        public static Viewport FullViewport;
        static Viewport[] _splitViewport;

        //reference
        public static Camera[] Cameras;
        public static RasterizerState _fullRasterizer, _wireRasterizer;


        // --------------------- BASE METHODS ------------------
        public void Start() {

        }

        public void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public static void InitialSetup(GraphicsDeviceManager graphics, Game game, GraphicsDevice graphicsDevice) {
            //called just once at the beginning of the game
            SetupWindow(graphics, game);
            SetupRasterizers(graphicsDevice);
        }

        static void SetupWindow(GraphicsDeviceManager graphics, Game game) {
            //window size
            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.ApplyChanges();
            game.Window.Title = "New Title";
            game.IsMouseVisible = true;
        }

        static void SetupRasterizers(GraphicsDevice graphicsDevice) {
            _fullRasterizer = graphicsDevice.RasterizerState;
            _wireRasterizer = new RasterizerState();
            _wireRasterizer.FillMode = FillMode.WireFrame;
        }

        //----------------------

        public static void SetupViewportsAndCameras(GraphicsDeviceManager graphics, int numCameras) {
            Debug.Log("called with " + numCameras);
            //called on scene change to reset cameras and viewports
            SetupViewports(graphics, numCameras);
            SetupCameras(numCameras);
        }
        

        static void SetupViewports(GraphicsDeviceManager graphics, int numCameras) {
            //make viewports
            FullViewport = graphics.GraphicsDevice.Viewport;
            _splitViewport = new Viewport[numCameras];
            SplitWidth = Width; SplitHeight = Height;

            if (numCameras == 1) {
                _splitViewport[0] = new Viewport(0, 0, Width, Height, 0, 1);
            } else if (numCameras == 2) {
                SplitWidth = Width / 2;
                _splitViewport[0] = new Viewport(0, 0, Width / 2, Height, 0, 1);
                _splitViewport[1] = new Viewport(Width / 2, 0, Width / 2, Height, 0, 1);
            } else if (numCameras == 4) {
                int h2 = SplitHeight = Height / 2;
                int w2 = SplitWidth = Width / 2;
                _splitViewport[0] = new Viewport(0, 0, w2, h2, 0, 1);
                _splitViewport[1] = new Viewport(w2, 0, w2, h2, 0, 1);
                _splitViewport[2] = new Viewport(0, h2, w2, h2, 0, 1);
                _splitViewport[3] = new Viewport(w2, h2, w2, h2, 0, 1);
            }
        }

        static void SetupCameras(int numCameras) {
            if(Cameras!=null)
                for(int i=0; i<Cameras.Length; i++) {
                    GameObject.Destroy(Cameras[i].gameObject);
                }

            Cameras = new Camera[numCameras];

            for (int i = 0; i < numCameras; i++) {
                GameObject camObject = new GameObject("camera_" + i); // CREATES gameobject
                camObject.AddComponent(new Camera(i, _splitViewport[i]));
                Cameras[i] = camObject.GetComponent<Camera>();
            }
        }

        // queries
        static float AspectRatio {
            get { return (float)Width / Height; }
        }

        public static Rectangle Full {
            get { return new Rectangle(0, 0, Width, Height); }
        }
        public static Rectangle Split {
            get { return new Rectangle(0, 0, SplitWidth, SplitHeight); }
        }



        // other

    }

}