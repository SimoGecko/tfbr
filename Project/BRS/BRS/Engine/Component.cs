// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    ////////// base class for scripting language //////////

    public interface IComponent {
        GameObject gameObject { get; set; }
        Transform transform { get; }

        void Start();
        void Update();
        void OnCollisionEnter(Collider c);

        object Clone();
    }

    public class Component : IComponent {
        GameObject m_gameObject; // added member
        public GameObject gameObject { get { return m_gameObject; } set { m_gameObject = value; } }
        public Transform  transform  { get { return m_gameObject.transform; } }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnCollisionEnter(Collider c) { }

        public virtual object Clone() {
            return this.MemberwiseClone(); // MUST DO DEEP COPY!
        }
    }
}
