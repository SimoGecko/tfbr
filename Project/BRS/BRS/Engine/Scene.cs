// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;

namespace BRS.Engine {
    public class Scene {
        ////////// static class that contains all gameobjects in the scene and allows to load new levels //////////

        //can create scene graph

        protected PhysicsManager PhysicsManager { get; set; }

        protected GameObject UiManager;
        protected GameObject Managers;

        public Scene(PhysicsManager physics) {
            PhysicsManager = physics;
        }

        public void Start() {
            Build();
            CreatePlayers();
            StartManagers();
        }

        public void Update() { }

        protected virtual void StartManagers() {

        }

        protected virtual void Build() { // levels inherit and fill this

        }

        protected virtual void CreatePlayers() {

        }

    }

}
