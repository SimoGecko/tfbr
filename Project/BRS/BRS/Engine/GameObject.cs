// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using BRS.Engine.Rendering;

namespace BRS.Engine {
    public enum ObjectTag { Default, Ground, Player, Base, Obstacle, Boundary, VaultDoor, DynamicObstacle, StaticObstacle, Chair, Plant, Cart, Police, Lighting }



    /// <summary>
    /// Class for objects in the world that have a transform, possibly a model and a list of components (scripts like in unity). Updated from main gameloop
    /// </summary>
    public class GameObject {
        private static readonly object LockList = new object();
        private static readonly object LockCounter = new object();


        public Transform transform;
        public List<IComponent> components;
        public Model Model { get; set; }
        public ModelMesh mesh { get { return Model?.Meshes[0]; } } // assumes just 1 mesh per model
        public bool active { get; set; } = true;
        public string name { private set; get; }

        public int DrawOrder { set; get; }
        public ObjectTag tag { set; get; } = ObjectTag.Default;
        public Material material = null;

        // True if the gameobject/model is flaged to use the hardware-instanciation for rendering speedup
        public bool UseHardwareInstanciation = false;
        // If hardware-instanciation is used the model type has to be specified
        public ModelType ModelType { get; set; }
        public float Alpha = 1.0f;


        static int InstanceCount = 0;

        public GameObject(string name, Model model = null) {
            Debug.Assert(!NameExists(name), "Name " + name + " must be unique!");

            this.name = name;
            DrawOrder = 0;
            transform = new Transform();
            components = new List<IComponent>();
            Model = model;

            //UseHardwareInstanciation = false;
            //ModelType = ModelType.NoHardwareInstanciation;

            AddGameObject(this);
        }

        /// <summary>
        /// This constructor is ONLY for the use of hardware instanciation..
        /// </summary>
        /// <param name="name"></param>
        /// <param name="modelType"></param>
        /// <param name="addToDrawings">True if the model is drawn, false if not.</param>
        public GameObject(string name, ModelType modelType, bool addToDrawings) {
            Debug.Assert(!NameExists(name), "Name " + name + " must be unique!");

            this.name = name;
            DrawOrder = 0;
            transform = new Transform();
            components = new List<IComponent>();
            AddGameObject(this);

            UseHardwareInstanciation = true;
            ModelType = modelType;

            if (addToDrawings) {
                HardwareRendering.AddInstance(modelType, this);
            }
        }

        private static void AddGameObject(GameObject go) {
            lock (LockList) {
                allGameObjects.Add(go);
            }
        }
        private static void RemoveGameObject(GameObject go) {
            lock (LockList) {
                allGameObjects.Remove(go);
            }
        }

        // ---------- CALLBACKS ----------
        public void Awake() {
            foreach (IComponent c in components) c.Awake();
        }
        public void Start() {
            foreach (IComponent c in components) c.Start();
        }
        public void Reset() {
            foreach (IComponent c in components) c.Reset();
        }

        public void Update() {
            if (active) foreach (IComponent c in components) c.Update();
        }
        public void LateUpdate() {
            if (active) foreach (IComponent c in components) c.LateUpdate();
        }

        public void OnCollisionEnter(Collider col) {
            if (active) foreach (IComponent c in components) c.OnCollisionEnter(col);
        }

        public void OnCollisionEnd(Collider col) {
            if (active) foreach (IComponent c in components) c.OnCollisionEnd(col);
        }


        public void Draw2D(int i) { // i=-1 -> fullscreen, else (0, 1, 2...) splitscreen
            if (active) {
                foreach (IComponent c in components) c.Draw2D(i);
            }
        }


        // ---------- STATIC COMMANDS ----------
        static HashSet<GameObject> allGameObjects = new HashSet<GameObject>();
        //public static GameObject[] All { get { return allGameObjects.ToArray(); } }
        public static GameObject[] All { get { lock (LockList) { return allGameObjects.ToArray(); } } }


        // ---------- INSTANTIATION ----------
        public static GameObject Instantiate(string name) {
            return Instantiate(name, Vector3.Zero, Quaternion.Identity, Vector3.Zero);
        }

        public static GameObject Instantiate(string name, Transform t) {
            //return Instantiate(name, t.World.Translation, t.World.Rotation);
            return Instantiate(name, t.position, t.rotation, Vector3.Zero);
        }

        public static GameObject Instantiate(string name, Vector3 position, Quaternion rotation) {
            return Instantiate(name, position, rotation, Vector3.Zero);
        }

        public static GameObject Instantiate(string name, Vector3 position, Quaternion rotation, Vector3 linearVelocity) {
            GameObject tocopy = Prefabs.GetPrefab(name);
            if (tocopy == null) {
                Debug.LogError("Prefab not found");
                return null;
            }

            GameObject result = (GameObject)tocopy.Clone();
            result.transform.position = position;
            result.transform.rotation = rotation;
            //if (tocopy.transform.isStatic) result.transform.SetStatic();

            result.Awake();
            result.Start(); // because instantiated at runtime

            // If it is a dynamic rigid body it cna have some linear velocity when instantiated
            if (result.HasComponent<DynamicRigidBody>()) {
                DynamicRigidBody dc = result.GetComponent<DynamicRigidBody>();
                dc.RigidBody.LinearVelocity = Conversion.ToJitterVector(linearVelocity) * 5;
                dc.RigidBody.AddTorque(Conversion.ToJitterVector(linearVelocity) * 5);
            }

            return result;
        }

        public virtual object Clone() {
            int counter = 0;
            //lock (LockCounter) {
                counter = InstanceCount++;
            //}
            string newName = name + "_clone_" + counter;
            GameObject newObject;

            if (UseHardwareInstanciation) {
                newObject = new GameObject(newName, ModelType, true);
            } else {
                newObject = new GameObject(newName);
                newObject.Model = Model;
                newObject.material = material?.Clone();
            }

            newObject.UseHardwareInstanciation = UseHardwareInstanciation;
            newObject.ModelType = ModelType;
            newObject.Alpha = Alpha;
            newObject.transform.CopyFrom(this.transform);
            newObject.tag = tag;
            newObject.active = true;

            foreach (IComponent c in this.components) {
                newObject.AddComponent((IComponent)c.Clone());
            }


            // Instanciating
            if (Model != null && UseHardwareInstanciation) {
                HardwareRendering.AddInstance(ModelType, newObject);
            }

            return newObject;
        }


        // ---------- DELETION ----------

        public static void ClearAll() {
            foreach (GameObject go in All) {
                Destroy(go);
            }

            lock (LockList) {
                allGameObjects.Clear();
            }
        }

        public static void Destroy(GameObject o) {
            if (o == null) return;
            o.active = false;

            // Instanciating
            if (o.Model != null) {
                HardwareRendering.RemoveInstance(o.ModelType, o);
            }

            //if (o.HasComponent<RigidBodyComponent>()) RigidBodyComponent.allcolliders.Remove(o.GetComponent<RigidBodyComponent>()); // to avoid increase in colliders
            RemoveGameObject(o);
            //TODO free up memory
            foreach (Component c in o.components) c.Destroy();
        }

        public static void Destroy(GameObject o, float lifetime) {// delete after some time
            new Timer(lifetime, () => Destroy(o));
        }

        public static void ConsiderPrefab(GameObject o) {
            RemoveGameObject(o);
        }


        // ---------- SEARCH ----------
        public static GameObject FindGameObjectWithName(string name) { // note: this is a dangerous method, as it could return null and cause unhandled exception -> Check NameExists()
            foreach (GameObject o in All) {
                if (o.name.Equals(name)) return o;
            }
            Debug.LogError("could not find gameobject " + name);
            return null;
        }

        public static bool NameExists(string name) {
            foreach (GameObject o in All) {
                if (o.name.Equals(name)) return true;
            }
            return false;
        }

        //returns all the gameobject that satisfy the tag
        //public static GameObject[] FindGameObjectsWithTag(ObjectTag _tag) {
        //    List<GameObject> result = new List<GameObject>();
        //    foreach (GameObject o in allGameObjects) {
        //        if (o.tag == _tag) result.Add(o);
        //    }
        //    if (result.Count == 0) {
        //        Debug.LogError("could not find any gameobject with tag " + _tag.ToString());
        //        return null;
        //    }
        //    return result.ToArray();
        //}

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
