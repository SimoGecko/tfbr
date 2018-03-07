// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts;

namespace BRS.Load {
    class Level1 : Scene {
        protected override void BuildScene() {
            ////////// scene setup for level1 //////////

            //MANAGER
            GameObject manager = new GameObject("manager");
            manager.AddComponent(new CameraController());
            manager.AddComponent(new GameManager());
            manager.AddComponent(new Spawner());


            //GROUND
            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    GameObject groundPlane = new GameObject("groundplane_" + x.ToString() + "_" + y.ToString(), Content.Load<Model>("gplane"));
                    groundPlane.transform.position = new Vector3(x * 10, 0, y * 10 -15);
                }
            }


            //PLAYER
            for(int i=0; i<GameManager.numPlayers; i++) {
                GameObject forklift = new GameObject("player_"+i.ToString(), Content.Load<Model>("forklift"));
                forklift.tag = "player";
                forklift.transform.Scale(2);
                forklift.AddComponent(new Player());
                forklift.GetComponent<Player>().playerIndex = i;
                forklift.transform.TranslateGlobal(Vector3.Right * 30 * i);
            }



            //BASE
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_"+i.ToString(), Content.Load<Model>("cube"));
                playerBase.tag = "base";
                playerBase.AddComponent(new Base());
                playerBase.GetComponent<Base>().baseIndex = i;
                playerBase.transform.TranslateGlobal(Vector3.Right * 30 * i);
            }



        }
    }

}