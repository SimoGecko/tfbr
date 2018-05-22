// (c) Alexander Lelidis, Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Particles {
    /// <summary>
    /// Wrapper for particle components
    /// </summary>
    public abstract class ParticleComponent : Component {

        // True if the particle component is currently emitting
        public abstract bool IsEmitting { get; set; }

        public virtual List<Tuple<Vector3,Vector3>> GetNextPositions()
        {
            return new List<Tuple<Vector3, Vector3>>();
        }
    }
}
