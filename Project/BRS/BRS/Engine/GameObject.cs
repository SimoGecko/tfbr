// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS {
    public enum ObjectType { Ground, Player, Base, Obstacle, Boundary, Default }

    /// <summary>
    /// Class for objects in the world that have a transform, possibly a model and a list of components (scripts like in unity). Updated from main gameloop
    /// </summary>
    public class GameObject {
        public Transform Transform;
        List<IComponent> components;
        public Model Model { get; private set; }
        public ModelMesh mesh { get { return Model?.Meshes[0]; } } // assumes just 1 mesh per model
        public bool Active { get; set; } = true;
        public string Name { private set; get; }
        public string myTag = "";
        public ObjectType Type { set; get; } = ObjectType.Default;


        static int InstanceCount = 0;

        public GameObject(string name, Model model = null) {
            Name = name;
            Transform = new Transform();
            components = new List<IComponent>();
            Model = model;
            allGameObjects.Add(this);
        }

        public virtual void Start() {
            foreach (IComponent c in components) {
                c.Start();
            }
        }

        public virtual void Update() {
            if (Active) {
                foreach (IComponent c in components) {
                    c.Update();
                }
            }
        }
        public virtual void LateUpdate() {
            if (Active) {
                foreach (IComponent c in components) {
                    c.LateUpdate();
                }
            }
        }

        public virtual void OnCollisionEnter(Collider col) {
            if (Active)
                foreach (IComponent c in components) c.OnCollisionEnter(col);
        }
        //public virtual void OnCollisionExit () { }

        public virtual void Draw(Camera cam) {
            if (Model != null && Active) {
                Utility.DrawModel(Model, cam.View, cam.Proj, Transform.World);
            }
        }



        //STATIC COMMANDS
        static List<GameObject> allGameObjects = new List<GameObject>();

        /*
        public static void Add(GameObject o) {
            allGameObjects.Add(o);
        }
        */

        //INSTANTIATION
        public static GameObject Instantiate(string name) {
            return Instantiate(name, Vector3.Zero, Quaternion.Identity);
        }

        public static GameObject Instantiate(string name, Transform t) {
            return Instantiate(name, t.World.Translation, t.World.Rotation);
        }

        public static GameObject Instantiate(string name, Vector3 position, Quaternion rotation) {
            GameObject tocopy = Prefabs.GetPrefab(name);
            if (tocopy == null) {
                Debug.LogError("Prefab not found");
                return null;
            }

            GameObject result = (GameObject)tocopy.Clone();
            result.Transform.position = position;
            result.Transform.rotation = rotation;
            if (tocopy.Transform.isStatic) result.Transform.SetStatic();

            result.Start();
            return result;
        }

        public virtual object Clone() {
            GameObject newObject = new GameObject(Name + "_clone_" + InstanceCount);// (((GameObject)Activator.CreateInstance(type);
            InstanceCount++;
            newObject.Transform.CopyFrom(this.Transform);
            newObject.Type = Type;
            newObject.Active = true;
            foreach (IComponent c in this.components) {
                newObject.AddComponent((IComponent)c.Clone());
            }
            newObject.Model = this.Model;
            return newObject;
        }

        public static void Destroy(GameObject o) {
            o.Active = false;
            allGameObjects.Remove(o);
            //TODO free up memory
        }

        public static async void Destroy(GameObject o, float lifetime) {// delete after some time
            await Task.Delay((int)(lifetime * 1000));
            Destroy(o);
        }

        public static GameObject[] All { get { return allGameObjects.ToArray(); } }

        //assumes name is unique
        public static GameObject FindGameObjectWithName(string name) { // THIS is dangerous method, as it could return null and cause unhandled exception
            foreach (GameObject o in allGameObjects) {
                if (o.Name.Equals(name)) return o;
            }
            Debug.LogError("could not find gameobject " + name);
            return null;
        }

        //returns all the gameobject that satisfy the tag
        public static GameObject[] FindGameObjectsByType(ObjectType type) {
            List<GameObject> result = new List<GameObject>();

            foreach (GameObject o in allGameObjects) {
                if (o.Type.Equals(type)) {
                    result.Add(o);
                }
            }

            if (result.Count == 0) {
                Debug.LogError("could not find any gameobject with tag " + type);
                return null;
            }

            return result.ToArray();
        }

        //COMPONENTS
        public void AddComponent(IComponent c) {
            components.Add(c);
            c.gameObject = this;
            //no components added at runtime, otherwise c.Start();
        }

        /*
        public T AddComponent<T>() where T : IComponent {
            IComponent c = new T();
        }*/

        public T GetComponent<T>() where T : IComponent {
            foreach (IComponent c in components) {
                if (c is T) return (T)c;
            }

            Debug.LogError("component not found " + typeof(T) + " inside " + Name);
            return default(T);
        }

    }
}
