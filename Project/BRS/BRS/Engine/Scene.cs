// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;

namespace BRS.Engine {
    public class Scene {
        ////////// static class that contains all gameobjects in the scene and allows to load new levels //////////
        //TODO make loading levels easy

        //can create scene graph
        static Dictionary<string, Scene> scenes;
        List<GameObject> objectsInScene;

        protected PhysicsManager PhysicsManager { get; set; }

        public Scene(PhysicsManager physics) {
            PhysicsManager = physics;
        }

        public void Start() {
            Build();
        }

        public void Update() { }


        protected virtual void Build() { // levels inherit and fill this

        }

    }

}
