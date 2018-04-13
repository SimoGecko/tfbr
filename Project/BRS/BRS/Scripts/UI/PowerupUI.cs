// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using BRS.Scripts.PowerUps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    class PowerupUI : Component {
        ////////// draws the powerups for the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        //string[] pngNames = { "bomb", "key", "capacity", "speed", "health", "shield", "trap" };

        //private
        //Dictionary<string, int> powerupStringToIndex = new Dictionary<string, int>();
        private readonly int _numPowerups = System.Enum.GetValues(typeof(PowerupType)).Length;
        //Texture2D[] powerupsPng;// = new Texture2D[numPowerups];
        private Texture2D _powerupsAtlas;
        private Rectangle[] _powerupsRectangle;
        private Texture2D _slot;
        private Texture2D smallButton;

        private PowerupUIStruct[] _powerupUi;

        // const
        const int AtlasWidth = 128;

        //reference
        public static PowerupUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _powerupUi = new PowerupUIStruct[GameManager.NumPlayers];
            for (int i = 0; i < GameManager.NumPlayers; i++) _powerupUi[i].CurrentPowerups = new int[0];

            //load pngs
            /*
            powerupsPng = new Texture2D[numPowerups];
            for (int i = 0; i < numPowerups; ++i) {
                powerupsPng[i] = File.Load<Texture2D>("Images/powerup/" + ((PowerupType)i).ToString() + "_pic");
                //if (!powerupStringToIndex.ContainsKey(pngNames[i])) powerupStringToIndex.Add(pngNames[i], i);
            }*/
            _powerupsAtlas = File.Load<Texture2D>("Images/powerup/powerups_clear"); // atlas
            _powerupsRectangle = new Rectangle[_numPowerups];
            for (int i = 0; i < _numPowerups; ++i) {
                int column = i%4;
                int row = i/4;
                _powerupsRectangle[i] = new Rectangle(column*AtlasWidth, row*AtlasWidth, AtlasWidth, AtlasWidth);
            }

            _slot = File.Load<Texture2D>("Images/powerup/powerup_slot");
            smallButton = File.Load<Texture2D>("Images/UI/small_button");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw(int index) { // TODO clean out this code
            UserInterface.Instance.DrawString("powerup", new Rectangle(0, 30, 100, 20), Align.Top, Align.Top, Align.Bottom, scale:.7f);
            UserInterface.Instance.DrawPicture(smallButton, new Rectangle(0, 55, 50, 50), new Rectangle(0, 0, 64, 64), Align.Top);

            if (_powerupUi[index].CurrentPowerups.Length > 0) { // it's going to draw just one
                foreach (int powerup in _powerupUi[index].CurrentPowerups) {
                    //UserInterface.instance.DrawPicture(destRect, powerupsPng[powerup]);
                    UserInterface.Instance.DrawPicture(smallButton, new Rectangle(0, 55, 50, 50), new Rectangle(0, 64, 64, 64), Align.Top, col:Color.Orange);
                    UserInterface.Instance.DrawPicture(_powerupsAtlas, new Rectangle(0, 57, 44, 44), _powerupsRectangle[powerup], Align.Top);
                    Suggestions.Instance.GiveCommand(index, new Rectangle(0, 130, 40, 40), XboxButtons.X, Align.Top);
                }
            }
        }


        public void DrawOLD2(int index) {
            /*
            Vector2 position = new Vector2(300, 100);
            position += Vector2.UnitX * UserInterface.Instance.GetOffset(index);
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, 50, 50);

            UserInterface.Instance.DrawPictureOLD(destRect, _slot);

            if (_powerupUi[index].CurrentPowerups.Length > 0) { // it's going to draw just one
                foreach (int powerup in _powerupUi[index].CurrentPowerups) {
                    //UserInterface.instance.DrawPicture(destRect, powerupsPng[powerup]);
                    UserInterface.Instance.DrawPicture(destRect, _powerupsAtlas, _powerupsRectangle[powerup]);
                    Suggestions.Instance.GiveCommand(index, destRect.Evaluate(new Vector2(.5f, 1)) + new Vector2(0, 30), XboxButtons.X);
                }
            }
            */
        }

        public void UpdatePlayerPowerupUI(int index, int[] powerupList) {
            _powerupUi[index].CurrentPowerups = powerupList;//.Add(name);
            //else     powerupUI[index].currentPowerup.Remove(name);
        }



        // queries



        // other

    }
}