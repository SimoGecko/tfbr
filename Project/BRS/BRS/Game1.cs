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

        //@andy remove these
        private Display _display;
        private DebugDrawer _debugDrawer;
        Model sceneModel;


        public Game1() {
            //NOTE: don't add anything into constructor
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            File.content = Content;
            Graphics.gDM = _graphics;
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
            UserInterface.sB = _spriteBatch;


            Graphics.texlightEffect = File.Load<Effect>("Other/shaders/colortexlightmap");
            Graphics.lightMap = File.Load<Texture2D>("Models/scenes/LightMap");
            Graphics.textureCol = File.Load<Texture2D>("Models/scenes/1");

            sceneModel = File.Load<Model>("Models/scenes/sceneNico2");

            //load prefabs and scene
            Prefabs.Start();
            SceneManager.Start();
            SceneManager.LoadScene("Level1");

            //start other big components
            UserInterface.Start();
            Input.Start();
            Audio.Start();
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
            SceneManager.Update(); // check for scene change (can remove later)

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
                PhysicsManager.Instance.Draw(cam);
                foreach (GameObject go in GameObject.All) go.Draw3D(cam);
                //gizmos
                GraphicsDevice.RasterizerState = Screen._wireRasterizer;
                Gizmos.DrawWire(cam);
                GraphicsDevice.RasterizerState = Screen._fullRasterizer;
                Gizmos.DrawFull(cam);

                Graphics.DrawModelWithEffect(sceneModel, cam.View, cam.Proj, Matrix.CreateTranslation(new Vector3(0, .1f, 0)));

            }
            Gizmos.ClearOrders();


            //-----2D-----
            int i = 1;
            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;
                _spriteBatch.Begin();
                foreach (GameObject go in GameObject.All) go.Draw2D(i);
                _spriteBatch.End();
                i++;
            }

            GraphicsDevice.Viewport = Screen.FullViewport;
            _spriteBatch.Begin();
            foreach (GameObject go in GameObject.All) go.Draw2D(0);
            _spriteBatch.End();
        }
    }
}
