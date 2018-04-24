namespace BRS.Engine.Particles {
    abstract class ParticleComponent : Component {
        public abstract bool IsEmitting { get; set; }
    }
}
