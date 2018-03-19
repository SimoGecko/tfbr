// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class Minimap : Component {
        ////////// minimap. stores position of important stuff and draws it on screen //////////

        // --------------------- VARIABLES ---------------------

        //public
        const int MAPWIDTH = 428, MAPHEIGHT = 694;
        const int ICONSIZE = 64;
        const float mapScale = .6f;
        const float iconScale = .3f;

        static Rectangle mapArea = new Rectangle((int)(Screen.WIDTH/2-MAPWIDTH/2*mapScale), 10, (int)(MAPWIDTH*mapScale), (int)(MAPHEIGHT*mapScale));
        static Vector3 upperLeftPt, lowerRightPt; // corners of physical map

        Texture2D mapSprite;
        Texture2D mapIcons;

        //private


        //reference
        public static Minimap instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            mapSprite = UserInterface.instance.LoadTexture2D("minimap");
            mapIcons  = UserInterface.instance.LoadTexture2D("minimap_icons");

            upperLeftPt  = new Vector3(-25, 0, -75);
            lowerRightPt = new Vector3( 25, 0,  5);
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(mapSprite, mapArea, Color.White);



            //MONEY
            Rectangle sourceRect = new Rectangle(ICONSIZE, ICONSIZE, ICONSIZE, ICONSIZE);
            foreach (Vector3 pos in Spawner.instance.AllMoneyPosition()) {
                spriteBatch.Draw(mapIcons, Pos3D2Pix(pos), sourceRect, Color.Green, 0, new Vector2(32, 32), .08f, SpriteEffects.None, 1f);
            }


            //CRATES
            sourceRect = new Rectangle(ICONSIZE, 0, ICONSIZE, ICONSIZE);
            foreach (Vector3 pos in Spawner.instance.AllCratePosition()) {
                spriteBatch.Draw(mapIcons, Pos3D2Pix(pos), sourceRect, Color.Brown, 0, new Vector2(32, 32), .12f, SpriteEffects.None, 1f);
            }


            //POWERUPS
            sourceRect = new Rectangle(0, ICONSIZE, ICONSIZE, ICONSIZE);
            foreach (Vector3 pos in Spawner.instance.AllPowerupPosition()) {
                spriteBatch.Draw(mapIcons, Pos3D2Pix(pos), sourceRect, Color.Blue, 0, new Vector2(32, 32), .12f, SpriteEffects.None, 1f);
            }


            //PLAYERS
            sourceRect = new Rectangle(0, 0, ICONSIZE, ICONSIZE);
            foreach (Transform player in Players()) {
                Vector3 position = player.position;
                float Yrot = -player.eulerAngles.Y;
                spriteBatch.Draw(mapIcons, Pos3D2Pix(position), sourceRect, Color.Orange, MathHelper.ToRadians(Yrot), new Vector2(32, 32), .3f, SpriteEffects.None, 1f);
            }

        }



        // queries
        Vector2 Pos3D2Pix(Vector3 pos) {
            Vector3 L = upperLeftPt, R = lowerRightPt;
            float x0 = (pos.X - L.X) / (R.X - L.X);
            float y0 = (pos.Z - L.Z) / (R.Z - L.Z);
            Vector2 coeff = new Vector2(x0, y0);
            return mapArea.Evaluate(coeff).Round();
        }

        Transform[] Players() {
            List<Transform> result = new List<Transform>();
            GameObject[] players = GameObject.FindGameObjectsWithTag("player");
            foreach (GameObject o in players) result.Add(o.Transform);
            return result.ToArray();
        }


        // other

    }

}