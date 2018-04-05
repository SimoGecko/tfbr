﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using BRS.Scripts;

namespace BRS {
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
            BuildManagers();
            //UiManager.Start();
            //Managers.Start();
            //GameManager.instance.RestartCustom();
        }

        public void Update() { }

        protected virtual void BuildManagers() {

        }

        protected virtual void Build() { // levels inherit and fill this

        }

        protected virtual void CreatePlayers() {

        }

    }

}
