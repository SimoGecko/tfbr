﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    static partial class Prefabs {
        ////////// static class that contains all GameObjects stored as prefabs and allows access to instantiate them //////////

        static Dictionary<string, GameObject> _allprefabs = new Dictionary<string, GameObject>();
        public static Model Emptymodel; // to represent a small transform without mesh - debug
        public static Model CubeModel, SphereModel;

        public static void Start() {
            Emptymodel  = File.Load<Model>("Models/primitives/emptyCol");
            CubeModel   = File.Load<Model>("Models/primitives/cube");
            SphereModel = File.Load<Model>("Models/primitives/sphere");
            BuildPrefabs();
        }

        //COMMANDS (do not modify)
        static void AddPrefab(GameObject o) {
            _allprefabs.Add(o.name, o);
            //o.Start(); // no need as already called from gameobject
            o.active = false;
        }

        public static GameObject GetPrefab(string name) {
            if (!_allprefabs.ContainsKey(name)) {
                Debug.LogError("No existing prefab with name " + name);
                return null;
            }

            return _allprefabs[name];
        }
        
    }
}
