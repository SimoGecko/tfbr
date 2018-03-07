// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS {
    public class GameObject {
        //Class for objects in the world that have a transform, possibly a model and a list of components (scripts like in unity). Updated from main gameloop
        public Transform transform;
        List<IComponent> components;
        Model model;
        public bool active = true;
        public string name = "";
        public string tag = "default";

        static int instancecount = 0;

        public GameObject(string _name) {
            name = _name;
            transform = new Transform();
            components = new List<IComponent>();
            allgameobjects.Add(this);
        }

        public GameObject(string _name, Model _model) {
            name = _name;
            transform = new Transform();
            components = new List<IComponent>();
            model = _model;
            allgameobjects.Add(this);
        }

        public virtual void Start() {
            foreach (IComponent c in components) c.Start();
        }
        public virtual void Update() {
            if(active)
                foreach (IComponent c in components) c.Update();
        }

        //TODO make sure this callback is called from physics engine
        public virtual void OnCollisionEnter() { }
        public virtual void OnCollisionExit () { }

        public virtual void Draw(Camera cam) {
            if(model != null && active) {
                Utility.DrawModel(model, cam.View, cam.Proj, transform.World);
            }
        }

        public ModelMesh mesh { get { return model?.Meshes[0]; } } // assumes just 1 mesh per model


        //STATIC COMMANDS
        static List<GameObject> allgameobjects = new List<GameObject>();

        /*
        public static void Add(GameObject o) {
            allgameobjects.Add(o);
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

            result.transform.position = position;
            result.transform.rotation = rotation;

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
            GameObject newObject = new GameObject(name + "_clone_"+ instancecount);// (((GameObject)Activator.CreateInstance(type);
            instancecount++;
            newObject.transform.CopyFrom(this.transform);
            newObject.tag = tag;
            newObject.active = true;
            foreach (IComponent c in this.components) {
                newObject.AddComponent((IComponent)c.Clone());
            }
            newObject.model = this.model;
            return newObject;
            //return this.MemberwiseClone();
        }

        public static void Destroy(GameObject o) {
            o.active = false;
            allgameobjects.Remove(o);
            //TODO free up memory
        }

        public static async void Destroy(GameObject o, float lifetime) {// delete after some time
            await Task.Delay((int)(lifetime * 1000));
            Destroy(o);
        }

        public static GameObject[] All { get { return allgameobjects.ToArray(); } }

        //assumes name is unique // TODO enforce that
        public static GameObject FindGameObjectWithName(string name) { // THIS is dangerous method, as it could return null and cause unhandled exception
            foreach(GameObject o in allgameobjects) {
                if (o.name.Equals(name)) return o;
            }
            Debug.LogError("could not find gameobject " + name);
            return null;
        }

        //returns all the gameobject that satisfy the tag
        public static GameObject[] FindGameObjectsWithTag(string tag) {
            List<GameObject> result = new List<GameObject>();
            foreach (GameObject o in allgameobjects) {
                if (o.tag.Equals(tag)) result.Add(o);
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
            foreach(IComponent c in components) {
                if (c is T) return (T)c;
            }
            Debug.LogError("component not found " + typeof(T).ToString());
            return default(T);
        }
        
    }
}
