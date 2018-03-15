// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using System.Threading.Tasks;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS {
    public enum ObjectType { Player, Base, Default }

    /// <summary>
    /// Class for objects in the world that have a transform, possibly a model and a list of components (scripts like in unity). Updated from main gameloop
    /// </summary>
    public class GameObject : RigidBody{
        public Transform Transform;
        List<IComponent> components;
        private Model _model;
        public ModelMesh mesh { get { return _model?.Meshes[0]; } } // assumes just 1 mesh per model
        public bool Active { get; set; } = true;
        public string Name { private set; get; }
        public ObjectType Type { set; get; } = ObjectType.Default;

        static int InstanceCount = 0;

        public GameObject(string name, Model model = null)
            : this(name,  new BoxShape(0.0f, 0.0f, 0.0f), model)
        {
        }

        public GameObject(string name, Shape collisionShape, Model model = null)
            : base(collisionShape) {
            Name = name;
            Transform = new Transform();
            components = new List<IComponent>();
            _model = model;
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

        // TODO make sure this callback is called from physics engine
        public virtual void OnCollisionEnter() { }
        public virtual void OnCollisionExit() { }

        public virtual void Draw(Camera cam) {
            if (_model != null && Active) {
                Utility.DrawModel(_model, cam.View, cam.Proj, Transform.World);
            }
        }



        //STATIC COMMANDS
        static List<GameObject> allGameObjects = new List<GameObject>();

        /*
        public static void Add(GameObject o) {
            allGameObjects.Add(o);
        }

        
        public static GameObject Instantiate(string name) { // or prefab?
            GameObject tocopy = Prefabs.GetPrefab(name);
            if (tocopy == null) {
                Debug.LogError("Prefab not found");
                return null;
            }

            //TODO: Make a deep copy

            GameObject result = (GameObject)tocopy.Clone();
            Add(result);
            result.Start();
            return result;
        }*/

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
            //GameObject result = GameObject.DeepCopy(tocopy);
            //GameObject result = GameObject.MyClone(tocopy);

            result.Transform.position = position;
            result.Transform.rotation = rotation;

            result.Start();
            return result;
        }

        /*
        public static GameObject New(Model _model) {
            GameObject newobject = new GameObject(_model);
            //Add(newobject);
            return newobject;
        }*/


        /*
        public static GameObject DeepCopy(GameObject go) {
            return Utility.DeepClone<GameObject>(go);
        }
       


        public static GameObject MyClone(GameObject go) {
            GameObject result = (GameObject)go.MemberwiseClone();
            //do deep copy
            result.transform = new Transform();
            result.transform.CopyFrom(go.transform);
            result.components = new List<IComponent>();
            foreach (IComponent c in go.components) {
                //result.AddComponent((IComponent)c.Clone());
                result.AddComponent(Utility.DeepClone<IComponent>(c));
            }
            return result;
        }
        */


        public virtual object Clone() {
            GameObject newObject = new GameObject(Name + "_clone_" + InstanceCount, Shape);// (((GameObject)Activator.CreateInstance(type);
            InstanceCount++;
            newObject.Transform.CopyFrom(this.Transform);
            newObject.Type = Type;
            newObject.Active = true;
            foreach (IComponent c in this.components) {
                newObject.AddComponent((IComponent)c.Clone());
            }
            newObject._model = this._model;
            return newObject;
            //return this.MemberwiseClone();
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

        //assumes name is unique // TODO enforce that
        public static GameObject FindGameObjectWithName(string name) { // THIS is dangerous method, as it could return null and cause unhandled exception
            foreach (GameObject o in allGameObjects) {
                if (o.Name.Equals(name)) return o;
            }
            Debug.LogError("could not find gameobject " + name);
            return null;
        }

        //returns all the gameobject that satisfy the tag
        public static GameObject[] FindGameObjectsWithTag(string tag) {
            List<GameObject> result = new List<GameObject>();

            foreach (GameObject o in allGameObjects) {
                if (o.Type.Equals(tag)) result.Add(o);
            }

            if (result.Count == 0) {
                Debug.LogError("could not find any gameobject with tag " + tag);
                return null;
            }

            return result.ToArray();
        }

        //COMPONENTS
        public void AddComponent(IComponent c) {
            components.Add(c);
            c.gameObject = this;
        }

        public T GetComponent<T>() where T : IComponent {
            foreach (IComponent c in components) {
                if (c is T) return (T)c;
            }

            Debug.LogError("component not found " + typeof(T).ToString());
            return default(T);
        }

    }
}
