using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Vehicle;
using BRS.Scripts;
using BRS.Scripts.Physics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Load {
    // Todo: To be refactored
    public enum BodyTag { DrawMe, DontDrawMe }

    class LevelPhysics : Scene {
        // Todo: To be refactored
        private List<GameObject> Players = new List<GameObject>();
        private CarObject _car = null;
        private Game1 _game = null;


        public LevelPhysics(Game1 game, PhysicsManager physics)
            : base(physics) {
            _game = game;
        }


        /// <summary>
        /// Set up the scene for the physics-prototype
        /// </summary>
        protected override void Build() {
            // Add top-level manager
            GameObject rootScene = new GameObject("manager", null);
            rootScene.AddComponent(new CameraController());
            rootScene.AddComponent(new GameManager());
            rootScene.AddComponent(new Elements());
            rootScene.AddComponent(new Spawner());



            // Add the ground
            AddGround(rootScene);
            AddWalls();


            //PLAYER
            for (int i = 0; i < GameManager.numPlayers; i++) {

                GameObject forklift = new GameObject("player_" + i, Content.Load<Model>("forklift"));
                forklift.Type = ObjectType.Player;
                //forklift.transform.Scale(2);
                forklift.transform.TranslateGlobal(new Vector3(30 * i, 1f, 0));
                forklift.AddComponent(new Player(i, i % 2));
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());
                forklift.AddComponent(new PlayerLift());
                forklift.AddComponent(new PlayerStamina());
                forklift.AddComponent(new DynamicRigidBody(PhysicsManager));

                Players.Add(forklift);
            }



            //BASE
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_" + i, Content.Load<Model>("cube"));
                playerBase.Type = ObjectType.Base;
                playerBase.transform.TranslateGlobal(Vector3.Right * 30 * i);
                playerBase.AddComponent(new Base(i));
                playerBase.AddComponent(new StaticRigidBody(PhysicsManager));
            }
            

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 3; ++j) {
                    GameObject body = new GameObject("domino_" + i, Content.Load<Model>("cube"));
                    body.Type = ObjectType.Obstacle;
                    body.transform.TranslateGlobal(new Vector3(1.5f * (i + 1), 2 * (j + 1), -1.5f * (i + 1)));
                    body.AddComponent(new DynamicRigidBody(PhysicsManager));
                }
            }

            AddCar(new JVector(0, 5, -10));
            AddSoftBody();

            // Dummy object at position (0/0/0) for debug-rendering.
            GameObject dummy = new GameObject("dummy_object", Content.Load<Model>("cube"));
            dummy.Type = ObjectType.Default;
            dummy.AddComponent(new StaticRigidBody(PhysicsManager));
        }


        public void AddGround(GameObject parent) {
            for (int x = -2; x < 2; ++x) {
                for (int y = -2; y < 2; ++y) {
                    Material material = new Material();
                    material.Restitution = 0.0f;
                    material.StaticFriction = 0.4f;
                    material.KineticFriction = 1.0f;

                    GameObject groundPlane = new GameObject("groundplane", Content.Load<Model>("gplane"));
                    groundPlane.transform.position = new Vector3(x * 10, 0, y * 10);
                    groundPlane.AddComponent(new StaticRigidBody(PhysicsManager, true, material: material));
                }
            }
        }

        public void AddWalls() {
            float y = 1f;

            for (int x = 0; x < 40; ++x) {
                GameObject body = new GameObject("wall_" + (4 * x + 0), Content.Load<Model>("cube"));
                body.Type = ObjectType.Obstacle;
                body.transform.TranslateGlobal(new Vector3(-25 + x, y, -25));
                body.AddComponent(new StaticRigidBody(PhysicsManager));


                body = new GameObject("wall_" + (4 * x + 1), Content.Load<Model>("cube"));
                body.Type = ObjectType.Obstacle;
                body.transform.TranslateGlobal(new Vector3(-25 + x, y, 15));
                body.AddComponent(new StaticRigidBody(PhysicsManager));


                body = new GameObject("wall_" + (4 * x + 1), Content.Load<Model>("cube"));
                body.Type = ObjectType.Obstacle;
                body.transform.TranslateGlobal(new Vector3(-25, y, -25 + x));
                body.AddComponent(new StaticRigidBody(PhysicsManager));


                body = new GameObject("wall_" + (4 * x + 1), Content.Load<Model>("cube"));
                body.Type = ObjectType.Obstacle;
                body.transform.TranslateGlobal(new Vector3(15, y, -25 + x));
                body.AddComponent(new StaticRigidBody(PhysicsManager));
            }
        }


        public void AddCar(JVector position) {
            _car = new CarObject(_game, PhysicsManager);
            _game.Components.Add(_car);

            _car.carBody.Position = position;
        }

        public void AddSoftBody() {
            SoftBody cloth = new SoftBody(20, 20, 0.1f);

            // ##### Uncomment for selfcollision, all 3 lines
            //cloth.SelfCollision = true;
            //cloth.TriangleExpansion = 0.05f;
            //cloth.VertexExpansion = 0.05f;

            cloth.Translate(new JVector(0, 2, 0));

            cloth.Material.KineticFriction = 0.9f;
            cloth.Material.StaticFriction = 0.95f;

            cloth.VertexBodies[0].IsStatic = true;
            //cloth.VertexBodies[380].IsStatic = true;
            cloth.VertexBodies[19].IsStatic = true;
            //cloth.VertexBodies[399].IsStatic = true;

            cloth.SetSpringValues(SoftBody.SpringType.EdgeSpring, 0.1f, 0.01f);
            cloth.SetSpringValues(SoftBody.SpringType.ShearSpring, 0.1f, 0.03f);
            cloth.SetSpringValues(SoftBody.SpringType.BendSpring, 0.1f, 0.03f);

            // ###### Uncomment here for a better visualization
            // Demo.Components.Add(new ClothObject(Demo, cloth));

            PhysicsManager.World.AddBody(cloth);
        }
    }
}
