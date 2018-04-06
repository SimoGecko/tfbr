// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;

namespace BRS.Engine {
    ////////// base class for scripting language //////////

    public interface IComponent {
        GameObject gameObject { get; set; }

        void Awake();
        void Start();
        void Update();
        void LateUpdate();
        void OnCollisionEnter(Collider c);
        void Draw(); // WHY?

        object Clone();
    }


    //TODO remove this Component duplicate
    public class Component : IComponent {
        public bool Active { get; set; } // TODO remove this
        public GameObject gameObject { get; set; }
        // ReSharper disable once InconsistentNaming
        public Transform  transform  { get { return gameObject.transform; } }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public virtual void Destroy() { }
        public virtual void OnCollisionEnter(Collider c) { }
        public virtual void Draw() { }

        public virtual object Clone() {
            return this.MemberwiseClone(); // MUST DO DEEP COPY!
        }

        public void Invoke(float delay, System.Action callback) {
            new Timer(delay, callback);
        }
    }

    public class ListComponents : Component { // WHAT IS THIS
        public readonly List<Component> Components;
        public readonly string NameIdentifier;


        public ListComponents(string name = null) {
            Components = new List<Component>();
            Active = true;
            NameIdentifier = name;
        }

        public override void Draw() {
            if (Active)
                foreach (Component comp in Components)
                    comp.Draw();
        }

        public override void Start() {
            foreach (Component comp in Components)
                comp.Start();
        }

        public override void Update() {
            if (Active)
                foreach (Component comp in Components)
                    comp.Update();
        }

        public void AddComponent(Component comp) {
            Components.Add(comp);
        }
    }
}
