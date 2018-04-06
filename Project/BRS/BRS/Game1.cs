using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Menu;

namespace BRS {

    //TODO organize

    public class Game1 : Game {

        //default
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;



        //all these should not be here
        private MenuManager _menuManager;
        public bool MenuDisplay = false;

        private Display _display;
        private DebugDrawer _debugDrawer; // these should also not exist
        private static bool _usePhysics = false;


        public Game1() {
            //NOTE: don't add anything into constructor
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            File.content = Content;
        }

        protected override void Initialize() {
            //NOTE: this is basic initialization of core components, nothing else

            Screen.Setup(_graphics, this, GraphicsDevice); // setup screen and create cameras

            //remove this
            _debugDrawer = new DebugDrawer(this);
            Components.Add(_debugDrawer);
            _display = new Display(this);
            Components.Add(_display);
            PhysicsManager.SetUpPhysics(_debugDrawer, _display, GraphicsDevice);

            base.Initialize();
        }


        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //this should also not be here
            //CREATE UI MANAGER
            if (MenuDisplay) {
                _menuManager = new MenuManager();
                _menuManager.LoadContent();
            } else {
                Screen.AdditionalSetupBasedOnNumPlayers(_graphics, this);
            }
            new UserInterface();


            //load prefabs and scene
            Prefabs.Start();
            SceneManager.Start();
            if (_usePhysics) {
                SceneManager.Load("LevelPhysics");
                //Scene = new LevelPhysics("levelPhysics");// PhysicsManager.Instance);
            } else {
                SceneManager.Load("Level1");
                //Scene = new Level1("Level1");// PhysicsManager.Instance);
            }

            //everything is loaded, call Start
            Start();

        }

        public void Start() {
            //all the objects are present in memory but still don't hold references. Initialize variables and start

            //if (!MenuDisplay)
            //Scene.Start();

            


            

            UserInterface.Instance.Start();

            //START
            //scene.Start();
            Input.Start();
            Audio.Start();

            //foreach (Camera cam in Screen.cameras) cam.Start();
            foreach (GameObject go in GameObject.All) go.Awake();
            foreach (GameObject go in GameObject.All) go.Start();
        }

        public void ScreenAdditionalSetup() {
            
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/ Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);

            if (MenuDisplay) {
                _menuManager.Update(); // this shouldn't be here -> put it in Userinterface.Update()
            }
            else {
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

            if (MenuDisplay) {
                _spriteBatch.Begin();
                UserInterface.Instance.DrawMenu(_spriteBatch);
                _spriteBatch.End();
            }
            else {
                //foreach camera
                int i = 0;
                foreach (Camera cam in Screen.Cameras) {
                    GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

                    _graphics.GraphicsDevice.Viewport = cam.Viewport;

                    PhysicsManager.Instance.Draw(cam); // why is this here

                    foreach (GameObject go in GameObject.All) go.Draw(cam);
                    //transform.Draw(camera);

                    //gizmos (wireframe)
                    GraphicsDevice.RasterizerState = Screen._wireRasterizer;
                    Gizmos.DrawWire(cam);
                    GraphicsDevice.RasterizerState = Screen._fullRasterizer;
                    Gizmos.DrawFull(cam);

                    //splitscreen UI
                    _spriteBatch.Begin();
                    UserInterface.Instance.DrawSplitscreen(_spriteBatch, i++);
                    _spriteBatch.End();
                }

                _graphics.GraphicsDevice.Viewport = Screen.FullViewport;

                //fullscreen UI
                _spriteBatch.Begin();
                UserInterface.Instance.DrawGlobal(_spriteBatch);
                _spriteBatch.End();

            }
            base.Draw(gameTime);
        }
    }
}
