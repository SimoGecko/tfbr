using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.Scenes {
    class LevelPhysics : Scene {
        // Todo: To be refactored
        private readonly List<GameObject> Players = new List<GameObject>();

        /*public LevelPhysics(PhysicsManager physics)
            : base(physics) {
        }*/


        /// <summary>
        /// Set up the scene for the physics-prototype
        /// </summary>
        public override void Load() {
            // Add top-level manager
            GameObject UIManager = new GameObject("UImanager"); // must be before the other manager
            UIManager.AddComponent(new BaseUI());
            UIManager.AddComponent(new PlayerUI());
            UIManager.AddComponent(new PowerupUI());
            UIManager.AddComponent(new GameUI());

            GameObject rootScene = new GameObject("manager");
            //rootScene.AddComponent(new CameraController());
            rootScene.AddComponent(new ElementManager());
            rootScene.AddComponent(new GameManager());
            rootScene.AddComponent(new RoundManager());
            rootScene.AddComponent(new Spawner());
            rootScene.AddComponent(new Minimap());




            // Add the ground
            AddGround(rootScene);
            AddWalls();


            //PLAYER
            for (int i = 0; i < GameManager.NumPlayers; i++) {

                GameObject forklift = new GameObject("player_" + i, File.Load<Model>("Models/vehicles/forklift"));
                forklift.tag = ObjectTag.Player;
                //forklift.transform.Scale(2);
                Vector3 startPos = new Vector3(30 * i, 1f, 0);

                forklift.transform.TranslateGlobal(startPos);
                forklift.AddComponent(new Player(i, i % 2, startPos));
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());
                forklift.AddComponent(new PlayerLift());
                forklift.AddComponent(new PlayerStamina());
                forklift.AddComponent(new MovingRigidBody());

                Players.Add(forklift);
            }



            //BASE
            for (int i = 0; i < GameManager.NumPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_" + i, File.Load<Model>("Models/primitives/cube"));
                playerBase.tag = ObjectTag.Base;
                playerBase.transform.TranslateGlobal(Vector3.Right * 30 * i);
                playerBase.AddComponent(new Base(i));
                playerBase.AddComponent(new StaticRigidBody());
            }


            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 3; ++j) {
                    GameObject body = new GameObject("domino_" + i, File.Load<Model>("Models/primitives/cube"));
                    body.tag = ObjectTag.Obstacle;
                    body.transform.TranslateGlobal(new Vector3(1.5f * (i + 1), 2 * (j + 1), -1.5f * (i + 1)));
                    body.AddComponent(new DynamicRigidBody(pureCollider: true));
                }
            }

            //AddCar(new JVector(-2, 0.8f, -2));
            AddSoftBody();

            //// Dummy object at position (0/0/0) for debug-rendering.
            //GameObject dummy = new GameObject("dummy_object", File.Load<Model>("Models/primitives/cube"));
            //dummy.tag = ObjectTag.Default;
            //dummy.AddComponent(new StaticRigidBody(PhysicsManager, tag: BodyTag.DrawMe));
        }


        public void AddGround(GameObject parent) {
            Material material = new Material();
            material.Restitution = 1.0f;
            material.StaticFriction = 0.4f;
            material.KineticFriction = 10.0f;

            //for (int x = -2; x < 2; ++x) {
            //    for (int y = -2; y < 2; ++y) {

            //        GameObject groundPlane = new GameObject("groundplane", File.Load<Model>("Models/vehicles/gplane"));
            //        groundPlane.transform.position = new Vector3(x * 10, 0, y * 10);
            //        //groundPlane.AddComponent(new StaticRigidBody(PhysicsManager, true, material: material));
            //    }
            //}
            GameObject groundPlane = new GameObject("groundplane", File.Load<Model>("Models/vehicles/gplane"));
            groundPlane.transform.position = new Vector3(5*10,0,5*10);
            groundPlane.tag = ObjectTag.Ground;

            Collider rbGround = new Collider(new BoxShape(5*10, 10, 5*10));
            rbGround.Position = new JVector(0, -5, 0);
            rbGround.IsStatic = true;
            rbGround.Material = material;
            rbGround.Tag = BodyTag.DontDrawMe;
            rbGround.GameObject = groundPlane;
            PhysicsManager.Instance.World.AddBody(rbGround);
        }

        public void AddWalls() {
            float y = 0.5f;

            for (int x = 0; x < 40; ++x) {
                GameObject body = new GameObject("wall_" + (4 * x), File.Load<Model>("Models/primitives/cube"));
                body.tag = ObjectTag.Obstacle;
                body.transform.TranslateGlobal(new Vector3(-25 + x, y, -25));
                body.AddComponent(new StaticRigidBody());


                body = new GameObject("wall_" + (4 * x + 1), File.Load<Model>("Models/primitives/cube"));
                body.tag = ObjectTag.Obstacle;
                body.transform.TranslateGlobal(new Vector3(-25 + x, y, 15));
                body.AddComponent(new StaticRigidBody());


                body = new GameObject("wall_" + (4 * x + 1), File.Load<Model>("Models/primitives/cube"));
                body.tag = ObjectTag.Obstacle;
                body.transform.TranslateGlobal(new Vector3(-25, y, -25 + x));
                body.AddComponent(new StaticRigidBody());


                body = new GameObject("wall_" + (4 * x + 1), File.Load<Model>("Models/primitives/cube"));
                body.tag = ObjectTag.Obstacle;
                body.transform.TranslateGlobal(new Vector3(15, y, -25 + x));
                body.AddComponent(new StaticRigidBody());
            }
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

            PhysicsManager.Instance.World.AddBody(cloth);
        }
    }
}
