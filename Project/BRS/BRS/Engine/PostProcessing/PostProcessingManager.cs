// (c) Alexander Lelidis 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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

        private readonly List<PostProcessingEffect> _effects = new List<PostProcessingEffect>();
        private readonly Dictionary<PostprocessingType, Effect> _loadedEffects = new Dictionary<PostprocessingType, Effect>();
        private readonly Dictionary<PostprocessingType, bool> _fixEffects = new Dictionary<PostprocessingType, bool>();
        private RenderTarget2D _blurTarget;
        private readonly Texture2D _testGrid;
        //private Vector3 _wavePosition;
        private bool DEBUG = false;
        private readonly List<Texture2D> _lut = new List<Texture2D>();
        private int _currentLuT = 0;
        private int _maxLuT = 20;

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

                        for (var i = 0; i < _maxLuT; i++) {
                            _lut.Add(File.Load<Texture2D>("Images/lut/lut (" + i.ToString() + ")"));
                        }

                        ppEffect.SetParameter("LUT", _lut[0]);

                        for (var i = 0; i < GameManager.NumPlayers; i++) {
                            ppEffect.Activate(i, true);
                        }
                        break;

                    case PostprocessingType.ShockWave:
                        _testGrid = File.Load<Texture2D>("Images/textures/Pixel_grid");
                        ppEffect.SetParameter("centerCoord", new Vector2(0.5f, 0.5f));
                        ppEffect.SetParameter("shockParams", new Vector3(10.0f, 0.8f, 0.1f));
                        break;

                    case PostprocessingType.Wave:
                        _testGrid = File.Load<Texture2D>("Images/textures/Pixel_grid");
                        ppEffect.SetParameter("centerCoord", new Vector2(0.5f, 0.5f));
                        ppEffect.SetParameter("shockParams", new Vector3(10.0f, 0.8f, 0.1f));
                        break;
                }

                _effects.Add(ppEffect);
            }
        }

        public void Start(SpriteBatch spriteBatch) {
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

            // if dynamic props are needed
            foreach (var ppShader in _effects) {
                if (ppShader.IsActive()) {
                    ppShader.SetParameter("active", ppShader.ActiveParameter);
                    ppShader.SetParameter("time", (float)gameTime.TotalGameTime.TotalSeconds);

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
                    RenderTarget2D nextTarget = ppShader.RenderTarget;
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

                        if (PostprocessingType.ShockWave == ppShader.Type && DEBUG) {
                            spriteBatch.Draw(_testGrid, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
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
