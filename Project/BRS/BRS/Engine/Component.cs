// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

namespace BRS {
    public interface IComponent {
        void Start();
        void Update();

        GameObject gameObject { get; set; }
        Transform  transform  { get; }
        object Clone();
    }

    //[Serializable]
    public class Component : IComponent {
        //base class for scripting language
        public GameObject gameObject { get; set; }

        public Transform  transform  { get { return gameObject.Transform; } }

        public virtual void Start() { }
        public virtual void Update() { }

        public virtual object Clone() {
            return this.MemberwiseClone(); // MUST DO DEEP COPY!
        }
    }
}
