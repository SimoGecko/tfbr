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

        // const
        const int AtlasWidth = 128;
        const bool usePowerupColor = true;

        //private
        private Texture2D _powerupsAtlas;
        private Texture2D _smallButton;

        //reference
        public static PowerupUI Instance;
        private PowerupUIStruct[] _powerupUi;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _powerupUi = new PowerupUIStruct[GameManager.NumPlayers];
            for (int i = 0; i < GameManager.NumPlayers; i++) _powerupUi[i].CurrentPowerup = -1;//disable it

            _powerupsAtlas = File.Load<Texture2D>("Images/powerup/powerupIcons");
            _smallButton   = File.Load<Texture2D>("Images/powerup/button");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) { // TODO clean out this code
            if (index == -1) return;

            /*
            if(GameManager.state == GameManager.State.Ended && _powerupUi[index].CurrentPowerup != -1) {
                //SHOW POWERUP
                if (BuyPowerupManager.Instance.HasBoughtPowerup(index)){
                    int powerupIdx = _powerupUi[index].CurrentPowerup;
                    // COPY PASTE TO DRAW BUTTON AND POWERUP
                    UserInterface.DrawString("powerup", new Rectangle(0, 32, 125, 25), Align.Top, Align.Top, Align.Bottom, scale: .7f);
                    UserInterface.DrawPicture(_smallButton, new Rectangle(0, 57, 70, 70), new Rectangle(0, 0, 64, 64), Align.Top);
                    UserInterface.DrawPicture(_powerupsAtlas, new Rectangle(0, 65, 55, 55), RectangleFromIndex(powerupIdx), Align.Top);
                    Color bgcol = (usePowerupColor) ? Powerup.PowerupColor(powerupIdx) : Color.Orange;
                    UserInterface.DrawPicture(_smallButton, new Rectangle(0, 57, 70, 70), new Rectangle(0, 64, 64, 64), Align.Top, col: bgcol);
                    UserInterface.DrawPicture(_powerupsAtlas, new Rectangle(0, 65, 55, 55), RectangleFromIndex(powerupIdx), Align.Top);
                    string powerupName = ((PowerupType)powerupIdx).ToString();
                    UserInterface.DrawString(powerupName, new Rectangle(0, 127, 125, 25), Align.Top, Align.Top, Align.Top);
                }
                return;
            }*/

            if (!GameManager.GameActive/* && !BuyPowerupManager.Instance.HasBoughtPowerup(index)*/) return;

            UserInterface.DrawString("powerup", new Rectangle(0, 32, 125, 25), Align.Top, Align.Top, Align.Bottom, scale:.7f);
            UserInterface.DrawPicture(_smallButton, new Rectangle(0, 57, 70, 70), new Rectangle(0, 0, 64, 64), Align.Top);

            if (_powerupUi[index].CurrentPowerup != -1) { // if it has a powerup
                int powerupIdx = _powerupUi[index].CurrentPowerup;
                Color bgcol = (usePowerupColor) ? Powerup.PowerupColor(powerupIdx) : Color.Orange;

                UserInterface.DrawPicture(_smallButton, new Rectangle(0, 57, 70, 70), new Rectangle(0, 64, 64, 64), Align.Top, col:bgcol);
                UserInterface.DrawPicture(_powerupsAtlas, new Rectangle(0, 65, 55, 55), RectangleFromIndex(powerupIdx), Align.Top);

                string powerupName = ((PowerupType)powerupIdx).ToString();
                UserInterface.DrawString(powerupName, new Rectangle(0, 127, 125, 25), Align.Top, Align.Top, Align.Top);

                ButtonsUI.Instance.GiveCommand(index, new Rectangle(0, 172, 40, 40), XboxButtons.X, Align.Top);
            }
        }

        public void UpdatePlayerPowerupUI(int index, int[] powerupList) {
            if(powerupList.Length>0)
                _powerupUi[index].CurrentPowerup = powerupList[0];
            else
                _powerupUi[index].CurrentPowerup = -1;
        }



        // queries
        Rectangle RectangleFromIndex(int index) {
            int column = index % 4;
            int row = index / 4;
            return new Rectangle(column * AtlasWidth, row * AtlasWidth, AtlasWidth, AtlasWidth);
        }


        // other

    }

    public struct PowerupUIStruct {
        public int CurrentPowerup;
    }

}