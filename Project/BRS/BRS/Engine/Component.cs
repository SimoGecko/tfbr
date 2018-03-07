// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        GameObject m_gameObject;
        public GameObject gameObject { get { return m_gameObject; } set { m_gameObject = value; } }
        public Transform  transform  { get { return m_gameObject.transform; } }

        public virtual void Start() { }
        public virtual void Update() { }

        public virtual object Clone() {
            return this.MemberwiseClone(); // MUST DO DEEP COPY!
        }
    }
}
