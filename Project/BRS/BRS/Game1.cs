using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Scripts;
using BRS.Load;

namespace BRS {

    //TODO organize

    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Scene scene;
        UserInterface ui;

        private Display _display;
        private DebugDrawer _debugDrawer;
        private PhysicsManager _physicsManager;
        RasterizerState fullRasterizer, wireRasterizer;
        public static Game1 instance;

        private static bool _usePhysics = false;



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
            } else {
                scene = new Level1(_physicsManager);
            }

            ui = new UserInterface();


            Start(); // CALL HERE

        }

        public void Reset() {
            LoadContent();
        }

        public void Start() {
            //START
            Prefabs.Start();
            ui.Start();
            scene.Start();
            Input.Start();
            Audio.Start();

            //foreach (Camera cam in Screen.cameras) cam.Start();
            foreach (GameObject go in GameObject.All) go.Start();
        }


        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/ Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            Input.Update();


            foreach (GameObject go in GameObject.All) go.Update();
            foreach (GameObject go in GameObject.All) go.LateUpdate();

            if (_usePhysics) {
                _physicsManager.Update(gameTime);
            }

            Physics.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //foreach camera
            int i = 0;
            foreach (Camera cam in Screen.cameras) {
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

                graphics.GraphicsDevice.Viewport = cam.viewport;

                if (_usePhysics) {
                    _physicsManager.Draw(cam);
                }

                foreach (GameObject go in GameObject.All) {
                    go.Draw(cam);
                }
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


            base.Draw(gameTime);
        }
    }
}
