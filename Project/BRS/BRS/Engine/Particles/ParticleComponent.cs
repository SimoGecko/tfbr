// (c) Alexander Lelidis, Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB
namespace BRS.Engine.Particles {
    /// <summary>
    /// Wrapper for particle components
    /// </summary>
    abstract class ParticleComponent : Component {
        // function if the particle component is currently emitting
        public abstract bool IsEmitting { get; set; }
    }
}
