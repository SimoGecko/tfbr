using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Menu;
using BRS.Scripts;

namespace BRS {

    public class Game1 : Game {

        //default - don't touch
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //@andy including these
        private Display _display;
        private DebugDrawer _debugDrawer; // these should also not exist
        private static bool _usePhysics = false;


        public Game1() {
            //NOTE: don't add anything into constructor
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            File.content = Content;
            Graphics.gD = GraphicsDevice;
        }

        protected override void Initialize() {
            //NOTE: this is basic initialization of core components, nothing else

            Screen.InitialSetup(_graphics, this, GraphicsDevice); // setup screen and create cameras

            //@andy remove this - hide everything inside PhysicsManager.Setup();
            _debugDrawer = new DebugDrawer(this);
            Components.Add(_debugDrawer);
            _display = new Display(this);
            Components.Add(_display);
            PhysicsManager.SetUpPhysics(_debugDrawer, _display, GraphicsDevice);

            base.Initialize();
        }


        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            new UserInterface();
            Screen.AdditionalSetup(_graphics);

            //load prefabs and scene
            Prefabs.Start();
            SceneManager.Start();
            if (_usePhysics)  SceneManager.Load("LevelPhysics"); // TODO make simple string to select level
            else SceneManager.Load("Level1");
            //_ui = new UserInterface();
            //_ui.Start();
           

            //everything is loaded, call Start
            Start();
        }

        public void Start() {
            //all the objects are present in memory but still don't hold references. Initialize variables and start

            UserInterface.Instance.Start();
            Input.Start();
            Audio.Start();

            //foreach (Camera cam in Screen.cameras) cam.Start(); // cameras are gameobjects
            foreach (GameObject go in GameObject.All) go.Awake();
            foreach (GameObject go in GameObject.All) go.Start();
        }


        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
            Heatmap.instance.SaveHeatMap();
        }

        protected override void Update(GameTime gameTime) {
            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/ Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            base.Update(gameTime);
            Time.Update(gameTime);

            Input.Update();


            Audio.Update();

            if (Input.GetKeyDown(Keys.D9)) {
                Debug.Log("changing scene...");
                SceneManager.Load("Level2");
                Screen.AdditionalSetup(_graphics);
                foreach (GameObject go in GameObject.All) go.Start();

            }
            if (Input.GetKeyDown(Keys.D0)) {
                Debug.Log("changing scene...");
                SceneManager.Load("Level1");
                Screen.AdditionalSetup(_graphics);
                foreach (GameObject go in GameObject.All) go.Start();

            }

            foreach (GameObject go in GameObject.All) go.Update();
            foreach (GameObject go in GameObject.All) go.LateUpdate();

            PhysicsManager.Instance.Update(gameTime);
            

        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            //-----3D-----
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true }; // activates z buffer

            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;

                PhysicsManager.Instance.Draw(cam); // why is this here??

                foreach (GameObject go in GameObject.All) go.Draw(cam);

                //gizmos
                GraphicsDevice.RasterizerState = Screen._wireRasterizer;
                Gizmos.DrawWire(cam);
                GraphicsDevice.RasterizerState = Screen._fullRasterizer;
                Gizmos.DrawFull(cam);
            }
            Gizmos.ClearOrders();





            //-----2D-----
            int i = 0;
            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;
                _spriteBatch.Begin();
                UserInterface.Instance.DrawSplitscreen(_spriteBatch, i++);
                _spriteBatch.End();
            }

            GraphicsDevice.Viewport = Screen.FullViewport;
            _spriteBatch.Begin();
            UserInterface.Instance.DrawGlobal(_spriteBatch);
            _spriteBatch.End();
        }
    }
}
