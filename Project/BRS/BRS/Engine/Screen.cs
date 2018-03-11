// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts;

namespace BRS {
    class Screen {
        ////////// deals with window issues, such as screen size, title, fullscreen and splitscreen //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        public static int WIDTH = 1920;
        public static int HEIGHT = 1080;
        public static string TITLE = "GAME TITLE";

        public static Viewport fullViewport;
        static Viewport[] splitViewport;

        //reference
        public static Camera[] cams;


        // --------------------- BASE METHODS ------------------
        public void Start() { }

        public void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public static void Setup(GraphicsDeviceManager graphics, Game game) {
            SetupWindow(graphics, game);
            SetupSplitScreen(graphics);
        }


        static void SetupWindow(GraphicsDeviceManager graphics, Game game) {
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();
            game.Window.Title = "New Title";
            game.IsMouseVisible = true;
        }

        static void SetupSplitScreen(GraphicsDeviceManager graphics) {
            fullViewport = graphics.GraphicsDevice.Viewport;
            int numPlayers = GameManager.numPlayers;
            //make viewports and setup cameras
            splitViewport = new Viewport[numPlayers];
            cams = new Camera[numPlayers];

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

            for(int i=0; i<numPlayers; i++) {
                cams[i] = new Camera(splitViewport[i]);
            }
           
        }


        // queries
        static float AspectRatio {
            get { return (float)WIDTH / HEIGHT; }
        }



        // other

    }

}