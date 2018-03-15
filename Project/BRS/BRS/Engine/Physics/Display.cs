using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {
    public class Display : DrawableGameComponent {
        private readonly ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font1, _font2;
        private Texture2D _texture;

        private int _frameRate;
        private int _frameCounter;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        private int _bbWidth, _bbHeight;
        private const int PaddingTop = 300;

        public List<string> DisplayText { set; get; }

        public Display(Game game)
            : base(game) {
            _content = game.Content;

            DisplayText = new List<string>();
            for (int i = 0; i < 25; i++) {
                DisplayText.Add(string.Empty);
            }
        }

        private void GraphicsDevice_DeviceReset(object sender, EventArgs e) {
            _bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            _bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        protected override void LoadContent() {
            GraphicsDevice.DeviceReset += GraphicsDevice_DeviceReset;
            GraphicsDevice_DeviceReset(null, null);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font1 = _content.Load<SpriteFont>("debugFont");
            _font2 = _content.Load<SpriteFont>("debugFont");

            _texture = _content.Load<Texture2D>("images\\icon");
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

        public override void Draw(GameTime gameTime) {
            _frameCounter++;

            string fps = _frameRate.ToString();

            _spriteBatch.Begin();

            _spriteBatch.Draw(_texture, new Rectangle(_bbWidth - 105, 5, 100, 91), Color.White);
            _spriteBatch.DrawString(_font1, fps, new Vector2(11, PaddingTop + 6), Color.Black);
            _spriteBatch.DrawString(_font1, fps, new Vector2(12, PaddingTop + 7), Color.Yellow);

            for (int i = 0; i < DisplayText.Count; i++) {
                if (!string.IsNullOrEmpty(DisplayText[i])) {
                    _spriteBatch.DrawString(_font2, DisplayText[i], new Vector2(11, PaddingTop + 40 + i * 20), Color.White);
                }
            }

            _spriteBatch.End();
        }
    }
}
