// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Scripts.Scenes;

namespace BRS.Engine {
    public class SceneManager {
        ////////// static class that allows to load and unload/change levels //////////

        static Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        static Scene currentScene;

        public static void Start() {
            scenes = new Dictionary<string, Scene>();


            Add("Level1", new Level1());
            Add("Level2", new Level2());
            Add("LevelPhysics", new LevelPhysics());
            
        }

        static void Add(string sceneName, Scene scene) {
            scenes.Add(sceneName, scene);
        }

        public static void Load(string sceneName) {
            if (currentScene != null) currentScene.Unload();
            currentScene = scenes[sceneName];
            if (currentScene != null) currentScene.Load();
        }
    }


    public class Scene {
        ////////// contains all gameobjects in the scene //////////

        //can create scene graph
        List<GameObject> objectsInScene = new List<GameObject>();

        /*
        protected PhysicsManager PhysicsManager { get; set; }

        public Scene(PhysicsManager physics) {
            PhysicsManager = physics;
        }*/




        public virtual void Load() { // levels inherit and fill this

        }

        public void Unload() {
            foreach (GameObject o in objectsInScene) GameObject.Destroy(o);
        }

        protected void Add(GameObject o) {
            objectsInScene.Add(o);
        }

        //CHANGE SCENE


    }

}
