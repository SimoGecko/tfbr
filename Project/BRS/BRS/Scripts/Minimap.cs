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
        const float scale = .6f;

        Rectangle mapArea = new Rectangle((int)(Screen.WIDTH/2-MAPWIDTH/2*scale), 10, (int)(MAPWIDTH*scale), (int)(MAPHEIGHT*scale));
        Vector3 lowerLeftPt, upperRightPt; // to set

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

            //TODO get actual values
            lowerLeftPt = new Vector3(-10, 0, 10);
            upperRightPt = new Vector3(10, 0, -40);
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(mapSprite, mapArea, Color.White);

            //player
            Rectangle sourceRect = new Rectangle(0, 0, ICONSIZE*2, ICONSIZE*2);
            spriteBatch.Draw(mapIcons, Vector2.One*100, sourceRect, Color.Red);
            //spriteBatch.Draw(mapIcons, Vector2.One*100, sourceRect, Color.Green, MathHelper.ToRadians(90), new Vector2(-64, -64), 1, SpriteEffects.None, 1f);

        }



        // queries
        Vector2 Pos3d2Pix(Vector3 pos) {
            Vector3 L = lowerLeftPt, R = upperRightPt;
            int x0 = (int)((pos.X - L.X) / (R.X - L.X));
            int y0 = (int)((pos.Y - L.Y) / (R.Y - L.Y));
            return new Vector2(x0, y0);
        }


        // other

    }

}