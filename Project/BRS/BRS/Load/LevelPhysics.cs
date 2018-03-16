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
        private QuadDrawer quadDrawer = null;
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
                forklift.Transform.TranslateGlobal(Vector3.Right * 30 * i);
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
                GameObject body = new GameObject("domino_" + i, Content.Load<Model>("cube"));
                body.Type = ObjectType.Obstacle;
                body.Transform.TranslateGlobal(new Vector3(1.5f * (i+1), 2, -1.5f * (i+1)));
                body.AddComponent(new RigidBodyComponent(PhysicsManager, false));
            }


            //// Dummy object at position (0/0/0) for debug-rendering.
            //JBBox box = JBBox.SmallBox;
            //GameObject dummy= new GameObject("dummy_object", new BoxShape(1,1,1));
            //dummy.Position = JVector.Zero;
            //dummy.IsStatic = true;
            //dummy.Active = false;
            //PhysicsManager.World.AddBody(dummy);
        }


        public void AddGround(GameObject parent) {
            Material material = new Material();
            material.Restitution = 0.0f;
            material.StaticFriction = 0.4f;
            material.KineticFriction = 0.0f;

            GameObject groundPlane = new GameObject("groundplane", Content.Load<Model>("gplane"));
            groundPlane.Transform.position = new Vector3(0, 0, 0);
            groundPlane.AddComponent(new RigidBodyComponent(PhysicsManager, true, material));
        }
    }
}
