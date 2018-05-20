// (c) Alexander Lelidis, Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BRS.Engine.PostProcessing {
    /// <summary>
    /// Manager which handles the postprocessing-effects 
    /// </summary>
    class PostProcessingManager {

        #region Singleton

        public static PostProcessingManager Instance { get; private set; }

        public static void Initialize() {
            List<PostprocessingType> defaultEffects = new List<PostprocessingType>
            {
                PostprocessingType.DepthOfField,
                PostprocessingType.Chromatic,
                PostprocessingType.ColorGrading,
                PostprocessingType.Vignette,
                PostprocessingType.TwoPassBlur,
                PostprocessingType.BlackAndWhite
            };

            Instance = new PostProcessingManager(defaultEffects);

            SceneTarget = new RenderTarget2D(
                Graphics.gD,
                Screen.Width,
                Screen.Height,
                false,
                Graphics.gD.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            DepthTarget = new RenderTarget2D(
                Graphics.gD,
                Screen.Width,
                Screen.Height,
                false,
                Graphics.gD.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        #endregion

        #region Properties and attributes
        public static RenderTarget2D SceneTarget { get; private set; }
        public static RenderTarget2D DepthTarget { get; private set; }
        private readonly List<PostProcessingEffect> _effects = new List<PostProcessingEffect>();
        private readonly Dictionary<PostprocessingType, Effect> _loadedEffects = new Dictionary<PostprocessingType, Effect>();
        private readonly Dictionary<PostprocessingType, PostProcessingEffect> _fixEffects = new Dictionary<PostprocessingType, PostProcessingEffect>();
        private readonly PostProcessingEffect _twoPassEffect;
        private RenderTarget2D _blurTarget1;
        private RenderTarget2D _blurTarget2;
        private RenderTarget2D _renderTarget1;
        private RenderTarget2D _renderTarget2;
        private  float Distance = 1f;
        private  float Range = 16.5f;

        #endregion

        #region Constructor


        private PostProcessingManager(List<PostprocessingType> initialized) {
            foreach (PostprocessingType pType in Enum.GetValues(typeof(PostprocessingType))) {
                string fileName = pType.ToString();
                _loadedEffects[pType] = File.Load<Effect>("Other/effects/" + fileName);
            }


            foreach (PostprocessingType pType in initialized) {
                PostProcessingEffect ppEffect = new PostProcessingEffect(pType, false, _loadedEffects[pType]);

                _fixEffects[pType] = ppEffect;

                ppEffect.SetParameter("players", GameManager.NumPlayers);
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
                        float nearClip = Camera.Near;
                        float farClip = Camera.FarDepth;
                        farClip = farClip / (farClip - nearClip);

                        ppEffect.SetParameter("Distance", Distance);
                        ppEffect.SetParameter("Range", Range);
                        ppEffect.SetParameter("Near", nearClip);
                        ppEffect.SetParameter("Far", farClip);

                        PostProcessingEffect ppBlur = new PostProcessingEffect(PostprocessingType.TwoPassBlur, false, _loadedEffects[PostprocessingType.TwoPassBlur]);
                        ppBlur.SetParameter("players", GameManager.NumPlayers);
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

                        ppEffect.SetParameter("LUT", File.Load<Texture2D>("Images/lut/lut_01"));

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


        #endregion

        #region Monogame-structure


        /// <summary>
        /// Initialize all render-targets which are needed
        /// </summary>
        /// <param name="spriteBatch">Spritebatch for the render-targets</param>
        public void Start(SpriteBatch spriteBatch) {
            _renderTarget1 = new RenderTarget2D(spriteBatch.GraphicsDevice, Screen.Width, Screen.Height, false,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            _renderTarget2 = new RenderTarget2D(spriteBatch.GraphicsDevice, Screen.Width, Screen.Height, false,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            _blurTarget1 = new RenderTarget2D(
                spriteBatch.GraphicsDevice,
                Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _blurTarget2 = new RenderTarget2D(
                spriteBatch.GraphicsDevice,
                Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }


        /// <summary>
        /// Apply all postprocessing-effects on the 3D-rendered output
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw( SpriteBatch spriteBatch) {
            RenderTarget2D curTarget = SceneTarget;
            int targetI = 0;

            // if dynamic props are needed
            foreach (var ppShader in _effects) {
                if (ppShader.IsActive()) {
                    ppShader.SetParameter("active", ppShader.ActiveParameter);
                    ppShader.SetParameter("time", Time.CurrentTime);

                    switch (ppShader.Type) {
                        case PostprocessingType.DepthOfField:
                            RenderTarget2D curBlurTarget = SceneTarget;

                            // get the gaussian blur shader
                            PostProcessingEffect blurShader = _twoPassEffect;
                            blurShader.SetParameter("active", new Vector4(1, 1, 1, 1));

                            for (int i = 0; i < 2; i++) {
                                // Setup next render-target to apply next filter
                                RenderTarget2D nextTarget = (targetI++ % 2 == 0) ? _blurTarget1 : _blurTarget2;
                                Graphics.gD.SetRenderTarget(nextTarget);

                                // apply post processing shader
                                SpriteBatchBegin(ref spriteBatch);
                                blurShader.Effect.CurrentTechnique.Passes[i].Apply();
                                spriteBatch.Draw(curBlurTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                                SpriteBatchEnd(ref spriteBatch);

                                Graphics.gD.SetRenderTarget(null);
                                curBlurTarget = nextTarget;
                            }

                            // set the blurred scene and the depth map as parameter
                            ppShader.SetParameter("BlurScene", curBlurTarget);
                            ppShader.SetParameter("DepthTexture", DepthTarget);

                            break;

                        case PostprocessingType.ShockWave:
                            for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                                Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(ppShader.Position);
                                ppShader.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                            }
                            break;

                        case PostprocessingType.Wave:
                            for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                                Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(ppShader.Position);
                                float distance = (ppShader.Position - Screen.Cameras[playerId].transform.position).Length();
                                ppShader.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                                ppShader.SetParameterForPlayer(playerId, "cameraDistance", distance);
                            }
                            break;
                    }

                    for (int i = 0; i < ppShader.Effect.CurrentTechnique.Passes.Count; i++) {
                        // Setup next render-target to apply next filter
                        RenderTarget2D nextTarget = (targetI++ % 2 == 0) ? _renderTarget1 : _renderTarget2;
                        Graphics.gD.SetRenderTarget(nextTarget);

                        // apply post processing shader
                        SpriteBatchBegin(ref spriteBatch);
                        ppShader.Effect.CurrentTechnique.Passes[i].Apply();
                        spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                        SpriteBatchEnd(ref spriteBatch);

                        Graphics.gD.SetRenderTarget(null);
                        curTarget = nextTarget;
                    }
                }
            }

            // draw to screen
            Graphics.gD.SetRenderTarget(null);
            SpriteBatchBegin(ref spriteBatch);
            spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
            SpriteBatchEnd(ref spriteBatch);
        }


        #endregion

        #region Shader management


        /// <summary>
        /// Set the state of a specific shader for a given player <paramref name="playerId"/>.
        /// </summary>
        /// <param name="shader">Shader-type</param>
        /// <param name="playerId">Id of the player</param>
        /// <param name="active">New state of the shader</param>
        public void SetShaderStatus(PostprocessingType shader, int playerId, bool active) {
            foreach (PostProcessingEffect postProcessingEffect in _effects) {
                if (postProcessingEffect.Type == shader) {
                    postProcessingEffect.Activate(playerId, active);
                }
            }
        }
        

        /// <summary>
        /// Remove all shaders with the given type. (Usually to cleanup)
        /// </summary>
        /// <remarks>Only shaders which are not fix by initialization are removed.</remarks>
        /// <param name="shader">Shader-type</param>
        public void RemoveShader(PostprocessingType shader) {
            if (_fixEffects.ContainsKey(shader) && _fixEffects.ContainsKey(shader)) {
                return;
            }

            _effects.RemoveAll(item => item.Type == shader);
        }


        /// <summary>
        /// Deactivate and remove an effect with the given id.
        /// </summary>
        /// <remarks>Only shaders which are not fix by initialization are removed.</remarks>
        /// <param name="effectId">Id of the effect</param>
        private void DectivateShader(int effectId) {
            for (int i = 0; i < _effects.Count; ++i) {
                if (_effects[i].Id == effectId) {
                    _effects.RemoveAt(i);
                    break;
                }
            }
        }


        #endregion

        #region Activate special-effects


        /// <summary>
        /// Activate the black and white filter for a given player.
        /// Important: parameters about duration are set in the shader-initialization.
        /// </summary>
        /// <param name="playerId">Id of the player to apply the shader</param>
        public void ActivateBlackAndWhite(int playerId) {
            PostProcessingEffect ppEffect = _fixEffects[PostprocessingType.BlackAndWhite];
            ppEffect.Activate(playerId, true);
            ppEffect.SetParameterForPlayer(playerId, "startTime", Time.CurrentTime);
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
            PostProcessingEffect ppEffect = new PostProcessingEffect(PostprocessingType.ShockWave, false, _loadedEffects[PostprocessingType.ShockWave], position);

            for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(position);

                ppEffect.Activate(playerId, true);
                ppEffect.SetParameterForPlayer(playerId, "startTime", Time.CurrentTime);
                ppEffect.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                ppEffect.SetParameterForPlayer(playerId, "animationLength", animationLength);
                ppEffect.SetParameter("shockParams", new Vector3(10.0f, 0.8f, 0.1f));
            }

            _effects.Add(ppEffect);

            if (deactivate) {
                // ReSharper disable once ObjectCreationAsStatement
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
            PostProcessingEffect ppEffect = new PostProcessingEffect(PostprocessingType.Wave, false, _loadedEffects[PostprocessingType.Wave], position);

            for (int playerId = 0; playerId < GameManager.NumPlayers; ++playerId) {
                Vector2 screenPosition = Screen.Cameras[playerId].WorldToScreenPoint01(position);
                float distance = (position - Screen.Cameras[playerId].transform.position).Length();

                ppEffect.Activate(playerId, true);
                ppEffect.SetParameterForPlayer(playerId, "startTime", Time.CurrentTime);
                ppEffect.SetParameterForPlayer(playerId, "centerCoord", screenPosition);
                ppEffect.SetParameterForPlayer(playerId, "animationLength", animationLength);
                ppEffect.SetParameterForPlayer(playerId, "cameraDistance", distance);
            }

            _effects.Add(ppEffect);

            if (deactivate) {
                // ReSharper disable once ObjectCreationAsStatement
                new Timer(deactivateAfter, () => DectivateShader(ppEffect.Id));
            }
        }


        #endregion

        #region Helper functions

        private void SpriteBatchBegin(ref SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
        }

        private void SpriteBatchEnd(ref SpriteBatch spriteBatch) {
            spriteBatch.End();
        }

        #endregion

    }
}
