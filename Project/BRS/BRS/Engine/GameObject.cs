﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using System.Threading.Tasks;
using BRS.Engine.Physics;
using BRS.Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    public enum ObjectTag { Default, Ground, Player, Base, Obstacle, Boundary, VaultDoor, DynamicObstacle, StaticObstacle }
    ////////// Class for objects in the world that have a transform, possibly a model and a list of components (scripts like in unity). Updated from main gameloop //////////

    public class GameObject {
        public Transform transform;
        public List<IComponent> components;
        public Model Model { get; set; }
        public ModelMesh mesh { get { return Model?.Meshes[0]; } } // assumes just 1 mesh per model
        public bool active { get; set; } = true;
        public string name { private set; get; }
        public ObjectTag tag { set; get; } = ObjectTag.Default;
        public EffectMaterial mat = null;

        static int InstanceCount = 0;

        public GameObject(string name, Model model = null) {
            Debug.Assert(!NameExists(name), "Name " + name + " must be unique!");

            this.name = name;
            transform = new Transform();
            components = new List<IComponent>();
            Model = model;
            allGameObjects.Add(this);
        }


        // ---------- CALLBACKS ----------
        public virtual void Awake() {
            foreach (IComponent c in components)  c.Awake();
        }
        public virtual void Start() {
            foreach (IComponent c in components)  c.Start();
        }

        public virtual void Update() {
            if (active)  foreach (IComponent c in components)  c.Update();
        }
        public virtual void LateUpdate() {
            if (active) foreach (IComponent c in components) c.LateUpdate();
        }

        public virtual void OnCollisionEnter(Collider col) {
            if (active) foreach (IComponent c in components) c.OnCollisionEnter(col);
        }

        public virtual void Draw(Camera cam) {
            if (Model != null && active) {
                Graphics.DrawModel(Model, cam.View, cam.Proj, transform.World, mat);
            }
        }



        // ---------- STATIC COMMANDS ----------
        static List<GameObject> allGameObjects = new List<GameObject>();
        public static GameObject[] All { get { return allGameObjects.ToArray(); } }



        // ---------- INSTANTIATION ----------
        public static GameObject Instantiate(string name) {
            return Instantiate(name, Vector3.Zero, Quaternion.Identity);
        }

        public static GameObject Instantiate(string name, Transform t) {
            return Instantiate(name, t.position, t.rotation);
        }

        public static GameObject Instantiate(string name, Vector3 position, Quaternion rotation) {
            GameObject tocopy = Prefabs.GetPrefab(name);
            if (tocopy == null) {
                Debug.LogError("Prefab not found");
                return null;
            }

            GameObject result = (GameObject)tocopy.Clone();
            result.transform.position = position;
            result.transform.rotation = rotation;
            //if (tocopy.transform.isStatic) result.transform.SetStatic();

            result.Start(); // because instantiated at runtime
            return result;
        }

        public virtual object Clone() {
            GameObject newObject = new GameObject(name + "_clone_" + InstanceCount);// (((GameObject)Activator.CreateInstance(type);
            InstanceCount++;
            newObject.transform.CopyFrom(this.transform);
            newObject.tag = tag;
            newObject.active = true;
            foreach (IComponent c in this.components) {
                newObject.AddComponent((IComponent)c.Clone());
            }
            newObject.Model = this.Model;
            //TODO copy material
            return newObject;
        }


        // ---------- DELETION ----------

        public static void ClearAll() {
            foreach (GameObject o in allGameObjects) Destroy(o);
            allGameObjects.Clear();
        }

        public static void Destroy(GameObject o) {
            if (o == null)  return;
            o.active = false;
            //if (o.HasComponent<Collider>()) Collider.allcolliders.Remove(o.GetComponent<Collider>()); // to avoid increase in colliders
            allGameObjects.Remove(o);
            //TODO free up memory
            foreach (Component c in o.components)  c.Destroy();
        }

        public static void Destroy(GameObject o, float lifetime) {// delete after some time
            new Timer(lifetime, () => Destroy(o));
            //await Task.Delay((int)(lifetime * 1000));
        }



        // ---------- SEARCH ----------
        public static GameObject FindGameObjectWithName(string name) { // note: this is a dangerous method, as it could return null and cause unhandled exception -> Check NameExists()
            foreach (GameObject o in allGameObjects) {
                if (o.name.Equals(name)) return o;
            }
            Debug.LogError("could not find gameobject " + name);
            return null;
        }

        public static bool NameExists(string name) {
            foreach (GameObject o in allGameObjects) {
                if (o.name.Equals(name)) return true;
            }
            return false;
        }

        //returns all the gameobject that satisfy the tag
        public static GameObject[] FindGameObjectsWithTag(ObjectTag _tag) {
            List<GameObject> result = new List<GameObject>();
            foreach (GameObject o in allGameObjects) {
                if (o.tag == _tag)  result.Add(o);
            }
            if (result.Count == 0) {
                Debug.LogError("could not find any gameobject with tag " + _tag.ToString());
                return null;
            }
            return result.ToArray();
        }

        // ---------- COMPONENTS ----------
        public void AddComponent(IComponent c) {
            components.Add(c);
            c.gameObject = this;
            //assumes no components added at runtime, otherwise c.Start();
        }

        public T GetComponent<T>() where T : IComponent {
            foreach (IComponent c in components) {
                if (c is T) return (T)c;
            }
            Debug.LogError("component not found " + typeof(T) + " inside " + name);
            return default(T);
        }

        public bool HasComponent<T>() where T : IComponent {
            foreach (IComponent c in components) {
                if (c is T) return true;
            }
            return false;
        }

    }
}
