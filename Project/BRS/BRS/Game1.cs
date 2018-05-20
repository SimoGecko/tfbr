using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.PostProcessing;
using BRS.Engine.Rendering;
using BRS.Scripts;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS {

    public class Game1 : Game {
        //default - don't touch
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        const bool loadMenu = false;
        const bool showUI = true;

        // todo: for andy for debugging framerate => to be removed soon
        private SpriteFont _font;
        private int _frames = 0;

        public Game1() {
            //NOTE: don't add anything into constructor
            _graphics = new GraphicsDeviceManager(this) /*{ IsFullScreen = true }*/;
            Content.RootDirectory = "Content";
            File.content = Content;
            Graphics.gDM = _graphics;
            HardwareRendering.GraphicsDeviceManager = _graphics;

            //IsFixedTimeStep = false;
        }

        protected override void Initialize() {
            //NOTE: this is basic initialization of core components, nothing else
            Screen.InitialSetup(_graphics, this, GraphicsDevice); // setup screen and create cameras
            // init the post processing manager
            PostProcessingManager.Initialize();

            // Allow physics drawing for debug-reasons (display boundingboxes etc..)
            // Todo: can be removed in the final stage of the game, but not yet, since it's extremly helpful to visualize the physics world
            PhysicsDrawer.Initialize(this, GraphicsDevice);

            base.Initialize();
        }


        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            UserInterface.sB = _spriteBatch;
            Graphics.Start();
            HardwareRendering.Start();
            ParticleRendering.Start();
            //start other big components
            Input.Start();

            //load prefabs and scene
            Prefabs.Start();
            UserInterface.Start();
            GameMode.Start();
            SceneManager.Start();

            SceneManager.LoadScene(loadMenu? "LevelMenu" : "LevelGame");

            Audio.Start();


            PostProcessingManager.Instance.Start(_spriteBatch);

            // Todo: Andy removes as soon as he is 100% sure that framerate is not a problem anymore
            _font = File.Load<SpriteFont>("Other/font/debug");

            // add skybox
            //Skybox.Start();

        }


        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
            //Heatmap.instance.SaveHeatMap();
        }

        protected override void Update(GameTime gameTime) {
            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/ Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            base.Update(gameTime);
            Time.Update(gameTime);

            Input.Update();
            Audio.Update();
            SceneManager.Update(); // check for scene change (can remove later)

            GameManager.Update();

            foreach (GameObject go in GameObject.All) go.Update();
            foreach (GameObject go in GameObject.All) go.LateUpdate();

            PhysicsDrawer.Instance.Update(gameTime);
            PhysicsManager.Instance.Update(gameTime);

            // Instanciating
            HardwareRendering.Update();
            ParticleRendering.Update();
        }

        protected override void Draw(GameTime gameTime) {
            // render scene for real 
            GraphicsDevice.SetRenderTarget(PostProcessingManager.SceneTarget);
            GraphicsDevice.Clear(Graphics.SkyBlue);

            base.Draw(gameTime);
            _graphics.PreferMultiSampling = true;

            //-----3D-----
            GraphicsDevice.DepthStencilState = DepthStencilState.Default; // new DepthStencilState() { DepthBufferEnable = true }; // activates z buffer

            HardwareRendering.Draw();
            ParticleRendering.Draw();

            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;

                // Allow physics drawing for debug-reasons (display boundingboxes etc..)
                PhysicsDrawer.Instance.Draw(cam);

                foreach (GameObject go in GameObject.All) go.Draw3D(cam);


                ////gizmos
                //GraphicsDevice.RasterizerState = Screen._wireRasterizer;
                //Gizmos.DrawWire(cam);
                //GraphicsDevice.RasterizerState = Screen._fullRasterizer;
                //Gizmos.DrawFull(cam);
            }
            //Gizmos.ClearOrders();


            // draw everything 3D to get the depth info
            _graphics.GraphicsDevice.SetRenderTarget(PostProcessingManager.DepthTarget);
            _graphics.GraphicsDevice.Clear(Color.Black);

            HardwareRendering.DrawDepth();


            // apply post processing
            // PostProcessingManager.Instance.Draw(_renderTarget, _spriteBatch, GraphicsDevice, _ZBuffer);
            PostProcessingManager.Instance.Draw(_spriteBatch);

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);


            //-----2D-----
            int i = 0;
            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;
                _spriteBatch.Begin();
                if (showUI)
                    foreach (GameObject go in GameObject.All) go.Draw2D(i);
                _spriteBatch.End();
                i++;
            }

            GraphicsDevice.Viewport = Screen.FullViewport;
            if (showUI) {
                _spriteBatch.Begin();
                foreach (GameObject go in GameObject.All) go.Draw2D(-1);
                _spriteBatch.End();
            }

            try {
                string text = string.Format(
                    "Frames per second: {0}/{1}\n" +
                    "Instances: {2}\n",
                    (1.0f / gameTime.ElapsedGameTime.TotalSeconds).ToString("0.00"),
                    (_frames++ / gameTime.TotalGameTime.TotalSeconds).ToString("0.00"),
                    GameObject.All.Length);

                _spriteBatch.Begin();

                _spriteBatch.DrawString(_font, text, new Vector2(65, 265), Color.Black);
                _spriteBatch.DrawString(_font, text, new Vector2(64, 264), Color.White);

                _spriteBatch.End();
            } catch { }
        }
    }

}
