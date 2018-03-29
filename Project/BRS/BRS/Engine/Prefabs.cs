// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BRS.Load;

namespace BRS {
    static partial class Prefabs {
        ////////// static class that contains all GameObjects stored as prefabs and allows access to instantiate them //////////

        static Dictionary<string, GameObject> allprefabs = new Dictionary<string, GameObject>();
        public static Model emptymodel; // to represent a small transform without mesh - debug
        public static Model cubeModel, sphereModel;

        public static void Start() {
            emptymodel = File.Load<Model>("Models/primitives/empty");
            cubeModel = File.Load<Model>("Models/primitives/cube");
            sphereModel = File.Load<Model>("Models/primitives/sphere");
            BuildPrefabs();
        }

        public static void Update() {

        }

        //COMMANDS (do not modify)
        static void AddPrefab(GameObject o) {
            allprefabs.Add(o.name, o);
            //o.Start(); // no need as already called from gameobject
            o.active = false;
        }

        public static GameObject GetPrefab(string name) {
            if (!allprefabs.ContainsKey(name)) {
                Debug.LogError("No existing prefab with name " + name);
                return null;
            }
            return allprefabs[name];
        }
        
    }
}
