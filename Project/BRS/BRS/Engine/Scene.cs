// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using BRS.Scripts;

namespace BRS {
    class Scene {
        //static class that contains all gameobjects in the scene
        protected ContentManager Content;
        //can create scene graph
        
        /*public Scene(ContentManager c) {
            Content = c;
        }*/

        public void Start() {
            BuildScene();
        }

        public void Update() { }

        protected virtual void BuildScene() {
            
        }

        public void GiveContent(ContentManager c) {
            Content = c;
        }

    }

}
