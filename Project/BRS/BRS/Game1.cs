using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.PostProcessing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Scripts;

namespace BRS {

    public class Game1 : Game {

        //default - don't touch
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Render the scene to this target
        RenderTarget2D _renderTarget;



        public Game1() {
            //NOTE: don't add anything into constructor
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            File.content = Content;
            Graphics.gDM = _graphics;
        }

        protected override void Initialize() {
            PhysicsDrawer.Initialize(this, GraphicsDevice);
            //NOTE: this is basic initialization of core components, nothing else
            Screen.InitialSetup(_graphics, this, GraphicsDevice); // setup screen and create cameras
            
            // init the rendertarget with the graphics device
            _renderTarget = new RenderTarget2D(
                GraphicsDevice,
                Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            // set up the post processing manager
            PostProcessingManager.Initialize(Content);


            base.Initialize();
        }


        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            UserInterface.sB = _spriteBatch;

            //load prefabs and scene
            Prefabs.Start();
            SceneManager.Start();
            SceneManager.LoadScene("Level1");

            //start other big components
            UserInterface.Start();
            Input.Start();
            Audio.Start();
            PostProcessingManager.Instance.Start(_spriteBatch);
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

            PhysicsDrawer.Instance.Update(gameTime);
            PhysicsManager.Instance.Update(gameTime);
            PostProcessingManager.Instance.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            //-----3D-----
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true }; // activates z buffer

            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;

                // Allow physics drawing for debug-reasons (display boundingboxes etc..)
                // Todo: can be removed in the final stage of the game, but not yet, since it's extremly helpful to visualize the physics world
                PhysicsDrawer.Instance.Draw(cam);

                foreach (GameObject go in GameObject.All) go.Draw3D(cam);

                //gizmos
                GraphicsDevice.RasterizerState = Screen._wireRasterizer;
                Gizmos.DrawWire(cam);
                GraphicsDevice.RasterizerState = Screen._fullRasterizer;
                Gizmos.DrawFull(cam);
            }
            Gizmos.ClearOrders();

            // apply post processing
            PostProcessingManager.Instance.Draw(_renderTarget, _spriteBatch, GraphicsDevice);
            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);

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
