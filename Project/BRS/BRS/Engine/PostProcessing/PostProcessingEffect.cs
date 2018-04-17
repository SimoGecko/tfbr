// (c) Alexander Lelidis 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.PostProcessing {
    class PostProcessingEffect {
        // type of the effect
        public PostprocessingType Type { get; }
        // how many time should this effect be applied
        public int Passes;
        // is this effect active
        public bool Active;
        // mg effect
        public Effect Effect { get; }
        // the name of the effect
        public string Name => Type.ToString();


        public PostProcessingEffect(PostprocessingType type, int passes, bool active, Effect effect) {
            Type = type;
            Passes = passes;
            Active = active;
            Effect = effect;
        }


        public void SetParameter(string name, Vector2 arg) {
            if (Effect.Parameters[name] != null)
            {
                Effect.Parameters[name].SetValue(arg);
            }
        }
        public void SetParameter(string name, Vector3 arg)
        {
            if (Effect.Parameters[name] != null)
            {
                Effect.Parameters[name].SetValue(arg);
            }
        }

        public void SetParameter(string name, float arg)
        {
            if(Effect.Parameters[name] != null)
            {
                Effect.Parameters[name].SetValue(arg);
            }
            
        }
        public void SetParameter(string name, Texture2D arg)
        {
            if(Effect.Parameters[name] != null)
            {
                Effect.Parameters[name].SetValue(arg);
            }
        }

    }
}
