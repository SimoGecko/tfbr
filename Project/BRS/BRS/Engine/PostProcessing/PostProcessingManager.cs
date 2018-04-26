// (c) Alexander Lelidis 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Scripts.Managers;

namespace BRS.Engine.PostProcessing {

    // 
    public enum PostprocessingType {
        BlackAndWhite,
        Chromatic,
        Vignette,
        GaussianBlur,
        DepthOfField,
        ColorGrading,
        ShockWave
    }


    class PostProcessingManager {
        public static PostProcessingManager Instance { get; private set; }

        private List<PostProcessingEffect> _effects = new List<PostProcessingEffect>();
        private RenderTarget2D[] _renderTargets;
        private RenderTarget2D _blurTarget;
        private Texture2D testGrid;
        private bool DEBUG = false;
        private List<Texture2D> _lut = new List<Texture2D>();
        private int _currentLuT = 0;
        private int _maxLuT = 20;

        public static void Initialize(ContentManager content) {
            Instance = new PostProcessingManager(content);
        }

        private PostProcessingManager(ContentManager content) {
            foreach (PostprocessingType pType in Enum.GetValues(typeof(PostprocessingType))) {
                string fileName = pType.ToString();
                Effect ppShader = content.Load<Effect>("Effects/" + fileName);
                PostProcessingEffect ppEffect = new PostProcessingEffect(pType, 1, false, ppShader);

                ppEffect.SetParameter("players", (float) GameManager.NumPlayers);
                // Special parameters for some effects
                switch (pType) {
                    case PostprocessingType.GaussianBlur:
                        ppEffect.SetParameter("screenSize", new Vector2(Screen.Width, Screen.Height));
                        break;
                    case PostprocessingType.DepthOfField:
                        float nearClip = 0.3f;
                        float farClip = 1000.0f;
                        farClip = farClip / (farClip - nearClip);

                        ppEffect.SetParameter("Distance", 70.0f);
                        ppEffect.SetParameter("Range", 30.0f);
                        ppEffect.SetParameter("Near", nearClip);
                        ppEffect.SetParameter("Far", farClip);
                        break;
                    case PostprocessingType.ColorGrading:
                        ppEffect.SetParameter("Size", 16f);
                        ppEffect.SetParameter("SizeRoot", 4f);
                        for (var i = 0; i < _maxLuT; i++) {
                            _lut.Add(content.Load<Texture2D>("Images/lut/lut (" + i.ToString()+ ")"));
                        }
                        ppEffect.SetParameter("LUT", _lut[0]);
                        

                        break;
                    case PostprocessingType.ShockWave:
                        testGrid = content.Load<Texture2D>("Images/textures/Pixel_grid");
                        ppEffect.SetParameter("centerCoord", new Vector2(0.5f, 0.5f));
                        ppEffect.SetParameter("shockParams", new Vector3(10.0f, 0.8f, 0.1f));
                        break;
                }

                _effects.Add(ppEffect);
            }
        }

        public void Start(SpriteBatch spriteBatch) {
            _renderTargets = new RenderTarget2D[_effects.Count];

            for (int i = 0; i < _effects.Count; ++i) {
                RenderTarget2D effectTarget2D = new RenderTarget2D(
                    spriteBatch.GraphicsDevice,
                    Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                    Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);

                _renderTargets[i] = effectTarget2D;
            }
            _blurTarget = new RenderTarget2D(
                spriteBatch.GraphicsDevice,
                Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

        }

        public bool SetShaderParameter(PostprocessingType shader, string parameterName, Vector2 arg) {
            int index = _effects.FindIndex(x => x.Name == parameterName);
            if (index > 0) {
                _effects[index].SetParameter(parameterName, arg);
                return true;
            }
            return false;
        }

        public bool SetShaderStatus(string shader, bool active) {
            int index = _effects.FindIndex(x => x.Name == shader);
            if (index > 0) {
                _effects[index].Active = active;
                return true;
            }
            return false;
        }


        public void Update(GameTime gameTime) {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed) {
                // Do whatever you want here
                Vector2 centerCoord = new Vector2((float)mouseState.X / (float)Screen.Width, (float)mouseState.Y / (float)Screen.Height);
                _effects[6].SetParameter("centerCoord", centerCoord);
                _effects[6].SetParameter("startTime", (float)gameTime.TotalGameTime.TotalSeconds);
            }

            if (Input.GetKeyDown(Keys.F1)) {
                _effects[0].Active = !_effects[0].Active;
            }
            if (Input.GetKeyDown(Keys.F2)) {
                _effects[1].Active = !_effects[1].Active;
            }
            if (Input.GetKeyDown(Keys.F3)) {
                _effects[2].Active = !_effects[2].Active;
            }
            if (Input.GetKeyDown(Keys.F4)) {
                _effects[3].Active = !_effects[3].Active;
            }
            if (Input.GetKeyDown(Keys.F5)) {
                _effects[4].Active = !_effects[4].Active;
            }
            if (Input.GetKeyDown(Keys.F6)) {
                _effects[5].Active = !_effects[5].Active;
            }
            if (Input.GetKeyDown(Keys.F7)) {
                _effects[6].Active = !_effects[6].Active;
                _effects[6].SetParameter("startTime", (float)gameTime.TotalGameTime.TotalSeconds);
            }
            if (Input.GetKeyDown(Keys.PageUp)) {
                _effects[3].Passes = MathHelper.Clamp(_effects[3].Passes + 1, 1, 4);
            }
            if (Input.GetKeyDown(Keys.PageDown)) {
                _effects[3].Passes = MathHelper.Clamp(_effects[3].Passes - 1, 1, 4);
            }
            if (Input.GetKeyDown(Keys.OemPlus)) {
                _currentLuT = (_currentLuT + 1) % _maxLuT;
                _effects[5].SetParameter("LUT", _lut[_currentLuT]); ;
            }
        }

        public void Draw(RenderTarget2D renderTarget, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D depth1Texture, GameTime gameTime) {
            RenderTarget2D curTarget = renderTarget;

            // if dynamic props are needed
            foreach (var ppShader in _effects) {
                if (ppShader.Active) {
                    // TODO: actived the shader on per player basis
                    ppShader.SetParameter("active", new Vector4(1.0f, 0.0f, 1.0f, 0.0f));

                    if (ppShader.Type == PostprocessingType.DepthOfField) {

                        // set the target to the blur target
                        graphicsDevice.SetRenderTarget(_blurTarget);

                        // get the gaussian blur shader
                        PostProcessingEffect blurShader = _effects[(int)PostprocessingType.GaussianBlur];

                        // apply 2 blur passes
                        for (int i = 0; i < 2; i++) {
                            spriteBatch.Begin(SpriteSortMode.Immediate,
                                BlendState.AlphaBlend,
                                SamplerState.LinearClamp,
                                DepthStencilState.Default,
                                RasterizerState.CullNone);

                            blurShader.Effect.CurrentTechnique.Passes[0].Apply();
                            spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                            curTarget = _blurTarget;

                            spriteBatch.End();
                        }

                        graphicsDevice.SetRenderTarget(null);
                        // set the blurred scene and the depth map as parameter
                        ppShader.SetParameter("BlurScene", _blurTarget);
                        ppShader.SetParameter("D1M", depth1Texture);
                    }

                    if (ppShader.Type == PostprocessingType.ShockWave) {
                        ppShader.SetParameter("time", (float)gameTime.TotalGameTime.TotalSeconds);
                    }

                    

                    // Setup next render-target to apply next filter
                    RenderTarget2D nextTarget = _renderTargets[(int)ppShader.Type];
                    graphicsDevice.SetRenderTarget(nextTarget);

                    for (int i = 0; i < ppShader.Passes; i++) {
                        // apply post processing shader
                        spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            SamplerState.LinearClamp,
                            DepthStencilState.Default,
                            RasterizerState.CullNone);
                        ppShader.Effect.CurrentTechnique.Passes[0].Apply();
                        if (PostprocessingType.ShockWave == ppShader.Type && DEBUG) {
                            spriteBatch.Draw(testGrid, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                        } else {
                            spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                        }

                        spriteBatch.End();
                    }

                    graphicsDevice.SetRenderTarget(null);
                    curTarget = nextTarget;
                }
            }

            // draw to screen
            graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone);
            spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
            spriteBatch.End();

        }
    }
}
