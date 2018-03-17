using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Scripts;
using BRS.Scripts.Physics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Load {
    // Todo: To be refactored
    public enum BodyTag { DrawMe, DontDrawMe }

    class LevelPhysics : Scene {
        // Todo: To be refactored
        private List<GameObject> Players = new List<GameObject>();


        public LevelPhysics(PhysicsManager physics)
            : base(physics) {
        }


        /// <summary>
        /// Set up the scene for the physics-prototype
        /// </summary>
        protected override void Build() {
            // Add top-level manager
            GameObject rootScene = new GameObject("manager", null);
            rootScene.AddComponent(new CameraController());
            rootScene.AddComponent(new GameManager());
            //rootScene.AddComponent(new Spawner());



            // Add the ground
            AddGround(rootScene);


            //PLAYER
            for (int i = 0; i < GameManager.numPlayers; i++) {

                GameObject forklift = new GameObject("player_" + i, Content.Load<Model>("forklift"));
                forklift.Type = ObjectType.Player;
                //forklift.Transform.Scale(2);
                forklift.Transform.TranslateGlobal(new Vector3(30 * i, 0, 0));
                forklift.AddComponent(new Player(i));
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());
                forklift.AddComponent(new RigidBodyComponent(PhysicsManager, false));

                Players.Add(forklift);
            }



            //BASE
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_" + i, Content.Load<Model>("cube"));
                playerBase.Type = ObjectType.Base;
                playerBase.Transform.TranslateGlobal(Vector3.Right * 30 * i);
                playerBase.AddComponent(new Base(i));
                playerBase.AddComponent(new RigidBodyComponent(PhysicsManager, true));
            }

            BoxShape bShape = new BoxShape(0.5f, 4.0f, 2.0f);

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 3; ++j) {
                    GameObject body = new GameObject("domino_" + i, Content.Load<Model>("cube"));
                    body.Type = ObjectType.Obstacle;
                    body.Transform.TranslateGlobal(new Vector3(1.5f * (i + 1), 2 * (j + 1), -1.5f * (i + 1)));
                    body.AddComponent(new RigidBodyComponent(PhysicsManager, false));
                }
            }

            // Dummy object at position (0/0/0) for debug-rendering.
            GameObject dummy = new GameObject("dummy_object", Content.Load<Model>("cube"));
            dummy.Type = ObjectType.Default;
            dummy.AddComponent(new RigidBodyComponent(PhysicsManager, true, false));
        }


        public void AddGround(GameObject parent) {
            for (int x = -2; x < 2; ++x) {
                for (int y = -2; y < 2; ++y) {
                    Material material = new Material();
                    material.Restitution = 0.0f;
                    material.StaticFriction = 0.4f;
                    material.KineticFriction = 1.0f;

                    GameObject groundPlane = new GameObject("groundplane", Content.Load<Model>("gplane"));
                    groundPlane.Transform.position = new Vector3(x * 10, 0, y*10);
                    groundPlane.AddComponent(new RigidBodyComponent(PhysicsManager, true, material: material));
                }
            }
        }
    }
}
