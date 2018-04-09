// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics.RigidBodies;

namespace BRS.Engine {
    ////////// base class for scripting language //////////

    public interface IComponent {
        GameObject GameObject { get; set; }

        void Start();
        void Update();
        void LateUpdate();
        void OnCollisionEnter(JRigidBody c);
        void Draw();

        object Clone();
    }

    public class Component : IComponent {
        public bool Active { get; set; }
        public GameObject GameObject { get; set; }
        // ReSharper disable once InconsistentNaming
        public Transform  transform  { get { return GameObject.transform; } }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { } // really necessary?
        public virtual void Destroy() { }
        public virtual void OnCollisionEnter(JRigidBody c) { }
        public virtual void Draw() { }

        public virtual object Clone() {
            return this.MemberwiseClone(); // MUST DO DEEP COPY!
        }

        public void Invoke(float delay, System.Action callback) {
            new Timer(delay, callback);
        }
    }

    public class ListComponents : Component {
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
