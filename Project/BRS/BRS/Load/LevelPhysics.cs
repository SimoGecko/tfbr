using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Scripts;
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
        private QuadDrawer quadDrawer = null;
        private Game1 Demo;
        private List<GameObject> Players = new List<GameObject>();


        public LevelPhysics(Game1 game, PhysicsManager physics)
            : base(physics) {
            Demo = game;
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
                Model model = Content.Load<Model>("forklift");
                BoundingBox bb = BoundingBoxHelper.Calculate(model);
                JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min) * 2;

                GameObject forklift = new GameObject("player_" + i, new BoxShape(bbSize), model);
                forklift.Type = ObjectType.Player;
                forklift.Transform.Scale(2);
                forklift.AddComponent(new Player());
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());
                forklift.AddComponent(new PlayerLift());
                forklift.Transform.TranslateGlobal(Vector3.Right * 30 * i);
                forklift.Position = Conversion.ToJitterVector(new Vector3(31 * i, 1, 0));
                forklift.IsStatic = false;
                //forklift.Tag = BodyTag.DrawMe;


                PhysicsManager.World.AddBody(forklift);
                Players.Add(forklift);
            }



            //BASE
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_" + i, new BoxShape(new JVector(1, 1, 1)), Content.Load<Model>("cube"));
                playerBase.Type = ObjectType.Base;
                playerBase.AddComponent(new Base(i));
                playerBase.Transform.TranslateGlobal(Vector3.Right * 30 * i);
                playerBase.Position = Conversion.ToJitterVector(new Vector3(30 * i, 1, 0));
                playerBase.IsStatic = true;
                PhysicsManager.World.AddBody(playerBase);
            }

            BoxShape bShape = new BoxShape(0.5f, 4.0f, 2.0f);

            for (int i = 0; i < 10; i++) {
                GameObject body = new GameObject("domino_" + i, bShape);
                body.Position = new JVector((i+1) * 2.0f, 2, 0);
                PhysicsManager.World.AddBody(body);
            }


            // Dummy object at position (0/0/0) for debug-rendering.
            JBBox box = JBBox.SmallBox;
            GameObject dummy= new GameObject("dummy_object", new BoxShape(1,1,1));
            dummy.Position = JVector.Zero;
            dummy.IsStatic = true;
            dummy.Active = false;
            PhysicsManager.World.AddBody(dummy);
        }


        public void AddGround(GameObject parent) {
            GameObject groundPlane = new GameObject("groundplane", new BoxShape(new JVector(200, 20, 200)), Content.Load<Model>("gplane"));
            groundPlane.Transform.position = new Vector3(0, 0, 0);

            groundPlane.Position = new JVector(0, -10, 0);
            //groundPlane.Tag = BodyTag.DontDrawMe;
            groundPlane.IsStatic = true;
            groundPlane.Material.Restitution = 0.0f;
            groundPlane.Material.StaticFriction = 0.4f;

            PhysicsManager.World.AddBody(groundPlane);
            groundPlane.Material.KineticFriction = 0.0f;
        }
    }
}
