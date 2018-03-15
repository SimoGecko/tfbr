using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Scripts;
using BRS.Load;

namespace BRS {

    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Scene scene;
        UserInterface ui;

        private Display _display;
        private DebugDrawer _debugDrawer;
        private PhysicsManager _physicsManager;
        RasterizerState fullRasterizer, wireRasterizer;
        


        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Screen.Setup(graphics, this); // setup screen and create cameras
        }

        protected override void Initialize() {
            _debugDrawer = new DebugDrawer(this);
            Components.Add(_debugDrawer);
            _display = new Display(this);
            Components.Add(_display);

            base.Initialize();

            fullRasterizer = GraphicsDevice.RasterizerState;
            wireRasterizer = new RasterizerState();
            wireRasterizer.FillMode = FillMode.WireFrame;
        }


        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _physicsManager = new PhysicsManager(_debugDrawer, _display, GraphicsDevice);

            scene = new LevelPhysics(this, _physicsManager);
            ui = new UserInterface();

            //LOAD
            Prefabs.GiveContent(Content); // do not put in initialize: Todo: Why?
            ui.GiveContent(Content);
            scene.GiveContent(Content);

            //START
            Prefabs.Start();
            ui.Start();
            scene.Start();
            Input.Start();

            foreach (Camera cam in Screen.cameras) cam.Start();

            foreach (GameObject go in GameObject.All) go.Start();

        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            Input.Update();

            _physicsManager.Update(gameTime);

            foreach (GameObject go in GameObject.All) go.Update();

            Physics.CheckOnCollisionEnter();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //foreach camera
            int i = 0;
            foreach (Camera cam in Screen.cameras) {
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

                graphics.GraphicsDevice.Viewport = cam.viewport;
                _physicsManager.Draw();
                foreach (GameObject go in GameObject.All) go.Draw(cam);
                //Transform.Draw(camera);

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

            base.Draw(gameTime);
        }
    }
}
