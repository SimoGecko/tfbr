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
        bool usePowerupColor = false;
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
        Color[] backgroundColor;

        //reference
        public static PowerupUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _powerupUi = new PowerupUIStruct[GameManager.NumPlayers];
            backgroundColor = new Color[GameManager.NumPlayers];
            for (int i = 0; i < GameManager.NumPlayers; i++) _powerupUi[i].CurrentPowerups = new int[0];

            //load pngs
            /*
            powerupsPng = new Texture2D[numPowerups];
            for (int i = 0; i < numPowerups; ++i) {
                powerupsPng[i] = File.Load<Texture2D>("Images/powerup/" + ((PowerupType)i).ToString() + "_pic");
                //if (!powerupStringToIndex.ContainsKey(pngNames[i])) powerupStringToIndex.Add(pngNames[i], i);
            }*/
            _powerupsAtlas = File.Load<Texture2D>("Images/powerup/powerups_new"); // atlas
            _powerupsRectangle = new Rectangle[_numPowerups];
            for (int i = 0; i < _numPowerups; ++i) {
                int column = i%4;
                int row = i/4;
                _powerupsRectangle[i] = new Rectangle(column*AtlasWidth, row*AtlasWidth, AtlasWidth, AtlasWidth);
            }

            _slot = File.Load<Texture2D>("Images/powerup/powerup_slot");
            smallButton = File.Load<Texture2D>("Images/powerup/small_button");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) { // TODO clean out this code
            if (index == 0) return;
            index--;

            UserInterface.DrawString("powerup", new Rectangle(0, 32, 125, 25), Align.Top, Align.Top, Align.Bottom, scale:.7f);
            UserInterface.DrawPicture(smallButton, new Rectangle(0, 57, 70, 70), new Rectangle(0, 0, 64, 64), Align.Top);

            if (_powerupUi[index].CurrentPowerups.Length > 0) { // it's going to draw just one
                foreach (int powerup in _powerupUi[index].CurrentPowerups) {
                    //UserInterface.instance.DrawPicture(destRect, powerupsPng[powerup]);
                    Color bgcol = (usePowerupColor) ? backgroundColor[index] : Color.Orange;
                    UserInterface.DrawPicture(smallButton, new Rectangle(0, 57, 70, 70), new Rectangle(0, 64, 64, 64), Align.Top, col:bgcol);
                    UserInterface.DrawPicture(_powerupsAtlas, new Rectangle(0, 65, 55, 55), _powerupsRectangle[powerup], Align.Top);
                    string powerupName = ((PowerupType)powerup).ToString();
                    UserInterface.DrawString(powerupName, new Rectangle(0, 127, 125, 25), Align.Top, Align.Top, Align.Top);
                    ButtonsUI.Instance.GiveCommand(index, new Rectangle(0, 172, 40, 40), XboxButtons.X, Align.Top);
                }
            }
        }

        public void UpdatePlayerPowerupUI(int index, int[] powerupList) {
            _powerupUi[index].CurrentPowerups = powerupList;//.Add(name);
            //else     powerupUI[index].currentPowerup.Remove(name);
        }



        // queries
        public void SetBackgroundColor(Color c, int index) {
            backgroundColor[index] = c;
        }


        // other

    }
    public struct PowerupUIStruct {
        //power ups
        public int[] CurrentPowerups;

    }

}