using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.PostProcessing;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Scripts;
using System;
using System.Collections.Generic;
using BRS.Scripts.Managers;

namespace BRS {

    public class Game1 : Game {
        //default - don't touch
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Render the scene to this target
        RenderTarget2D _renderTarget;
        // depth info
        RenderTarget2D _ZBuffer;
        Effect _ZBufferShader;
        const string startScene = "LevelMenu";
        bool showUI = true;

        public Game1() {
            //NOTE: don't add anything into constructor
            _graphics = new GraphicsDeviceManager(this) { IsFullScreen = true };
            Content.RootDirectory = "Content";
            File.content = Content;
            Graphics.gDM = _graphics;
        }

        protected override void Initialize() {
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


            _ZBuffer = new RenderTarget2D(
                GraphicsDevice,
                Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            // set up the post processing manager
            List<PostprocessingType> defaultEffects = new List<PostprocessingType> { PostprocessingType.Chromatic, PostprocessingType.ColorGrading, PostprocessingType.Vignette, PostprocessingType.TwoPassBlur };
            PostProcessingManager.Initialize(defaultEffects);

            // Allow physics drawing for debug-reasons (display boundingboxes etc..)
            // Todo: can be removed in the final stage of the game, but not yet, since it's extremly helpful to visualize the physics world
            PhysicsDrawer.Initialize(this, GraphicsDevice);

            // Todo: can be removed for alpha-release
            PoliceManager.IsActive = true;
            LenseFlareManager.IsActive = true;
            ParticleSystem3D.IsActive = true;
            Skybox.IsActive = false;

            base.Initialize();
        }


        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            UserInterface.sB = _spriteBatch;
            Graphics.Start();
            //start other big components
            Input.Start();

            //load prefabs and scene
            Prefabs.Start();
            UserInterface.Start();
            GameMode.Start();
            SceneManager.Start();

#if DEBUG
            SceneManager.LoadScene("LevelMenu");
#else
            SceneManager.LoadScene("LevelMenu");
#endif

            Audio.Start();


            PostProcessingManager.Instance.Start(_spriteBatch);

            // load the z buffer shader
            _ZBufferShader = File.Load<Effect>("Effects/Depth");

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

            // Todo: Switch for the interim
            if (Input.GetKeyDown(Keys.Tab)) {
                ParticleSystem3D.IsActive = !ParticleSystem3D.IsActive;
                LenseFlareManager.IsActive = !LenseFlareManager.IsActive;
            }

            foreach (GameObject go in GameObject.All) go.Update();
            foreach (GameObject go in GameObject.All) go.LateUpdate();

            PhysicsDrawer.Instance.Update(gameTime);
            PhysicsManager.Instance.Update(gameTime);
            PostProcessingManager.Instance.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            // render scene for real 
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Graphics.SkyBlue);

            base.Draw(gameTime);

            //-----3D-----
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true }; // activates z buffer
            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;

                GraphicsDevice.RasterizerState = Screen._nocullRasterizer;
                //Skybox.Draw(cam);
                GraphicsDevice.RasterizerState = Screen._fullRasterizer;

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


            // Todo: For now disabled because it screwed up all shadows and lights etc...
            //// draw everything 3 D to get the depth info 
            //_graphics.GraphicsDevice.SetRenderTarget(_ZBuffer);
            //_graphics.GraphicsDevice.Clear(Color.Black);

            //GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true }; // activates z buffer
            //foreach (Camera cam in Screen.Cameras) {
            //    GraphicsDevice.Viewport = cam.Viewport;

            //    foreach (GameObject go in GameObject.All) go.Draw3DDepth(cam, _ZBufferShader);
            //}
            


            // apply post processing
            // PostProcessingManager.Instance.Draw(_renderTarget, _spriteBatch, GraphicsDevice, _ZBuffer);
            PostProcessingManager.Instance.Draw(_renderTarget, _spriteBatch, GraphicsDevice, _ZBuffer, gameTime);

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);


            //-----2D-----
            int i = 1;
            foreach (Camera cam in Screen.Cameras) {
                GraphicsDevice.Viewport = cam.Viewport;
                _spriteBatch.Begin();
                if(showUI)
                    foreach (GameObject go in GameObject.All) go.Draw2D(i);
                _spriteBatch.End();
                i++;
            }

            GraphicsDevice.Viewport = Screen.FullViewport;
            _spriteBatch.Begin();
            if(showUI)
                foreach (GameObject go in GameObject.All) go.Draw2D(0);
            _spriteBatch.End();
        }
    }

}
