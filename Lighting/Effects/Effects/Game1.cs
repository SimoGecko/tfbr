using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Effects.MacOS
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D clockTexture;
        //Model car;
        VertexPositionTexture[] floorVerts;
        BasicEffect effect; 

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            floorVerts = new VertexPositionTexture[6];

            floorVerts[0].Position = new Vector3(-20, -20, 0);
            floorVerts[1].Position = new Vector3(-20, 20, 0);
            floorVerts[2].Position = new Vector3(20, -20, 0);

            floorVerts[3].Position = floorVerts[1].Position;
            floorVerts[4].Position = new Vector3(20, 20, 0);
            floorVerts[5].Position = floorVerts[2].Position;

            // Map texture coordinates
            floorVerts[0].TextureCoordinate = new Vector2(0, 0);
            floorVerts[1].TextureCoordinate = new Vector2(0, 1);
            floorVerts[2].TextureCoordinate = new Vector2(1, 0);

            floorVerts[3].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[4].TextureCoordinate = new Vector2(1, 1);
            floorVerts[5].TextureCoordinate = floorVerts[2].TextureCoordinate;

            effect = new BasicEffect(graphics.GraphicsDevice);

            base.Initialize();
        }

       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            using (var stream = TitleContainer.OpenStream("Content/clocks.png"))
            {
                clockTexture = Texture2D.FromStream(this.GraphicsDevice, stream);

             }

            // Load assets
           // car = Content.Load<Model>("Content/Audi.fbx");
        }

       
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.AliceBlue);

            spriteBatch.Begin();

            // Draw texture
            Vector2 topLeftOfSprite = new Vector2(50, 50);
            Color tintColor = Color.White;

            //spriteBatch.Draw(clockTexture, topLeftOfSprite, tintColor);

            spriteBatch.End();

            //foreach (var mesh in car.Meshes)
            //{

            // foreach (BasicEffect effect in mesh.Effects)
            //{

            //  effect.EnableDefaultLighting();
            // effect.PreferPerPixelLighting = true;
            //  effect.World = Matrix.Identity;
            //}


            // mesh.Draw();
            //}

            // Basic lighting
            //effect.EnableDefaultLighting();

            // Custorm lighting
            //effect.LightingEnabled = true;
            //effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0, 0);
            //effect.DirectionalLight0.Direction = new Vector3(-10, 0, -1);
            //effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);


            DrawGround();
            base.Draw(gameTime);
        }

        void DrawGround(){
            // Add ground
            var cameraPosition = new Vector3 (0, 40, 20);
            var cameraLookAtVector = Vector3.Zero;
            var cameraUpVector = Vector3.UnitZ;

            effect.View = Matrix.CreateLookAt (
            cameraPosition, cameraLookAtVector, cameraUpVector);

            float aspectRatio = 
                graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
            fieldOfView, aspectRatio, nearClipPlane, farClipPlane);


            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                 pass.Apply ();

                 graphics.GraphicsDevice.DrawUserPrimitives (
                    PrimitiveType.TriangleList, floorVerts,0,2);
            }

            // Add texture
            effect.TextureEnabled = true;
            effect.Texture = clockTexture;
        } 
    }
}
