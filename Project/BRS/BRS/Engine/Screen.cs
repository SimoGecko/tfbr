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
        static int screenWidth = 1920;
        static int screenHeight = 900;
        static string screenTitle = "GAME TITLE";

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
            // Todo: Dynamically change this.. If we set it like this we are not able to write on the whole screen now.
            //graphics.PreferredBackBufferWidth = 1920 / 2;
            //graphics.PreferredBackBufferHeight = 1080 / 2;

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
                splitViewport[0] = new Viewport(0, 0, screenWidth, screenHeight, 0, 1);
            }else if (numPlayers == 2) {
                splitViewport[0] = new Viewport(0, 0, screenWidth/2, screenHeight, 0, 1);
                splitViewport[1] = new Viewport(screenWidth/2, 0, screenWidth/2, screenHeight, 0, 1);
            }else if (numPlayers == 4) {
                int h2 = screenHeight / 2;
                int w2 = screenWidth / 2;
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
            get { return (float)screenWidth / screenHeight; }
        }



        // other

    }

}