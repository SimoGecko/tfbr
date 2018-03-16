// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using BRS.Scripts;

namespace BRS {
    class Scene {
        ////////// static class that contains all gameobjects in the scene and allows to load new levels //////////

        protected ContentManager Content;
        //can create scene graph

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

        public void GiveContent(ContentManager c) {
            Content = c;
        }

    }

}
