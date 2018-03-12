// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts;

namespace BRS {
    class Screen {
        ////////// deals with window issues, such as screen size, title, fullscreen and splitscreen //////////
        ////////// creates all the cameras that will be in the game //////////


        // --------------------- VARIABLES ---------------------

        //public
        public const int WIDTH = 1920;
        public const int HEIGHT = 1080;
        public const string TITLE = "GAME TITLE";

        //private
        public static Viewport fullViewport;
        static Viewport[] splitViewport;

        //reference
        public static Camera[] cameras;


        // --------------------- BASE METHODS ------------------
        public void Start() { }

        public void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public static void Setup(GraphicsDeviceManager graphics, Game game) {
            SetupWindow(graphics, game);
            SetupViewports(graphics);
            SetupCameras();
        }


        static void SetupWindow(GraphicsDeviceManager graphics, Game game) {
            //window size
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges(); // DO NOT COMMENT OUT THIS LINE - causes unhandled exception
            
            //game.Window.Title = "New Title";
            game.IsMouseVisible = true;
        }

        static void SetupViewports(GraphicsDeviceManager graphics) {
            int numPlayers = GameManager.numPlayers;
            //make viewports
            fullViewport = graphics.GraphicsDevice.Viewport;
            splitViewport = new Viewport[numPlayers];

            if (numPlayers == 1) {
                splitViewport[0] = new Viewport(0, 0, WIDTH, HEIGHT, 0, 1);
            }else if (numPlayers == 2) {
                splitViewport[0] = new Viewport(0, 0, WIDTH/2, HEIGHT, 0, 1);
                splitViewport[1] = new Viewport(WIDTH/2, 0, WIDTH/2, HEIGHT, 0, 1);
            }else if (numPlayers == 4) {
                int h2 = HEIGHT / 2;
                int w2 = WIDTH / 2;
                splitViewport[0] = new Viewport(0,  0, w2, h2, 0, 1);
                splitViewport[1] = new Viewport(w2, 0, w2, h2, 0, 1);
                splitViewport[2] = new Viewport(0, h2, w2, h2, 0, 1);
                splitViewport[3] = new Viewport(w2,h2, w2, h2, 0, 1);
            }
        }

        static void SetupCameras() {
            cameras = new Camera[GameManager.numPlayers];
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject camObject = new GameObject("camObject_" + i);
                camObject.AddComponent(new Camera(splitViewport[i]));
                camObject.AddComponent(new CameraController()); // TODO move out this creation code
                camObject.GetComponent<CameraController>().camIndex = i;
                cameras[i] = camObject.GetComponent<Camera>();
            }
        }


        // queries
        static float AspectRatio {
            get { return (float)WIDTH / HEIGHT; }
        }



        // other

    }

}