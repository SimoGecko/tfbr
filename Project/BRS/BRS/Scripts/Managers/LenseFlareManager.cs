// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    /// <summary>
    /// stores a texture, accesses rgb components and evaluates the distribution
    /// </summary>
    class LenseFlareManager : Component {

        // --------------------- VARIABLES ---------------------

        //public
        public static bool IsActive = true;

        //private
        private List<List<Flare>> _flares;
        private List<bool> _active;

        private float _pulseTimer;
        private float _lightScale;
        private readonly Color _lightColor = Color.White;
        private Vector2 _screenCenter = new Vector2(Screen.SplitWidth / 2.0f, Screen.SplitHeight / 2.0f);

        private readonly Vector3 _sun;

        //reference
        private Texture2D _flareTexture2D;
        private SpriteBatch _spriteBatch;


        // --------------------- BASE METHODS ------------------
        public LenseFlareManager(Vector3 sun) {
            _sun = sun;
        }

        public override void Awake() {
            _flareTexture2D = File.Load<Texture2D>("Images/textures/flares");
        }

        public override void Start() {
            _spriteBatch = new SpriteBatch(Graphics.gD);
            _flares = new List<List<Flare>>();
            _active = new List<bool> { false, false, false, false };

            for (int i = 0; i < 4; ++i) {
                _flares.Add(new List<Flare>());
                AddFlare(i, 1, 1, 254, 254, 1.2f, 2.1f);
                AddFlare(i, 258, 1, 254, 254, 1.1f, 0.9f);
                AddFlare(i, 513, 1, 254, 254, 1.0f, 1.1f);
                AddFlare(i, 0, 256, 126, 126, 0.8f, 1.1f);
                AddFlare(i, 0, 256, 126, 126, 0.7f, 1f);
                AddFlare(i, 4, 388, 122, 122, 0.6f, 0.4f);
                AddFlare(i, 4, 388, 122, 122, 0.4f, 0.44f);
                AddFlare(i, 4, 388, 122, 122, 0.3f, 0.36f);
                AddFlare(i, 770, 1, 254, 254, 0.37f, 1f);
                AddFlare(i, 770, 1, 254, 254, 0.18f, 1.6f);
                AddFlare(i, 129, 257, 122, 122, 0.13f, 1.4f);
                AddFlare(i, 129, 385, 122, 122, 0.16f, 1f);
                AddFlare(i, 129, 385, 122, 122, 0.9f, 1.1f);
                AddFlare(i, 129, 385, 122, 122, 1.14f, 1.1f);
                AddFlare(i, 257, 257, 254, 254, 0.07f, 1.1f);
            }
        }

        /// <summary>
        /// Update from the GameLoop:
        /// - Flares are calculated for each active player
        /// - Flares are only calculated if the camera is directed against the sun
        /// </summary>
        public override void Update() {
            if (!IsActive) {
                return;
            }

            _pulseTimer = (_pulseTimer + 0.14f) % 6.27f;
            _lightScale = (7f + (float)Math.Sin(_pulseTimer)) / 9f;

            for (int playerIndex = 0; playerIndex < GameManager.NumPlayers; ++playerIndex) {

                // find the vector between the center of the screen and the point light:
                Camera camera = Camera.GetCamera(playerIndex);
                Vector2 mosV = camera.WorldToScreenPoint(_sun);
                _screenCenter = new Vector2(Screen.SplitWidth / 2.0f, Screen.SplitHeight / 2.0f);
                Vector2 vec = mosV - _screenCenter;

                Vector3 cameraSun = camera.transform.Forward;

                _active[playerIndex] = Vector3.Dot(cameraSun, _sun) > 0.0f;

                if (!_active[playerIndex]) {
                    continue;
                }

                // Update the flare positions along the vector and update alpha, and rotation
                for (int i = 0; i < _flares[playerIndex].Count; ++i) {
                    Flare flare = _flares[playerIndex][i];
                    flare.Pos = flare.Offset * vec + _screenCenter;

                    // (don't rotate the main flare - looks strange)
                    if (i != 2) {
                        //(calculates the angle of the vector so we can make sprites face the light)
                        flare.Rot = (float)Math.Atan2(vec.Y, vec.X);
                    }

                    // last flare needs angle to be corrected (should just do on sprite sheet)
                    if (i + 1 == _flares[playerIndex].Count) {
                        flare.Rot += MathHelper.PiOver2;
                    }

                    flare.Color = _lightColor;

                    // find the percentage distance the flare is from the center (approximately)
                    // we divide based on approximate maximum distance we want between the opposite ends of the vector (to control fade out rate)
                    Vector2 radiusVec = (i != 2) ? vec / (15 * 300) : vec / (15 * 450);

                    // now scale the alpha transparency by this percent:                
                    flare.Color.A = (byte)MathHelper.Clamp(255.0f - 255.0f * radiusVec.LengthSquared(), 0f, 255f);
                }
            }
        }



        // --------------------- CUSTOM METHODS ----------------

        private void AddFlare(int index, int x, int y, int w, int h, float distance, float scale) {
            //note: I subtract 0.5 from distance because I need them to extend in the opposite direction along the vector also
            _flares[index].Add(new Flare(new Rectangle(x, y, w, h), (distance - 0.5f) * 2f, scale * 0.75f, Color.White));
        }

        public override void Draw2D(int i) {
            if (i == -1 || !IsActive) {
                return;
            }

            if (!_active[i]) {
                return;
            }

            //draw lens flare effect over everything (using Additive blending):
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (int j = 0; j < _flares[i].Count; ++j) {
                Flare flare = _flares[i][j];
                _spriteBatch.Draw(_flareTexture2D, flare.Pos, flare.Rect, flare.Color, flare.Rot,
                    flare.Origin, flare.Scale + _lightScale / 10, SpriteEffects.None, 0);
            }
            _spriteBatch.End();
        }



        // other
        private class Flare {

            #region Properties and attributes

            // Source rectangle on texture image
            public readonly Rectangle Rect;

            // Flare's center
            public readonly Vector2 Origin;

            // Flare's position
            public Vector2 Pos;

            // Flare's rotation
            public float Rot;

            // Set intensity and tint
            public Color Color;

            // % distance along vector between screen center and light
            public readonly float Offset;

            // Size of the flare
            public readonly float Scale;

            #endregion

            #region Constructor

            public Flare(Rectangle sourceRect, float distance, float scale, Color tint) {
                Rect = sourceRect;
                Origin = new Vector2(sourceRect.Width / 2.0f, sourceRect.Height / 2.0f);
                Offset = distance;
                Pos = Vector2.One;
                Rot = 0f;
                Scale = scale;
                Color = tint;
            }

            #endregion

        }
    }
}