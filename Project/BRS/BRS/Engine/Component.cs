﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics.Colliders;

namespace BRS.Engine {
    ////////// base class for scripting language //////////

    public interface IComponent {
        GameObject gameObject { get; set; }

        void Awake();
        void Start();
        void Reset();
        void Update();
        void LateUpdate();
        void OnCollisionEnter(Collider c);
        void OnCollisionEnd(Collider c);
        //void Draw3D(Camera camera);
        void Draw2D(int i);

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
        public virtual void Reset() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public virtual void Destroy() { }
        public virtual void OnCollisionEnter(Collider c) { }
        public virtual void OnCollisionEnd(Collider c) { }
        //public virtual void Draw3D(Camera camera) { }
        public virtual void Draw2D(int i) { }

        public virtual object Clone() {
            return this.MemberwiseClone(); // MUST DO DEEP COPY!
        }

        public void Invoke(float delay, System.Action callback) {
            new Timer(delay, callback);
        }
    }


    /// <summary>
    /// Class to store a list of components
    /// </summary>
    public class ListComponents : Component { 
        public readonly List<Component> Components;
        public readonly string NameIdentifier;


        public ListComponents(string name = null) {
            Components = new List<Component>();
            Active = true;
            NameIdentifier = name;
        }

        public override void Draw2D(int i) {
            if (Active)
                foreach (Component comp in Components)
                    comp.Draw2D(i);
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
