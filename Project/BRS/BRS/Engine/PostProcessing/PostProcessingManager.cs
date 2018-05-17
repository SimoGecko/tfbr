// (c) Alexander Lelidis 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine.PostProcessing {

    // 
    public enum PostprocessingType {
        BlackAndWhite,
        Chromatic,
        Vignette,
        GaussianBlur,
        DepthOfField,
        ColorGrading,
        ShockWave,
        Wave,
        TwoPassBlur
    }


    class PostProcessingManager {
        public static PostProcessingManager Instance { get; private set; }

        private static int MaxEffects = 20;

        private readonly List<PostProcessingEffect> _effects = new List<PostProcessingEffect>();
        private readonly Dictionary<PostprocessingType, Effect> _loadedEffects = new Dictionary<PostprocessingType, Effect>();
        private readonly Dictionary<PostprocessingType, bool> _fixEffects = new Dictionary<PostprocessingType, bool>();
        private readonly PostProcessingEffect _twoPassEffect;
        private RenderTarget2D _blurTarget;
        private List<RenderTarget2D> _renderTargets = new List<RenderTarget2D>();
        private float _distance = 6f;
        private float _range = 16.5f;

        public static void Initialize(List<PostprocessingType> initialized) {
            Instance = new PostProcessingManager(initialized);
        }

        private PostProcessingManager(List<PostprocessingType> initialized) {
            foreach (PostprocessingType pType in Enum.GetValues(typeof(PostprocessingType))) {
                string fileName = pType.ToString();
                _loadedEffects[pType] = File.Load<Effect>("Effects/" + fileName);
            }


            foreach (PostprocessingType pType in initialized) {
                PostProcessingEffect ppEffect = new PostProcessingEffect(pType, 1, false, _loadedEffects[pType]);

                _fixEffects[pType] = true;

                ppEffect.SetParameter("players", (float)GameManager.NumPlayers);
                // Special parameters for some effects
                switch (pType) {
                    case PostprocessingType.BlackAndWhite:
                        ppEffect.SetParameter("durationFadeIn", 0.1f);
                        ppEffect.SetParameter("durationFadeOut", 1.9f);
                        ppEffect.SetParameter("max", 1.0f);
                        ppEffect.SetParameter("min", 0.1f);
                        break;

                    case PostprocessingType.GaussianBlur:
                        ppEffect.SetParameter("screenSize", new Vector2(Screen.Width, Screen.Height));
                        break;

                    case PostprocessingType.TwoPassBlur:
                        ppEffect.Passes = 10;
                        ppEffect.SetParameter("screenSize", new Vector2(Screen.Width, Screen.Height));
                        break;

                    case PostprocessingType.DepthOfField:
                        float nearClip = Camera.Near;
                        float farClip = Camera.FarDepth;
                        farClip = farClip / (farClip - nearClip);

                        ppEffect.SetParameter("Distance", _distance);
                        ppEffect.SetParameter("Range", _range);
                        ppEffect.SetParameter("Near", nearClip);
                        ppEffect.SetParameter("Far", farClip);

                        PostProcessingEffect ppBlur = new PostProcessingEffect(PostprocessingType.TwoPassBlur, 1, false, _loadedEffects[PostprocessingType.TwoPassBlur]);
                        ppBlur.SetParameter("players", (float)GameManager.NumPlayers);
                        ppBlur.SetParameter("screenSize", new Vector2(Screen.Width, Screen.Height));
                        ppBlur.SetParameter("active", new Vector4(1, 1, 1, 1));

                        for (var i = 0; i < GameManager.NumPlayers; i++) {
                            ppEffect.Activate(i, true);
                            ppBlur.Activate(i, true);
                        }
                        _twoPassEffect = ppBlur;

                        break;

                    case PostprocessingType.Chromatic:
                        for (var i = 0; i < GameManager.NumPlayers; i++) {
                            ppEffect.Activate(i, true);
                        }
                        break;

                    case PostprocessingType.Vignette:
                        for (var i = 0; i < GameManager.NumPlayers; i++) {
                            ppEffect.Activate(i, true);
                        }
                        break;

                    case PostprocessingType.ColorGrading:
                        ppEffect.SetParameter("Size", 16f);
                        ppEffect.SetParameter("SizeRoot", 4f);
                        
                        ppEffect.SetParameter("LUT", File.Load<Texture2D>("Images/lut/lut (1)"));

                        for (var i = 0; i < GameManager.NumPlayers; i++) {
                            ppEffect.Activate(i, true);
                        }
                        break;

                    case PostprocessingType.ShockWave:
                        ppEffect.SetParameter("centerCoord", new Vector2(0.5f, 0.5f));
                        ppEffect.SetParameter("shockParams", new Vector3(10.0f, 0.8f, 0.1f));
                        break;

                    case PostprocessingType.Wave:
                        ppEffect.SetParameter("centerCoord", new Vector2(0.5f, 0.5f));
                        ppEffect.SetParameter("shockParams", new Vector3(10.0f, 0.8f, 0.1f));
                        break;
                }

                _effects.Add(ppEffect);
            }
        }

        public void Start(SpriteBatch spriteBatch) {
            for (int i = 0; i < MaxEffects; ++i) {
                _renderTargets.Add(new RenderTarget2D(
                    spriteBatch.GraphicsDevice,
                    Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                    Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24));
            }

            _blurTarget = new RenderTarget2D(
                spriteBatch.GraphicsDevice,
                Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        public void SetShaderStatus(PostprocessingType shader, int playerId, bool active) {
            foreach (PostProcessingEffect postProcessingEffect in _effects) {
                if (postProcessingEffect.Type == shader) {
                    postProcessingEffect.Activate(playerId, active);
                }
            }
        }

        private bool GetShaderState(PostprocessingType shader) {
            foreach (PostProcessingEffect postProcessingEffect in _effects) {
                if (postProcessingEffect.Type == shader) {
                    return postProcessingEffect.IsActive();
                }
            }

            return false;
        }

        public void RemoveShader(PostprocessingType shader) {
            if (_fixEffects.ContainsKey(shader) && _fixEffects[shader]) {
                return;
            }

            _effects.RemoveAll(item => item.Type == shader);
        }


        /// <summary>
        /// Activate the black and white filter for a given player.
        /// Important: parameters about duration are set in the shader-initialization.
        /// </summary>
        /// <param name="playerId">Id of the player to apply the shader</param>
        /// <param name="deactivate">Deactivate the shader after <paramref name="deactivateAfter"/></param>
        /// <param name="deactivateAfter">If <paramref name="deactivate"/> is set to true, after this many seconds the effect is disabled for this player-id.</param>
        public void ActivateBlackAndWhite(int playerId, bool deactivate = true, float deactivateAfter = 2.0f) {
            PostProcessingEffect ppEffect = new PostProcessingEffect(PostprocessingType.BlackAndWhite, 1, false, _loadedEffects[PostprocessingType.BlackAndWhite]);
            ppEffect.Activate(playerId, true);
            ppEffect.SetParameterForPlayer(playerId, "startTime", (float)Time.Gt.TotalGameTime.TotalSeconds);

            _effects.Add(ppEffect);

            if (deactivate) {
                new Timer(deactivateAfter, () => DectivateShader(ppEffect.Id));
            }
        }


        /// <summary>
        /// Activate the shockwave filter for a given player.
        /// Important: parameters about duration are set in the shader-initialization.
        /// </summary>
        /// <param name="position">3D-space coordinate of the position for the shockwave</param>
        /// <param name="animationLength">Length  of the animation for the shockwave to go over the whole screen</param>
        /// <param name="deactivate">Deactivate the shader after <paramref name="deactivateAfter"/></param>
        /// <param name="deactivateAfter">If <paramref name="deactivate"/> is set to true, after this many seconds the effect is disabled for this player-id.</param>
        public void ActivateShockWave(Vector3 position, float animationLength = 0.6f, bool deactivate = true, float deactivateAfter = 5.0f) {
            PostProcessingEffect ppEffect = new PostProcessingEffect(PostprocessingType.ShockWave, 1, false, _loadedEffects[PostprocessingType.ShockWave], position);

            for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(position);

                ppEffect.Activate(playerId, true);
                ppEffect.SetParameterForPlayer(playerId, "startTime", (float)Time.Gt.TotalGameTime.TotalSeconds);
                ppEffect.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                ppEffect.SetParameterForPlayer(playerId, "animationLength", animationLength);
                ppEffect.SetParameter("shockParams", new Vector3(10.0f, 0.8f, 0.1f));
            }

            _effects.Add(ppEffect);

            if (deactivate) {
                new Timer(deactivateAfter, () => DectivateShader(ppEffect.Id));
            }
        }

        /// <summary>
        /// Activate the shockwave filter for a given player.
        /// Important: parameters about duration are set in the shader-initialization.
        /// </summary>
        /// <param name="position">3D-space coordinate of the position for the shockwave</param>
        /// <param name="animationLength">Length  of the animation for the shockwave to go over the whole screen</param>
        /// <param name="deactivate">Deactivate the shader after <paramref name="deactivateAfter"/></param>
        /// <param name="deactivateAfter">If <paramref name="deactivate"/> is set to true, after this many seconds the effect is disabled for this player-id.</param>
        public void ActivateWave(Vector3 position, float animationLength = 5.0f, bool deactivate = true, float deactivateAfter = 5.0f) {
            PostProcessingEffect ppEffect = new PostProcessingEffect(PostprocessingType.Wave, 1, false, _loadedEffects[PostprocessingType.Wave], position);

            for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(position);
                float distance = (position - Screen.Cameras[playerId].transform.position).Length();

                ppEffect.Activate(playerId, true);
                ppEffect.SetParameterForPlayer(playerId, "startTime", (float)Time.Gt.TotalGameTime.TotalSeconds);
                ppEffect.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                ppEffect.SetParameterForPlayer(playerId, "animationLength", animationLength);
                ppEffect.SetParameterForPlayer(playerId, "cameraDistance", distance);
            }

            _effects.Add(ppEffect);

            if (deactivate) {
                new Timer(deactivateAfter, () => DectivateShader(ppEffect.Id));
            }
        }

        private void DectivateShader(int effectId) {
            for (int i = 0; i < _effects.Count; ++i) {
                if (_effects[i].Id == effectId) {
                    _effects.RemoveAt(i);
                    break;
                }
            }
        }

        public void Draw(RenderTarget2D renderTarget, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D depth1Texture, GameTime gameTime) {
            RenderTarget2D curTarget = renderTarget;
            int targetI = 0;

            // if dynamic props are needed
            foreach (var ppShader in _effects) {
                if (ppShader.IsActive()) {
                    ppShader.SetParameter("active", ppShader.ActiveParameter);
                    ppShader.SetParameter("time", (float)gameTime.TotalGameTime.TotalSeconds);

                    if (ppShader.Type == PostprocessingType.DepthOfField) {

                        // set the target to the blur target
                        graphicsDevice.SetRenderTarget(_blurTarget);

                        // get the gaussian blur shader
                        PostProcessingEffect blurShader = _twoPassEffect;
                        blurShader.SetParameter("active", new Vector4(1, 1, 1, 1));

                        // apply 2 blur passes      
                        spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            SamplerState.LinearClamp,
                            DepthStencilState.Default,
                            RasterizerState.CullNone);
                        blurShader.Effect.CurrentTechnique.Passes[0].Apply();
                        blurShader.Effect.CurrentTechnique.Passes[1].Apply();
                        spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);

                        spriteBatch.End();

                        // set the blurred scene and the depth map as parameter
                        ppShader.SetParameter("BlurScene", _blurTarget);
                        ppShader.SetParameter("DepthTexture", depth1Texture);
                    }

                    if (ppShader.Type == PostprocessingType.ShockWave) {
                        for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                            Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(ppShader.Position);
                            ppShader.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                        }
                    }

                    if (ppShader.Type == PostprocessingType.Wave) {
                        for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                            Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(ppShader.Position);
                            float distance = (ppShader.Position - Screen.Cameras[playerId].transform.position).Length();
                            ppShader.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                            ppShader.SetParameterForPlayer(playerId, "cameraDistance", distance);
                        }
                    }

                    // Setup next render-target to apply next filter
                    RenderTarget2D nextTarget = _renderTargets[targetI];
                    graphicsDevice.SetRenderTarget(nextTarget);


                    for (int i = 0; i < ppShader.Passes; i++) {
                        // apply post processing shader
                        spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            SamplerState.LinearClamp,
                            DepthStencilState.Default,
                            RasterizerState.CullNone);
                        ppShader.Effect.CurrentTechnique.Passes[0].Apply();
                        if (PostprocessingType.TwoPassBlur == ppShader.Type) {
                            ppShader.Effect.CurrentTechnique.Passes[1].Apply();
                        }

                        spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                        spriteBatch.End();
                    }

                    graphicsDevice.SetRenderTarget(null);
                    curTarget = nextTarget;
                    ++targetI;

                    if (targetI >= MaxEffects) {
                        break;
                    }
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
