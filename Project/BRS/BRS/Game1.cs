using BRS.Engine;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Scripts;
using BRS.Load;
using BRS.Menu;

namespace BRS {

    //TODO organize

    public class Game1 : Game {
        public static Game1 Instance { get; set; }

        public Scene Scene;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        private UserInterface _ui;
        private Display _display;
        private DebugDrawer _debugDrawer;
        
        private RasterizerState _fullRasterizer, _wireRasterizer;


        private static bool _usePhysics = false;

        private MenuManager _menuManager;
        public bool MenuDisplay;

        public Game1() {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Screen.Setup(_graphics, this); // setup screen and create cameras
            File.content = Content;
        }

        protected override void Initialize() {
            _debugDrawer = new DebugDrawer(this);
            Components.Add(_debugDrawer);
            _display = new Display(this);
            Components.Add(_display);
            PhysicsManager.SetUpPhysics(_debugDrawer, _display, GraphicsDevice);

            base.Initialize();

            _fullRasterizer = GraphicsDevice.RasterizerState;
            _wireRasterizer = new RasterizerState();
            _wireRasterizer.FillMode = FillMode.WireFrame;
        }


        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            if (_usePhysics) {
                Scene = new LevelPhysics(PhysicsManager.Instance);
            } else {
                Scene = new Level1(PhysicsManager.Instance);
            }

            _ui = new UserInterface();
            _ui.Start();

            _menuManager = new MenuManager();
            _menuManager.LoadContent();
            MenuDisplay = true;

            Start(); // CALL HERE

        }

        public void Reset() {
            LoadContent();
        }

        public void Start() {
            //START
            Engine.Prefabs.Start();
            //scene.Start();
            Input.Start();
            Audio.Start();

            //foreach (Camera cam in Screen.cameras) cam.Start();
            foreach (GameObject go in GameObject.All) go.Start();
        }

        public void ScreenAdditionalSetup() {
            Screen.AdditionalSetup(_graphics, this);
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/ Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);

            _menuManager.Update();

            if (!MenuDisplay) {
                Input.Update();
                Audio.Update();

                foreach (GameObject go in GameObject.All) go.Update();
                foreach (GameObject go in GameObject.All) go.LateUpdate();

                PhysicsManager.Instance.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _ui.DrawMenu(_spriteBatch);
            _spriteBatch.End();

            if (!MenuDisplay) {
                //foreach camera
                int i = 0;
                foreach (Camera cam in Screen.Cameras) {
                    GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

                    _graphics.GraphicsDevice.Viewport = cam.Viewport;

                    PhysicsManager.Instance.Draw(cam);

                    foreach (GameObject go in GameObject.All) go.Draw(cam);
                    //transform.Draw(camera);

                    //gizmos (wireframe)
                    GraphicsDevice.RasterizerState = _wireRasterizer;
                    Gizmos.DrawWire(cam);
                    GraphicsDevice.RasterizerState = _fullRasterizer;
                    Gizmos.DrawFull(cam);

                    //splitscreen UI
                    _spriteBatch.Begin();
                    _ui.DrawSplitscreen(_spriteBatch, i++);
                    _spriteBatch.End();
                }
                Gizmos.ClearOrders();

                _graphics.GraphicsDevice.Viewport = Screen.FullViewport;

                //fullscreen UI
                _spriteBatch.Begin();
                _ui.DrawGlobal(_spriteBatch);
                _spriteBatch.End();

            }
            base.Draw(gameTime);
        }
    }
}
