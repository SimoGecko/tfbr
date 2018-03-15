// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts;

namespace BRS.Load {
    class Level1 : Scene {
        public Level1(PhysicsManager physics)
            : base(physics) { }
        /// <summary>
        /// Scene setup for level1
        /// </summary>
        protected override void Build() {
            

            //MANAGER
            GameObject manager = new GameObject("manager");
            manager.AddComponent(new CameraController());
            manager.AddComponent(new GameManager());
            manager.AddComponent(new Spawner());


            //GROUND
            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    GameObject groundPlane = new GameObject("groundplane_" + x.ToString() + "_" + y.ToString(), model: Content.Load<Model>("gplane"));
                    groundPlane.Transform.position = new Vector3(x * 10, 0, y * 10 -15);
                }
            }


            //PLAYER
            for(int i=0; i<GameManager.numPlayers; i++) {
                GameObject forklift = new GameObject("player_"+i.ToString(), model: Content.Load<Model>("forklift"));
                forklift.Type = ObjectType.Player;
                forklift.Transform.Scale(2);
                forklift.AddComponent(new Player(i));
                forklift.Transform.TranslateGlobal(Vector3.Right * 30 * i);
            }



            //BASE
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_"+i.ToString(), model: Content.Load<Model>("cube"));
                playerBase.Type = ObjectType.Base;
                playerBase.AddComponent(new Base(i));
                playerBase.Transform.TranslateGlobal(Vector3.Right * 30 * i);
            }



        }
    }

}