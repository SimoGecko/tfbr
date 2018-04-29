// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Jitter;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {
    /// <summary>
    /// Display some states about the physics engine to debug the performance.
    /// </summary>
    public class Display : DrawableGameComponent {

        #region Properties and attributes

        // Reference to the content to load the needed assets
        private readonly ContentManager _content;

        // Sprite batch to draw on
        private SpriteBatch _spriteBatch;

        // Fonts used for the texts
        private SpriteFont _font1, _font2;

        // Logo of the game-lab
        private Texture2D _texture;

        // Accumulated update time to check if enough time passed to have accurate numbers
        private float _accUpdateTime;
        private int _frameRate;
        private int _frameCounter;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        private int _bbWidth;
        private const int PaddingTop = 300;

        private readonly List<string> _displayText;

        private bool DoDrawings => PhysicsDrawer.Instance?.DoDrawings ?? false;

        #endregion

        public Display(Game game)
            : base(game) {
            _content = game.Content;

            _displayText = new List<string>();
            for (int i = 0; i < 25; i++) {
                _displayText.Add(string.Empty);
            }
        }

        private void GraphicsDevice_DeviceReset(object sender, EventArgs e) {
            _bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            //_bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        protected override void LoadContent() {
            GraphicsDevice.DeviceReset += GraphicsDevice_DeviceReset;
            GraphicsDevice_DeviceReset(null, null);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font1 = File.Load<SpriteFont>("Other/font/debugFont");
            _font2 = File.Load<SpriteFont>("Other/font/debugFont");

            _texture = File.Load<Texture2D>("Images/logos/gamelab");
        }

        protected override void UnloadContent() {
            _content.Unload();
        }

        public override void Update(GameTime gameTime) {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1)) {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
        }

        public void UpdatePhysicsInformation(GameTime gameTime, World world) {
            _accUpdateTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_accUpdateTime < 0.1f) return;

            _accUpdateTime = 0.0f;

            int contactCount = 0;
            int activeBodies = 0;

            // Count contacts
            foreach (Arbiter ar in world.ArbiterMap.Arbiters) {
                contactCount += ar.ContactList.Count;
            }

            // Count active bodies
            foreach (RigidBody body in world.RigidBodies) {
                if (body.IsActive) activeBodies++;
            }

            _displayText[1] = world.CollisionSystem.ToString();
            _displayText[2] = "Arbitercount: " + world.ArbiterMap.Arbiters.Count + ";" + " Contactcount: " + contactCount;
            _displayText[3] = "Islandcount: " + world.Islands.Count;
            _displayText[4] = "Bodycount: " + world.RigidBodies.Count + " (" + activeBodies + ")";
            _displayText[6] = "gen0: " + GC.CollectionCount(0)
                + "  gen1: " + GC.CollectionCount(1)
                + "  gen2: " + GC.CollectionCount(2);

            int entries = (int)World.DebugType.Num;
            double total = 0;

            for (int i = 0; i < entries; i++) {
                World.DebugType type = (World.DebugType)i;

                _displayText[8 + i] = type + ": " + world.DebugTimes[i].ToString("0.00");

                total += world.DebugTimes[i];
            }

            _displayText[8 + entries] = "------------------------------";
            _displayText[9 + entries] = "Total Physics Time: " + total.ToString("0.00");

            float frameRate = (float)(1000.0d / total);
            _displayText[10 + entries] = "Physics Framerate: " + (float.IsInfinity(frameRate) ? "-" : frameRate.ToString("0")) + " fps";
        }

        public override void Draw(GameTime gameTime) {
            _frameCounter++;

            if (!DoDrawings) {
                return;
            }

            string fps = _frameRate.ToString();

            _spriteBatch.Begin();

            _spriteBatch.Draw(_texture, new Rectangle(_bbWidth - 105, 5, 100, 91), Color.White);
            _spriteBatch.DrawString(_font1, fps, new Vector2(11, PaddingTop + 6), Color.Black);
            _spriteBatch.DrawString(_font1, fps, new Vector2(12, PaddingTop + 7), Color.Yellow);

            for (int i = 0; i < _displayText.Count; i++) {
                if (!string.IsNullOrEmpty(_displayText[i])) {
                    _spriteBatch.DrawString(_font2, _displayText[i], new Vector2(11, PaddingTop + 40 + i * 20), Color.Black);
                }
            }

            _spriteBatch.End();
        }
    }
}
