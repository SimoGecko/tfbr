// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework.Content;

namespace BRS {
    /// <summary>
    /// Static class that contains all gameobjects in the scene
    /// </summary>
    class Scene {

        // Reference to the content-manager to load the assets
        protected ContentManager Content;

        // Reference to the game 
        protected readonly PhysicsManager PhysicsManager;

        // Ground of the level
        protected GameObject Ground { get; set; }

        // All players
        protected List<GameObject> Players = new List<GameObject>();


        public Scene(PhysicsManager physics) {
            PhysicsManager = physics;
        }

        public void Start() {
            Build();
        }

        public void Update() { }

        protected virtual void Build() {
            
        }

        public void GiveContent(ContentManager c) {
            Content = c;
        }

    }

}
