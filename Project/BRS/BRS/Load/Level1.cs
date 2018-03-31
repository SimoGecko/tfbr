// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts;
using System.IO;
using System.Threading.Tasks;
using BRS.Engine.Physics;

namespace BRS.Load {
    class Level1 : Scene {
        public Level1(PhysicsManager physics)
            : base(physics) { }

        

        protected override void Build() {
            ////////// scene setup for level1 //////////

            //MANAGER
            GameObject UIManager = new GameObject("UImanager"); // must be before the other manager
            UIManager.AddComponent(new BaseUI());
            UIManager.AddComponent(new PlayerUI());
            UIManager.AddComponent(new PowerupUI());
            UIManager.AddComponent(new GameUI());

            GameObject manager = new GameObject("manager");
            manager.AddComponent(new Elements());
            manager.AddComponent(new GameManager());
            manager.AddComponent(new RoundManager());
            manager.AddComponent(new Spawner());
            manager.AddComponent(new Minimap());

            



            //GROUND
            /*for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 3; y++) {
                    GameObject groundPlane = new GameObject("groundplane_" + x.ToString() + "_" + y.ToString(), File.Load<Model>("gplane"));
                    groundPlane.transform.position = new Vector3(x * 10-5, 0, -y * 10);
                    groundPlane.transform.SetStatic();
                }
            }*/


            //PLAYER
            for (int i=0; i<GameManager.numPlayers; i++) {
                GameObject player = new GameObject("player_"+i.ToString(), File.Load<Model>("Models/vehicles/forklift")); // for some reason the _tex version is much less shiny
                player.tag = ObjectTag.Player;
                player.AddComponent(new Player(i, i%2));
                player.transform.position = new Vector3(-5 + 10 * i, 0, 0);
                player.AddComponent(new SphereCollider(Vector3.Zero, .7f));
                //subcomponents
                player.AddComponent(new PlayerMovement());
                player.AddComponent(new PlayerAttack());
                player.AddComponent(new PlayerInventory());
                player.AddComponent(new PlayerPowerup());
                player.AddComponent(new PlayerStamina());
                player.AddComponent(new PlayerLift());

                //arrow
                GameObject arrow = new GameObject("arrow_" + i, File.Load<Model>("Models/elements/arrow"));
                arrow.AddComponent(new Arrow(player.transform, null, i));
                arrow.transform.Scale(.1f);
            }



            //BASE // TODO have this code make the base
            /*for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject playerBase = new GameObject("playerBase_"+i.ToString(), File.Load<Model>("cube"));
                playerBase.tag = "base";
                playerBase.AddComponent(new Base());
                playerBase.GetComponent<Base>().baseIndex = i;
                playerBase.transform.position = new Vector3(-5 + 10 * i, 0, 1);
                playerBase.transform.scale = new Vector3(3, 1, 1);
                playerBase.transform.SetStatic();
                playerBase.AddComponent(new BoxCollider(playerBase));

            }*/

            //VAULT
            GameObject vault = new GameObject("vault", File.Load<Model>("Models/primitives/cylinder"));
            vault.AddComponent(new Vault());
            vault.transform.position = new Vector3(5 , 1.5f, -62);
            vault.transform.scale = new Vector3(3, .5f, 3);
            vault.transform.eulerAngles = new Vector3(90, 0, 0);
            vault.AddComponent(new SphereCollider(Vector3.Zero, 3f));

            //other elements
            GameObject.Instantiate("speedpadPrefab", Vector3.Zero, Quaternion.Identity);

            //LOAD UNITY SCENE
            var task = Task.Run(() => { File.ReadFile("Load/UnitySceneData/lvl" + GameManager.lvlScene.ToString() + "/ObjectSceneUnity.txt"); });
            task.Wait();

            var task2 = Task.Run(() => { File.ReadHeistScene("Load/UnitySceneData/export1.txt"); });
            task2.Wait();

            GameObject[] bases = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            //Debug.Assert(bases.Length == 2, "there should be 2 bases");
            for (int i = 0; i < bases.Length; i++) {
                bases[i].AddComponent(new Base(i));
                bases[i].AddComponent(new BoxCollider(Vector3.Zero, Vector3.One*3));
                bases[i].transform.SetStatic();
            }
        }
    }
}