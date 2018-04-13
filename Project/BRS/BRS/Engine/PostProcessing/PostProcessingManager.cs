// (c) Alexander Lelidis 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine.PostProcessing {

    public enum PostprocessingType {
        BlackAndWhite,
        Chromatic,
        Vignette,
        GaussianBlur
    }


    class PostProcessingManager {
        public static PostProcessingManager Instance { get; private set; }

        private List<PostProcessingEffect> _effects = new List<PostProcessingEffect>();
        private RenderTarget2D[] _renderTargets;


        public static void Initialize(ContentManager content) {
            Instance = new PostProcessingManager(content);
        }

        private PostProcessingManager(ContentManager content) {
            foreach (PostprocessingType pType in Enum.GetValues(typeof(PostprocessingType))) {
                string fileName = pType.ToString();
                Effect ppShader = content.Load<Effect>("Effects/" + fileName);
                PostProcessingEffect ppEffect = new PostProcessingEffect(pType, 1, false, ppShader);

                // Special parameters for some effects
                switch (pType) {
                    case PostprocessingType.GaussianBlur:
                        ppEffect.SetParameter("screenSize", new Vector2(Screen.Width, Screen.Height));
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
            if (Input.GetKeyDown(Keys.D1)) {
                _effects[0].Active = !_effects[0].Active;
            }
            if (Input.GetKeyDown(Keys.D2)) {
                _effects[1].Active = !_effects[1].Active;
            }
            if (Input.GetKeyDown(Keys.D3)) {
                _effects[2].Active = !_effects[2].Active;
            }
            if (Input.GetKeyDown(Keys.D4)) {
                _effects[3].Active = !_effects[3].Active;
            }
            if (Input.GetKeyDown(Keys.PageUp)) {
                _effects[3].Passes = MathHelper.Clamp(_effects[3].Passes + 1, 1, 4);
            }
            if (Input.GetKeyDown(Keys.PageDown)) {
                _effects[3].Passes = MathHelper.Clamp(_effects[3].Passes - 1, 1, 4);
            }
        }

        public void Draw(RenderTarget2D renderTarget, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) {
            RenderTarget2D curTarget = renderTarget;

            // if dynamic props are needed
            foreach (var ppShader in _effects) {
                if (ppShader.Active) {
                    // Setup next render-target to apply next filter
                    RenderTarget2D nextTarget = _renderTargets[(int) ppShader.Type];
                    graphicsDevice.SetRenderTarget(nextTarget);

                    for (int i = 0; i < ppShader.Passes; i++) {
                        // apply post processing shader
                        spriteBatch.Begin(SpriteSortMode.Immediate,
                                BlendState.AlphaBlend,
                                SamplerState.LinearClamp,
                                DepthStencilState.Default,
                                RasterizerState.CullNone);
                        ppShader.Effect.CurrentTechnique.Passes[0].Apply();
                        spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                        spriteBatch.End();
                    }

                    graphicsDevice.SetRenderTarget(null);
                    //curTarget.Dispose();
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
