﻿using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Scripts;
using BRS.Load;
using BRS.Menu;

namespace BRS {

    //TODO organize

    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Scene scene;
        UserInterface ui;

        private Display _display;
        private DebugDrawer _debugDrawer;
        private PhysicsManager _physicsManager;
        RasterizerState fullRasterizer, wireRasterizer;
        public static Game1 instance;

        private static bool _usePhysics = false;

        MenuManager menuManager;
        public bool menuDisplay;

        public Game1() {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Screen.Setup(graphics, this); // setup screen and create cameras
            File.content = Content;
        }

        protected override void Initialize() {
            if (_usePhysics) {
                _debugDrawer = new DebugDrawer(this);
                Components.Add(_debugDrawer);
                _display = new Display(this);
                Components.Add(_display);
            }

            base.Initialize();

            fullRasterizer = GraphicsDevice.RasterizerState;
            wireRasterizer = new RasterizerState();
            wireRasterizer.FillMode = FillMode.WireFrame;
        }


        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (_usePhysics) {
                _physicsManager = new PhysicsManager(_debugDrawer, _display, GraphicsDevice);
                scene = new LevelPhysics(this, _physicsManager);
            }
            else {
                scene = new Level1(_physicsManager);
            }

            ui = new UserInterface();
            ui.Start();

            menuManager = new MenuManager();
            menuManager.LoadContent();
            menuDisplay = true;

            Start(); // CALL HERE

        }

        public void Reset() {
            LoadContent();
        }

        public void Start() {
            //START
            Prefabs.Start();
            //scene.Start();
            Input.Start();

            //foreach (Camera cam in Screen.cameras) cam.Start();
            foreach (GameObject go in GameObject.All) go.Start();
        }

        public void ScreenAdditionalSetup() {
            Screen.AdditionalSetup(graphics, this);
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/ Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);

            menuManager.Update();

            if (!menuDisplay) {

                Input.Update();


                foreach (GameObject go in GameObject.All) go.Update();
                foreach (GameObject go in GameObject.All) go.LateUpdate();

                if (_usePhysics) {
                    _physicsManager.Update(gameTime);
                }

                Physics.Update();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            ui.DrawMenu(spriteBatch);
            spriteBatch.End();

            if (!menuDisplay) {
                //foreach camera
                int i = 0;
                foreach (Camera cam in Screen.cameras) {
                    GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

                    graphics.GraphicsDevice.Viewport = cam.viewport;

                    if (_usePhysics) {
                        _physicsManager.Draw(cam);
                    }

                    foreach (GameObject go in GameObject.All) go.Draw(cam);
                    //transform.Draw(camera);

                    //gizmos (wireframe)
                    GraphicsDevice.RasterizerState = wireRasterizer;
                    Gizmos.Draw(cam);
                    GraphicsDevice.RasterizerState = fullRasterizer;

                    //splitscreen UI
                    spriteBatch.Begin();
                    ui.DrawSplitscreen(spriteBatch, i++);
                    spriteBatch.End();
                }
                Gizmos.ClearOrders();

                graphics.GraphicsDevice.Viewport = Screen.fullViewport;

                //fullscreen UI
                spriteBatch.Begin();
                ui.DrawGlobal(spriteBatch);
                spriteBatch.End();

            }
            base.Draw(gameTime);
        }
    }
}
