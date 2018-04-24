﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    class Minimap : Component {
        ////////// draws the minimap including game elements at the correct location //////////

        // --------------------- VARIABLES ---------------------
        enum IconType { Triangle, Square, Circle, Star, House }

        //public
        const bool rotateMinimap = false;

        //private
        static Rectangle _mapDest, _mapAreaVirgin, _miniDest;
        static Vector2 _pivot;

        Texture2D _mapSprite;
        Texture2D _mapIcons;

        // const
        private const int MapWidth = 603, MapHeight = 770; // of screenshot
        private const int IconSize = 64;
        private const float MapScale = 1f;
        private const int SmallMapWidth = 250; // squared pixel size of minimap

        //to avoid passing them to the function
        private Vector2 _playerPos;
        private float _cameraRot;

        //reference
        public static Minimap Instance;
        private Transform _playerT; // reference transform
        


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _mapSprite = File.Load<Texture2D>("Images/minimap/level1");
            _mapIcons  = File.Load<Texture2D>("Images/minimap/icons");
            
            _pivot = new Vector2(IconSize / 2, IconSize / 2);

            _mapDest =  new Rectangle((int)(Screen.Width / 2 - MapWidth / 2 * MapScale), 10, (int)(MapWidth * MapScale), (int)(MapHeight * MapScale));
            _mapAreaVirgin = new Rectangle(0, 0, MapWidth, MapHeight);
            _miniDest = new Rectangle(-20, -20, SmallMapWidth, SmallMapWidth);
            _miniDest = UserInterface.AlignRect(Align.BotRight, Align.BotRight, _miniDest);
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw(int i) {
            if (i == 0) return;
            DrawSmall(UserInterface.sB, i-1);
        }


        public void Draw(SpriteBatch spriteBatch) { // draws the whole map in the middle
            //MAP
            spriteBatch.Draw(_mapSprite, _mapDest, Color.White);

            //MONEY
            foreach (Vector3 pos in ElementManager.Instance.AllMoneyPosition()) {
                spriteBatch.Draw(_mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Circle), Color.Green, 0, _pivot, .08f, SpriteEffects.None, 1f);
            }
            //CRATES
            foreach (Vector3 pos in ElementManager.Instance.AllCratePosition()) {
                spriteBatch.Draw(_mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Square), Color.SaddleBrown, 0, _pivot, .12f, SpriteEffects.None, 1f);
            }
            //POWERUPS
            foreach (Vector3 pos in ElementManager.Instance.AllPowerupPosition()) {
                spriteBatch.Draw(_mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Star), Color.Blue, 0, _pivot, .12f, SpriteEffects.None, 1f);
            }
            //BASES
            foreach (var b in ElementManager.Instance.Bases()) {
                spriteBatch.Draw(_mapIcons, Pos3D2Pix(b.transform.position), IconFromType(IconType.House), b.BaseColor, 0, _pivot, .3f, SpriteEffects.None, 1f);
            }
            //PLAYERS
            foreach (var p in ElementManager.Instance.Players()) {
                Vector3 pos = p.transform.position;
                float rotY = -p.transform.eulerAngles.Y;
                spriteBatch.Draw(_mapIcons, Pos3D2Pix(pos), IconFromType(IconType.Triangle), p.PlayerColor, MathHelper.ToRadians(rotY), _pivot, .3f, SpriteEffects.None, 1f);
            }
        }


        //---------------------------------------------------------------------
        public void DrawSmall(SpriteBatch spriteBatch, int index) { // drawp it relative to the player
            //draw relative to player position
            _playerT = ElementManager.Instance.Player(index).transform;
            _playerPos = Pos3D2Pix(_playerT.position);
            _cameraRot = rotateMinimap ? ElementManager.Instance.Player(index).CamController.YRotation : 0f;

            //MAP
            Vector2 playerPosVirgin = Pos3D2PixVirgin(_playerT.position);
            int scaledWidth = (int)(SmallMapWidth/MapScale);
            Rectangle sourceRect = new Rectangle((int)playerPosVirgin.X- scaledWidth / 2, (int)playerPosVirgin.Y- scaledWidth / 2, scaledWidth, scaledWidth);

            Point mapPivot = new Point((int)(MapScale * SmallMapWidth / 2), (int)(MapScale * SmallMapWidth / 2));
            Point mapPivot2 = new Point( SmallMapWidth / 2, SmallMapWidth / 2);

            Rectangle miniDest2 = _miniDest; miniDest2.Location += mapPivot2;
            spriteBatch.Draw(_mapSprite, miniDest2, sourceRect, Color.White, MathHelper.ToRadians(_cameraRot), mapPivot.ToVector2(), SpriteEffects.None, 1); // todo make rotation
            //TODO cut map accordingly

            Vector2 finalPx;
            //MONEY
            foreach (Vector3 pos in ElementManager.Instance.AllCashPosition()) {
                if(IsInsideMini(pos, out finalPx)) {
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Circle), Graphics.Green, 0, _pivot, .08f, SpriteEffects.None, 1f);
                }
            }
            foreach (Vector3 pos in ElementManager.Instance.AllGoldPosition()) {
                if (IsInsideMini(pos, out finalPx)) {
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Circle), Graphics.Yellow, 0, _pivot, .08f, SpriteEffects.None, 1f);
                }
            }
            foreach (Vector3 pos in ElementManager.Instance.AllDiamondPosition()) {
                if (IsInsideMini(pos, out finalPx)) {
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Circle), Color.LightBlue, 0, _pivot, .08f, SpriteEffects.None, 1f);
                }
            }
            //CRATES
            foreach (Vector3 pos in ElementManager.Instance.AllCratePosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Square), Color.SaddleBrown, 0, _pivot, .12f, SpriteEffects.None, 1f);
            }
            //POWERUPS
            foreach (Vector3 pos in ElementManager.Instance.AllPowerupPosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Star), Graphics.Blue, 0, _pivot, .12f, SpriteEffects.None, 1f);
            }
            //BASES
            foreach (var b in ElementManager.Instance.Bases()) {
                Vector3 pos = b.transform.position;
                if (IsInsideMiniProject(pos, out finalPx))
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.House), b.BaseColor, 0, _pivot, .3f, SpriteEffects.None, 1f);
            }
            //PLAYERS
            foreach (var p in ElementManager.Instance.Players()) {
                Vector3 pos = p.transform.position;
                float rotY = -p.transform.eulerAngles.Y;
                if(IsInsideMiniProject(pos, out finalPx))
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Triangle), p.PlayerColor, MathHelper.ToRadians(rotY+_cameraRot), _pivot, .3f, SpriteEffects.None, 1f);
            }
        }



        // queries
        Vector2 Pos3D2Pix(Vector3 pos) { // converts 3d position of object to pixel on screen inside minimap
            return _mapDest.Evaluate(PlayArea.Pos3DNormalized(pos)).Round();
        }

        bool IsInsideMini(Vector3 pos, out Vector2 result) {
            result = _miniDest.GetCenter() + (Pos3D2Pix(pos) - _playerPos).Rotate(_cameraRot); // center + delta
            return _miniDest.Contains(result);
        }
        bool IsInsideMiniProject(Vector3 pos, out Vector2 result) {
            result = _miniDest.GetCenter() + (Pos3D2Pix(pos) - _playerPos).Rotate(_cameraRot); // center + delta
            result = _miniDest.Project(result);
            return true;
        }

        Vector2 Pos3D2PixVirgin(Vector3 pos) { // converts 3d position of object to pixel on screen inside minimap WITHOUT any scaling or offset
            return _mapAreaVirgin.Evaluate(PlayArea.Pos3DNormalized(pos)).Round();
        }

        Rectangle IconFromType(IconType type) {
            int col = (int)type % 4;
            int row = (int)type / 4;
            return new Rectangle(col * IconSize, row * IconSize, IconSize, IconSize);
        }

        // other

    }

}