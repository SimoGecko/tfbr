using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine.PostProcessing
{
    class PostProcessingEffect
    {
        // how many time should this effect be applied
        public int Passes = 1;
        // is this effect active
        public bool Active = false;
        // mg effect
        public Effect Effect;
        // the name of the effect
        public String Name;

        public PostProcessingEffect(int passes, bool active, Effect effect, String name)
        {
            Passes = passes;
            Active = active;
            Effect = effect;
            Name = name;
        }

        public void SetParameter(String name, Vector2 arg)
        {
            this.Effect.Parameters[name].SetValue(arg);
        }



    }
}
