using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        RasterizerState fullRasterizer, wireRasterizer;
        

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Screen.Setup(graphics, this); // setup screen and create cameras
            
        }

        protected override void Initialize() {
            base.Initialize();

            fullRasterizer = GraphicsDevice.RasterizerState;
            wireRasterizer = new RasterizerState();
            wireRasterizer.FillMode = FillMode.WireFrame;
        }


        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            scene = new Level1();
            ui = new UserInterface();

            //LOAD
            Prefabs.GiveContent(Content); // do not put in initialize
            ui.GiveContent(Content);
            scene.GiveContent(Content);

            //START
            Prefabs.Start();
            ui.Start();
            scene.Start();
            Input.Start();

            foreach (Camera cam in Screen.cams) cam.Start();

            foreach (GameObject go in GameObject.All) go.Start();
            
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            
            Time.Update(gameTime);
            Input.Update();
            //camera.Update();

            foreach (GameObject go in GameObject.All) go.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //3D
            int i = 0;
            foreach(Camera cam in Screen.cams) {
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

                graphics.GraphicsDevice.Viewport = cam.viewport;
                foreach (GameObject go in GameObject.All) go.Draw(cam);
                //Transform.Draw(camera);

                //gizmos (wireframe)
                GraphicsDevice.RasterizerState = wireRasterizer;
                Gizmos.Draw(cam);
                GraphicsDevice.RasterizerState = fullRasterizer;


                spriteBatch.Begin();
                ui.DrawSplitscreen(spriteBatch, i++);
                spriteBatch.End();
            }
            Gizmos.ClearOrders();

            graphics.GraphicsDevice.Viewport = Screen.fullViewport;

            //2D
            spriteBatch.Begin();
            ui.DrawGlobal(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
