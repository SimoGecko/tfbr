// (c) Alexander Lelidis 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BRS.Engine.PostProcessing {
    class PostProcessingEffect {
        private static int _counter;
        public int Id { get; }
        // type of the effect
        public PostprocessingType Type { get; }
        // how many time should this effect be applied
        public int Passes;
        // is this effect active
        private bool[] _active;
        public Vector4 ActiveParameter => new Vector4(_active[0] ? 1.0f : 0.0f, _active[1] ? 1.0f : 0.0f, _active[2] ? 1.0f : 0.0f, _active[3] ? 1.0f : 0.0f);
        public Vector3 Position;

        //private RenderTarget2D RenderTarget { get; }

        // mg effect
        public Effect Effect { get; }


        public PostProcessingEffect(PostprocessingType type, int passes, bool active, Effect effect, Vector3 position = default(Vector3)) {
            Id = _counter++;
            Type = type;
            Passes = passes;
            _active = new [] { active, active, active, active };
            Effect = effect;
            Position = position;

            //RenderTarget = new RenderTarget2D(
            //    Graphics.gD,
            //    Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
            //    Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
            //    false,
            //    Graphics.gD.PresentationParameters.BackBufferFormat,
            //    DepthFormat.Depth24);
        }


        public void SetParameter(string name, Vector2 arg) {
            if (Effect.Parameters[name] != null) {
                Effect.Parameters[name].SetValue(arg);
            }
        }


        public void SetParameter(string name, Vector3 arg) {
            if (Effect.Parameters[name] != null) {
                Effect.Parameters[name].SetValue(arg);
            }
        }


        public void SetParameter(string name, Vector4 arg) {
            if (Effect.Parameters[name] != null) {
                Effect.Parameters[name].SetValue(arg);
            }
        }


        public void SetParameter(string name, float arg) {
            if (Effect.Parameters[name] != null) {
                Effect.Parameters[name].SetValue(arg);
            }

        }


        public void SetParameter(string name, Texture2D arg) {
            if (Effect.Parameters[name] != null) {
                Effect.Parameters[name].SetValue(arg);
            }
        }

        public void SetParameterForPlayer(int playerId, string name, float value) {
            if (Effect.Parameters[name] != null) {
                Vector4 parameters = Effect.Parameters[name].GetValueVector4();

                switch (GameManager.NumPlayers) {
                    case 1:
                        parameters = new Vector4(value);
                        break;
                    case 2:
                        if (playerId == 0) {
                            parameters.X = value;
                            parameters.Z = value;
                        } else {
                            parameters.Y = value;
                            parameters.W = value;
                        }
                        break;
                    case 4:
                        switch (playerId) {
                            case 0: parameters.X = value; break;
                            case 1: parameters.Y = value; break;
                            case 2: parameters.Z = value; break;
                            case 3: parameters.W = value; break;
                        }
                        break;
                }

                Effect.Parameters[name].SetValue(parameters);
            }
        }

        /// <summary>
        /// Set a vector2-parameter for the given player <paramref name="playerId"/>.
        /// Important: In the shader there needs to be 4 variables named as "name1", "name2", ...
        /// </summary>
        /// <example>
        /// In the shader:
        /// float2 centerCoord0;
        /// float2 centerCoord1;
        /// float2 centerCoord2;
        /// float2 centerCoord3;
        /// 
        /// In the code:
        /// SetParamterForPlayer(0, "centerCoord", new Vector2(0.5f, 0.5f);
        /// </example>
        /// <param name="playerId">Player-id</param>
        /// <param name="name">Base name of the parameter in the shader</param>
        /// <param name="value">New value for the parameter.</param>
        public void SetParameterForPlayer(int playerId, string name, Vector2 value) {
            switch (GameManager.NumPlayers) {
                case 1:
                    Effect.Parameters[name + "0"].SetValue(value);
                    Effect.Parameters[name + "1"].SetValue(value);
                    Effect.Parameters[name + "2"].SetValue(value);
                    Effect.Parameters[name + "3"].SetValue(value);
                    break;
                case 2:
                    Effect.Parameters[name + playerId].SetValue(value);
                    Effect.Parameters[name + (playerId + 2)].SetValue(value);
                    break;
                case 4:
                    Effect.Parameters[name + playerId].SetValue(value);
                    break;
            }
        }


        /// <summary>
        /// Set the shader-state for the given cameras/viewports.
        /// </summary>
        /// <param name="cameraIds">All cameras/viewports for which the state is updated</param>
        /// <param name="active">New shader-state</param>
        private void Activate(List<int> cameraIds, bool active) {
            foreach (int cameraId in cameraIds) {
                _active[cameraId] = active;
            }
        }


        /// <summary>
        /// Set the shader-state for the given player <paramref name="playerId"/>
        /// </summary>
        /// <param name="playerId">Player-id</param>
        /// <param name="active">New shader-state</param>
        public void Activate(int playerId, bool active) {
            Activate(GetPlayerCamerasId(playerId), active);
        }


        /// <summary>
        /// Returns if the player is active for any player.
        /// </summary>
        /// <returns>True  if the shader is active for any player; false if for no player.</returns>
        public bool IsActive() {
            return _active[0] || _active[1] || _active[2] || _active[3];
        }


        /// <summary>
        /// Get the list of the affected cameras/viewports for player <paramref name="playerId"/>
        /// </summary>
        /// <param name="playerId">Player-id</param>
        /// <returns>List which contains the id of all affected cameras/viewports</returns>
        private List<int> GetPlayerCamerasId(int playerId) {
            switch (GameManager.NumPlayers) {
                case 1: return new List<int> { 0, 1, 2, 3 };
                case 2: return new List<int> { playerId, playerId + 2 };
                case 4: return new List<int> { playerId };
                default: return new List<int>();
            }
        }

    }
}
