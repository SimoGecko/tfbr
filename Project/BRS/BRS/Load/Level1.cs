﻿// (c) Simone Guggiari 2018
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
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 3; y++) {
                    GameObject groundPlane = new GameObject("groundplane_" + x.ToString() + "_" + y.ToString(), Content.Load<Model>("gplane"));
                    groundPlane.transform.position = new Vector3(x * 10-5, 0, -y * 10);
                }
            }


            //PLAYER
            for(int i=0; i<GameManager.numPlayers; i++) {
                GameObject forklift = new GameObject("player_"+i.ToString(), Content.Load<Model>("forklift"));
                forklift.tag = "player";
                //forklift.transform.Scale(2);
                forklift.AddComponent(new Player());
                forklift.GetComponent<Player>().playerIndex = i;
                forklift.transform.position = new Vector3(-5 + 10 * i, 0, 0);
                //forklift.transform.TranslateGlobal(Vector3.Right * (10 * i -5));
                //subcomponents
                forklift.AddComponent(new PlayerMovement());
                forklift.AddComponent(new PlayerAttack());
                forklift.AddComponent(new PlayerInventory());

            }



            //BASE
            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_"+i.ToString(), Content.Load<Model>("cube"));
                playerBase.tag = "base";
                playerBase.AddComponent(new Base());
                playerBase.GetComponent<Base>().baseIndex = i;
                //playerBase.transform.TranslateGlobal(Vector3.Right *( 10 * i-5));
                playerBase.transform.position = new Vector3(-5 + 10 * i, 0, 1);
                playerBase.transform.scale = new Vector3(3, 1, 1);

            }



        }
    }

}