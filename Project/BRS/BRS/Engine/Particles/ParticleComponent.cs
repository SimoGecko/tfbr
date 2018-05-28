// (c) Alexander Lelidis, Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Particles {
    /// <summary>
    /// Wrapper for particle components
    /// </summary>
    public abstract class ParticleComponent : Component {

        /// <summary>
        /// State of the emitter if new particles are generated
        /// </summary>
        public abstract bool IsEmitting { get; set; }


        /// <summary>
        /// Get the next position and velocity for the particle.
        /// </summary>
        /// <returns>Tuple with the first item representing the position of the new particle and the second is the velocity.</returns>
        public virtual Tuple<Vector3, Vector3> GetNextPosition() {
            return new Tuple<Vector3, Vector3>(Vector3.Zero, Vector3.Zero);
        }
    }
}
