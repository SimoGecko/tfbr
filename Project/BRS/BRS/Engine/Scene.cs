// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Scripts.Managers;
using BRS.Scripts.Scenes;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine {
    public class SceneManager {
        ////////// static class that allows to load and unload/change levels //////////

        static Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        static Scene currentScene;

        public static void Start() {
            scenes = new Dictionary<string, Scene>();

            //FILL here all the scenes in the game
            Add("Level1", new Level1());
            Add("Level2", new Level2());
            Add("Level3", new Level3());
            Add("LevelPhysics", new LevelPhysics());
        }

        public static void Update() {
            // For Simone to test other levels
            //if (Input.GetKeyDown(Keys.D1)) LoadScene("Level1");
            //if (Input.GetKeyDown(Keys.D2)) LoadScene("Level2");
            //if (Input.GetKeyDown(Keys.D3)) LoadScene("Level3");
            //if (Input.GetKeyDown(Keys.D4)) LoadScene("LevelPhysics");

            // For chris to load the different levels
            if (Input.GetKeyDown(Keys.D1)) LoadScene("Level1", 1);
            if (Input.GetKeyDown(Keys.D2)) LoadScene("Level1", 2);
            if (Input.GetKeyDown(Keys.D3)) LoadScene("Level1", 3);
            if (Input.GetKeyDown(Keys.D4)) LoadScene("Level1", 4);
            if (Input.GetKeyDown(Keys.D5)) LoadScene("Level1", 5);
        }

        static void Add(string sceneName, Scene scene) {
            scenes.Add(sceneName, scene);
        }

        public static void LoadScene(string sceneName, int sceneId = 1) {
            // Disable the physics-manager when loading the scene to avoid inconsistency between collision-handlings and loading
            PhysicsManager.Instance.IsActive = false;

            GameManager.LvlScene = sceneId;
            GameObject.ClearAll();
            currentScene = scenes[sceneName];
            Screen.SetupViewportsAndCameras(Graphics.gDM, currentScene.GetNumCameras());
            if (currentScene != null) currentScene.Load();
            StartScene();

            // Enable the physics-manager after all is loaded and positioned
            PhysicsManager.Instance.IsActive = true;
        }

        public static void StartScene() {
            foreach (GameObject go in GameObject.All) go.Awake();
            foreach (GameObject go in GameObject.All) go.Start();
        }
    }


    public class Scene {
        ////////// loads all gameobjects in a scene //////////

        public virtual void Load() { }// levels inherit and fill this

        public virtual int GetNumCameras() { return 1; } // override this for more than 1 player

        /*
        List<GameObject> objectsInScene = new List<GameObject>();//can create scene graph
        public void Unload() {
            foreach (GameObject o in objectsInScene) GameObject.Destroy(o);
        }

        protected void Add(GameObject o) {
            objectsInScene.Add(o);
        }
        */

    }

}
