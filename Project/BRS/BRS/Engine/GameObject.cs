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
        public Transform transform;
        public List<IComponent> components;
        public Model Model { get; set; }
        public ModelMesh mesh { get { return Model?.Meshes[0]; } } // assumes just 1 mesh per model
        public bool active { get; set; } = true;
        public string name { private set; get; }

        public int DrawOrder { set; get; }
        public ObjectTag tag { set; get; } = ObjectTag.Default;
        public ModelType ModelType { get; set; }
        public Material material = null;
        public bool Instanciate = false;


        static int InstanceCount = 0;

        public GameObject(string name, Model model = null) {
            Debug.Assert(!NameExists(name), "Name " + name + " must be unique!");

            this.name = name;
            DrawOrder = 0;
            transform = new Transform();
            components = new List<IComponent>();
            Model = model;
            allGameObjects.Add(this);
            SortAll();
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

        public void Draw3D(Camera cam) {
            if (active) {
                if (Model != null && !Instanciate) {
                    Graphics.DrawModel(Model, cam.View, cam.Proj, transform.World, material);
                }

                foreach (IComponent c in components) c.Draw3D(cam);
            }
        }

        internal void Draw3DDepth(Camera cam, Effect depthShader) {
            if (active && (tag == ObjectTag.Default || tag == ObjectTag.Boundary || tag == ObjectTag.StaticObstacle || tag == ObjectTag.Ground)) {
                if (Model != null) {
                    Graphics.DrawModelDepth(Model, cam.View, cam.Proj, transform.World, depthShader);
                }

                foreach (IComponent c in components) {
                    c.Draw3D(cam);
                }
            }
        }

        public void Draw2D(int i) { // i=0 -> fullscreen, else (1..4) splitscreen
            if (active) {
                foreach (IComponent c in components) c.Draw2D(i);
            }
        }




        // ---------- STATIC COMMANDS ----------
        static List<GameObject> allGameObjects = new List<GameObject>();
        public static GameObject[] All { get { return allGameObjects.ToArray(); } }

        public static void SortAll() {
            allGameObjects = allGameObjects.OrderBy(x => x.DrawOrder).ToList();
        }



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
            GameObject newObject = new GameObject(name + "_clone_" + InstanceCount);// (((GameObject)Activator.CreateInstance(type);
            InstanceCount++;
            newObject.Instanciate = Instanciate;
            newObject.ModelType = ModelType;
            newObject.transform.CopyFrom(this.transform);
            newObject.tag = tag;
            newObject.active = true;
            foreach (IComponent c in this.components) {
                newObject.AddComponent((IComponent)c.Clone());
            }
            newObject.Model = this.Model;
            newObject.material = material?.Clone();


            // Instanciating
            if (Model != null && Instanciate) {
                Graphics.AddInstance(ModelType, newObject);
            }

            return newObject;
        }


        // ---------- DELETION ----------

        public static void ClearAll() {
            while (allGameObjects.Count > 0) {
                Destroy(allGameObjects[0]);
            }
            allGameObjects.Clear();
        }

        public static void Destroy(GameObject o) {
            if (o == null) return;
            o.active = false;
            //if (o.HasComponent<RigidBodyComponent>()) RigidBodyComponent.allcolliders.Remove(o.GetComponent<RigidBodyComponent>()); // to avoid increase in colliders
            allGameObjects.Remove(o);
            //TODO free up memory
            foreach (Component c in o.components) c.Destroy();

            // Instanciating
            if (o.Model != null) {
                Graphics.RemoveInstance(o.ModelType, o);
            }
        }

        public static void Destroy(GameObject o, float lifetime) {// delete after some time
            new Timer(lifetime, () => Destroy(o));
        }

        public static void ConsiderPrefab(GameObject o) {
            allGameObjects.Remove(o);
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
                if (o.tag == _tag) result.Add(o);
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
