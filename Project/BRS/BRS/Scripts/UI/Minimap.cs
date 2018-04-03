// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class Minimap : Component {
        ////////// draws the minimap including game elements at the correct location //////////

        // --------------------- VARIABLES ---------------------
        enum IconType { Triangle, Square, Circle, Star, House }

        //public
        const int MAPWIDTH = 428, MAPHEIGHT = 694; // of screenshot
        const int ICONSIZE = 64;
        const float mapScale = 1f;
        const int SMALLMAPWIDTH = 200; // squared pixel size of minimap

        //private
        static Rectangle mapDest, mapAreaVirgin, miniDest;
        static Vector3 upperLeftPt, lowerRightPt; // corners of physical map
        static Vector2 pivot;

        Texture2D mapSprite;
        Texture2D mapIcons;

        //to avoid passing them to the function
        Vector2 playerPos;
        float cameraRot;

        //reference
        public static Minimap instance;
        Transform playerT; // reference transform
        


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            mapSprite = File.Load<Texture2D>("Images/minimap/level1");
            mapIcons  = File.Load<Texture2D>("Images/minimap/icons");

            upperLeftPt = new Vector3(-25, 0, -75); //looked up in unity
            lowerRightPt = new Vector3(25, 0, 5);
            pivot = new Vector2(ICONSIZE / 2, ICONSIZE / 2);

            mapDest =  new Rectangle((int)(Screen.WIDTH / 2 - MAPWIDTH / 2 * mapScale), 10, (int)(MAPWIDTH * mapScale), (int)(MAPHEIGHT * mapScale));
            mapAreaVirgin = new Rectangle(0, 0, MAPWIDTH, MAPHEIGHT);
            miniDest = new Rectangle(720, 850, SMALLMAPWIDTH, SMALLMAPWIDTH);
            
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(SpriteBatch spriteBatch) {
            //MAP
            spriteBatch.Draw(mapSprite, mapDest, Color.White);

            //MONEY
            foreach (Vector3 pos in Elements.instance.AllMoneyPosition()) {
                spriteBatch.Draw(mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Circle), Color.Green, 0, pivot, .08f, SpriteEffects.None, 1f);
            }
            //CRATES
            foreach (Vector3 pos in Elements.instance.AllCratePosition()) {
                spriteBatch.Draw(mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Square), Color.SaddleBrown, 0, pivot, .12f, SpriteEffects.None, 1f);
            }
            //POWERUPS
            foreach (Vector3 pos in Elements.instance.AllPowerupPosition()) {
                spriteBatch.Draw(mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Star), Color.Blue, 0, pivot, .12f, SpriteEffects.None, 1f);
            }
            //BASES
            foreach (var b in Elements.instance.Bases()) {
                spriteBatch.Draw(mapIcons, Pos3D2Pix(b.transform.position), IconFromType(IconType.House), b.BaseColor, 0, pivot, .3f, SpriteEffects.None, 1f);
            }
            //PLAYERS
            foreach (var p in Elements.instance.Players()) {
                Vector3 pos = p.transform.position;
                float rotY = -p.transform.eulerAngles.Y;
                spriteBatch.Draw(mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Triangle), p.playerColor, MathHelper.ToRadians(rotY), pivot, .3f, SpriteEffects.None, 1f);
            }
        }


        //---------------------------------------------------------------------
        public void DrawSmall(SpriteBatch spriteBatch, int index) {
            //draw relative to player position
            playerT = Elements.instance.Player(index).transform;
            playerPos = Pos3D2Pix(playerT.position);
            cameraRot = Elements.instance.Player(index).camController.YRotation;

            //MAP
            Vector2 playerPosVirgin = Pos3D2PixVirgin(playerT.position);
            int scaledWidth = (int)(SMALLMAPWIDTH/mapScale);
            Rectangle sourceRect = new Rectangle((int)playerPosVirgin.X- scaledWidth / 2, (int)playerPosVirgin.Y- scaledWidth / 2, scaledWidth, scaledWidth);

            Point mapPivot = new Point((int)(mapScale * SMALLMAPWIDTH / 2), (int)(mapScale * SMALLMAPWIDTH / 2));
            Point mapPivot2 = new Point( SMALLMAPWIDTH / 2, SMALLMAPWIDTH / 2);

            Rectangle miniDest2 = miniDest; miniDest2.Location += mapPivot2;
            spriteBatch.Draw(mapSprite, miniDest2, sourceRect, Color.White, MathHelper.ToRadians(cameraRot), mapPivot.ToVector2(), SpriteEffects.None, 1); // todo make rotation
            //TODO cut map accordingly

            Vector2 finalPx;
            //MONEY
            foreach (Vector3 pos in Elements.instance.AllMoneyPosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(mapIcons, finalPx, IconFromType(IconType.Circle), Color.Green, 0, pivot, .08f, SpriteEffects.None, 1f);
            }
            //CRATES
            foreach (Vector3 pos in Elements.instance.AllCratePosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(mapIcons, finalPx, IconFromType(IconType.Square), Color.SaddleBrown, 0, pivot, .12f, SpriteEffects.None, 1f);
            }
            //POWERUPS
            foreach (Vector3 pos in Elements.instance.AllPowerupPosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(mapIcons, finalPx, IconFromType(IconType.Star), Color.Blue, 0, pivot, .12f, SpriteEffects.None, 1f);
            }
            //BASES
            foreach (var b in Elements.instance.Bases()) {
                Vector3 pos = b.transform.position;
                if (IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(mapIcons, finalPx, IconFromType(IconType.House), b.BaseColor, 0, pivot, .3f, SpriteEffects.None, 1f);
            }
            //PLAYERS
            foreach (var p in Elements.instance.Players()) {
                Vector3 pos = p.transform.position;
                float rotY = -p.transform.eulerAngles.Y;
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(mapIcons, finalPx, IconFromType(IconType.Triangle), p.playerColor, MathHelper.ToRadians(rotY+cameraRot), pivot, .3f, SpriteEffects.None, 1f);
            }
        }



        // queries
        Vector2 Pos3D2Pix(Vector3 pos) { // converts 3d position of object to pixel on screen inside minimap
            Vector3 L = upperLeftPt, R = lowerRightPt;
            float x0 = (pos.X - L.X) / (R.X - L.X);
            float y0 = (pos.Z - L.Z) / (R.Z - L.Z);
            Vector2 coeff = new Vector2(x0, y0);
            return mapDest.Evaluate(coeff).Round();
        }

        bool IsInsideMini(Vector3 pos, out Vector2 result) {
            result = miniDest.GetCenter() + (Pos3D2Pix(pos) - playerPos).Rotate(cameraRot); // center + delta
            return miniDest.Contains(result);
        }


        /*
        bool Pos3D2PixSmall(Vector3 pos, Transform player, out Vector2 result) { // converts 3d position of object to pixel on screen inside minimap relative to player
            //TODO consider rotation (nothing working yet)
            float width = (float)SMALLMAPWIDTH/MAPWIDTH*(lowerRightPt.X-upperLeftPt.X)/mapScale;//todo compute somehow

            Vector3 L = player.position - Vector3.One * width;
            Vector3 R = player.position + Vector3.One * width;
            float x0 = (pos.X - L.X) / (R.X - L.X);
            float y0 = (pos.Z - L.Z) / (R.Z - L.Z);
            Vector2 coeff = new Vector2(x0, y0);
            result = mapDest.Evaluate(coeff).Round();
            return (0 <= x0 && x0 <= 1 && 0 <= y0 && y0 <= 1);
        }*/

        Vector2 Pos3D2PixVirgin(Vector3 pos) { // converts 3d position of object to pixel on screen inside minimap WITHOUT any scaling or offset
            Vector3 L = upperLeftPt, R = lowerRightPt;
            float x0 = (pos.X - L.X) / (R.X - L.X);
            float y0 = (pos.Z - L.Z) / (R.Z - L.Z);
            Vector2 coeff = new Vector2(x0, y0);
            return mapAreaVirgin.Evaluate(coeff).Round();
        }

        Rectangle IconFromType(IconType type) {
            int col = (int)type % 4;
            int row = (int)type / 4;
            return new Rectangle(col * ICONSIZE, row * ICONSIZE, ICONSIZE, ICONSIZE);
        }

        /*
        Transform[] Players() {
            List<Transform> result = new List<Transform>();
            GameObject[] players = GameObject.FindGameObjectsWithTag(ObjectTag.Player);
            foreach (GameObject o in players) result.Add(o.transform);
            return result.ToArray();
        }

        Vector3[] Bases() {
            List<Vector3> result = new List<Vector3>();
            GameObject[] bases = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            foreach (GameObject o in bases) result.Add(o.transform.position);
            return result.ToArray();
        }*/


        // other

    }

}