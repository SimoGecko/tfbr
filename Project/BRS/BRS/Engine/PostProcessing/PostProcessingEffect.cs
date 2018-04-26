// (c) Alexander Lelidis 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.PostProcessing {
    class PostProcessingEffect {
        // type of the effect
        public PostprocessingType Type { get; }
        // how many time should this effect be applied
        public int Passes;
        // is this effect active
        public bool[] Active;
        public Vector4 ActiveParameter { get { return new Vector4(Active[0] ? 1.0f : 0.0f, Active[1] ? 1.0f : 0.0f, Active[2] ? 1.0f : 0.0f, Active[3] ? 1.0f : 0.0f); } }
        // mg effect
        public Effect Effect { get; }
        // the name of the effect
        public string Name => Type.ToString();


        public PostProcessingEffect(PostprocessingType type, int passes, bool active, Effect effect) {
            Type = type;
            Passes = passes;
            Active = new bool[] { active, active, active, active };
            Effect = effect;
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

        public void Activate(List<int> players, bool active) {
            foreach (int playerId in players) {
                Active[playerId] = active;
            }
        }

        public void Activate(int playerId, bool active) {
            Activate(GetPlayerIds(playerId), active);
        }

        public bool IsActive() {
            return Active[0] || Active[1] || Active[2] || Active[3];
        }

        private List<int> GetPlayerIds(int playerId) {
            switch (GameManager.NumPlayers) {
                case 1: return new List<int> { 0, 1, 2, 3 };
                case 2: return new List<int> { playerId, playerId + 2 };
                case 4: return new List<int> { playerId };
                default: return new List<int>();
            }
        }

    }
}
