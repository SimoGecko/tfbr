// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;

namespace BRS {
    ////////// base class for scripting language //////////

    public interface IComponent {
        GameObject gameObject { get; set; }

        void Start();
        void Update();
        void LateUpdate();
        void OnCollisionEnter(Collider c);
        void Draw();

        object Clone();
    }

    public class Component : IComponent {
        public bool active;
        GameObject m_gameObject; // added member
        public GameObject gameObject { get { return m_gameObject; } set { m_gameObject = value; } }
        public Transform  transform  { get { return m_gameObject.transform; } }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { } // really necessary?
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

    public class ListComponents : Component {
        public List<Component> listComponents;
        public string nameIdentifier;


        public ListComponents(string name = null) {
            listComponents = new List<Component>();
            active = true;
            nameIdentifier = name;
        }

        public override void Draw() {
            if (active)
                foreach (Component comp in listComponents)
                    comp.Draw();
        }

        public override void Start() {
            foreach (Component comp in listComponents)
                comp.Start();
        }

        public override void Update() {
            if (active)
                foreach (Component comp in listComponents)
                    comp.Update();
        }

        public void AddComponent(Component comp) {
            listComponents.Add(comp);
        }
    }
}
