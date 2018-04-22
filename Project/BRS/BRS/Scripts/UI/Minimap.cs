// (c) Simone Guggiari 2018
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

        //private
        static Rectangle _mapDest, _mapAreaVirgin, _miniDest;
        static Vector2 _pivot;

        Texture2D _mapSprite;
        Texture2D _mapIcons;

        // const
        private const int MapWidth = 428, MapHeight = 694; // of screenshot
        private const int IconSize = 64;
        private const float MapScale = 1f;
        private const int SmallMapWidth = 200; // squared pixel size of minimap

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
            _miniDest = new Rectangle(720, 850, SmallMapWidth, SmallMapWidth);
            
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //TODO use correct Draw
        public void Draw(SpriteBatch spriteBatch) {
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
        public void DrawSmall(SpriteBatch spriteBatch, int index) {
            //draw relative to player position
            _playerT = ElementManager.Instance.Player(index).transform;
            _playerPos = Pos3D2Pix(_playerT.position);
            _cameraRot = ElementManager.Instance.Player(index).CamController.YRotation;

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
            foreach (Vector3 pos in ElementManager.Instance.AllMoneyPosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Circle), Color.Green, 0, _pivot, .08f, SpriteEffects.None, 1f);
            }
            //CRATES
            foreach (Vector3 pos in ElementManager.Instance.AllCratePosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Square), Color.SaddleBrown, 0, _pivot, .12f, SpriteEffects.None, 1f);
            }
            //POWERUPS
            foreach (Vector3 pos in ElementManager.Instance.AllPowerupPosition()) {
                if(IsInsideMini(pos, out finalPx))
                    spriteBatch.Draw(_mapIcons, finalPx, IconFromType(IconType.Star), Color.Blue, 0, _pivot, .12f, SpriteEffects.None, 1f);
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
            //result = _miniDest.Project(result);
            return _miniDest.Contains(result);
            //miniDest.
        }
        bool IsInsideMiniProject(Vector3 pos, out Vector2 result) {
            result = _miniDest.GetCenter() + (Pos3D2Pix(pos) - _playerPos).Rotate(_cameraRot); // center + delta
            result = _miniDest.Project(result);
            return true;
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
            return _mapAreaVirgin.Evaluate(PlayArea.Pos3DNormalized(pos)).Round();
        }

        Rectangle IconFromType(IconType type) {
            int col = (int)type % 4;
            int row = (int)type / 4;
            return new Rectangle(col * IconSize, row * IconSize, IconSize, IconSize);
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