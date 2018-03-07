﻿// (c) Simone Guggiari 2018
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
        static int screenWidth = 1200;
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
            graphics.PreferredBackBufferWidth = 1920 / 2;
            graphics.PreferredBackBufferHeight = 1080 / 2;
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
                cams[0] = new Camera(splitViewport[0], splitViewport[0].AspectRatio);
            }else if (numPlayers == 2) {
                splitViewport[0] = new Viewport(0, 0, screenWidth/2, screenHeight, 0, 1);
                splitViewport[1] = new Viewport(screenWidth/2, 0, screenWidth/2, screenHeight, 0, 1);
                cams[0] = new Camera(splitViewport[0], splitViewport[0].AspectRatio);
                cams[1] = new Camera(splitViewport[1], splitViewport[0].AspectRatio);
            }
        }


        // queries
        static float AspectRatio {
            get { return (float)screenWidth / screenHeight; }
        }



        // other

    }

}