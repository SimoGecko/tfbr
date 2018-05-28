// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.UI {
    class PlayerUI : Component {
        ////////// draws UI about player stats //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private PlayerUIStruct[] _playerUi;

        private Texture2D _forkliftIcon; // original was 256x256, new ones 320x320
        private Texture2D _barIcons; // each is 200x200

        //reference
        public static PlayerUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            Instance = this;
        }

        public override void Start() {
            _playerUi = new PlayerUIStruct[GameManager.NumPlayers];
            SetPlayerUIModel();

            _forkliftIcon = File.Load<Texture2D>("Images/UI/forklift_icon");
            _barIcons = File.Load<Texture2D>("Images/UI/bar_icons");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw2D(int index) {
            if (index == -1) return;
            if (!GameManager.GameActive) return;

            UserInterface.DrawString(_playerUi[index].Name, new Rectangle(20, 10, 330, 40), Align.TopLeft, scale: .5f, bold: true);
            UserInterface.DrawString("LVL" + _playerUi[index].level.ToString(), new Rectangle(160, 10, 330, 40), Align.TopLeft, scale: .5f, bold: true);

            //UserInterface.DrawPicture(_forkliftIcon, new Rectangle(20, 40, 100, 100), null, Align.TopLeft, flip: flip);
            UserInterface.DrawPicture(_playerUi[index].modelBack, new Rectangle(15, 35, 110, 110), null, Align.TopLeft);
            UserInterface.DrawPicture(_playerUi[index].modelPartColor, new Rectangle(15, 35, 110, 110), null, Align.TopLeft, col: _playerUi[index].modelColor);

            //capacity
            UserInterface.DrawPicture(_barIcons, new Rectangle(120, 57, 25, 25), new Rectangle(0, 0, 200, 200), Align.TopLeft);
            UserInterface.DrawString("carrying", new Rectangle(145, 32, 125, 25), Align.TopLeft, Align.TopLeft, Align.Bottom, scale: .7f);
            float capacityPercent = (float)_playerUi[index].CarryingWeight / _playerUi[index].MaxCapacity;
            UserInterface.DrawBarStriped(capacityPercent, new Rectangle(145, 57, 125, 25), Graphics.Green, Align.TopLeft);
            string capacityString = _playerUi[index].CarryingWeight + "/" + _playerUi[index].MaxCapacity;
            string valueString = Utility.IntToMoneyStringSimple(_playerUi[index].CarryingValue);
            UserInterface.DrawString(valueString, new Rectangle(275, 57, 75, 25), Align.TopLeft, Align.TopLeft, Align.Left);

            //fuel
            UserInterface.DrawPicture(_barIcons, new Rectangle(120, 107, 25, 25), new Rectangle(0, 200, 200, 200), Align.TopLeft);
            UserInterface.DrawString("fuel", new Rectangle(145, 82, 125, 25), Align.TopLeft, Align.TopLeft, Align.Bottom, scale: .7f);
            float staminaPercent = _playerUi[index].Stamina / _playerUi[index].MaxStamina;
            UserInterface.DrawBarStriped(staminaPercent, new Rectangle(145, 107, 125, 25), Graphics.Blue, Align.TopLeft);
            string staminaString = (int)(_playerUi[index].Stamina / _playerUi[index].MaxStamina*100) + "%";
            UserInterface.DrawString(staminaString, new Rectangle(275, 107, 75, 25), Align.TopLeft, Align.TopLeft, Align.Left);

            //small bars
            Vector2 screenPosition = Camera.GetCamera(index).WorldToScreenPoint(ElementManager.Instance.Player(index).transform.position+ Vector3.Up * 1.5f);
            Rectangle smallBar = new Rectangle(screenPosition.ToPoint() + new Point(-37, -8), new Point(75, 7)); // -8 instead of -68
            UserInterface.DrawBarStriped(capacityPercent, smallBar, Graphics.Green);
            smallBar.Y += 8;
            UserInterface.DrawBarStriped(staminaPercent, smallBar, Graphics.Blue);

            //draw name of all other players except self
            for(int playerIndex = 0; playerIndex < GameManager.NumPlayers; playerIndex++) {
                if (playerIndex == index) continue; // not myself
                Player otherPlayer = ElementManager.Instance.Player(playerIndex);
                Vector2 screenPosCam = Camera.GetCamera(index).WorldToScreenPoint(otherPlayer.transform.position+ Vector3.Up*1.5f);
                UserInterface.DrawString(otherPlayer.PlayerName, screenPosCam, Align.TopLeft, Align.Center, Align.Bottom, scale:.7f);
            }

            Player p = ElementManager.Instance.Player(index);
            string attentionNotification = p.Empty() ? "no fuel" : p.Full() ? "full" : " ";
            Rectangle notificationRect = new Rectangle(screenPosition.ToPoint() + new Point(0, -18), new Point(75, 20));
            UserInterface.DrawString(attentionNotification, notificationRect, Align.TopLeft, Align.Center, Align.Bottom, scale:.7f);

            //stamina button suggestions
            if (_playerUi[index].CanAttack) {
                ButtonsUI.Instance.GiveCommand(index, new Rectangle(70, 155, 40, 40), XboxButtons.A, Align.TopLeft);
            } else if (staminaPercent == 1) {
                ButtonsUI.Instance.GiveCommand(index, new Rectangle(70, 155, 40, 40), XboxButtons.RT, Align.TopLeft);
            }

        }

        public void UpdatePlayerUI(int index, float stamina, float maxStamina, int maxCapacity, int carryingValue, int carryingWeight, string name, bool canAttack, int lvl) {
            //TODO remove all unnecessary parameters
            // current
            _playerUi[index].CarryingValue = carryingValue;

            _playerUi[index].Stamina = stamina;
            _playerUi[index].CarryingWeight = carryingWeight;
            // max
            _playerUi[index].MaxStamina = maxStamina;
            _playerUi[index].MaxCapacity = maxCapacity;

            _playerUi[index].Name = name;
            _playerUi[index].CanAttack = canAttack;

            _playerUi[index].level = lvl;
        }

        // Set default Player Model Image and color (UI)
        public void SetPlayerUIModel() { 
            Texture2D defaultModelImage = File.Load<Texture2D>("Images/vehicles/fl_back");
            Texture2D defaultModelImagePartColor = File.Load<Texture2D>("Images/vehicles/fl_color");

            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                Color modelColor = i % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor;

                if (ScenesCommunicationManager.Instance != null) {
                    int currIdModel;
                    if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey("player_" + i)) {
                        currIdModel = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item2;
                        modelColor = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item3;
                    }
                    else {
                        currIdModel = 0;
                    }

                    defaultModelImage = ScenesCommunicationManager.Instance.ModelImages[currIdModel];
                    defaultModelImagePartColor = ScenesCommunicationManager.Instance.ModelImagesColorPart[currIdModel];
                }
                SetPlayerUIModelImage(i, defaultModelImage, defaultModelImagePartColor, modelColor);
            }

        }

        public void SetPlayerUIModelImage(int index, Texture2D model, Texture2D modelPartcol, Color col) {
            _playerUi[index].modelBack = model;
            _playerUi[index].modelPartColor = modelPartcol;
            _playerUi[index].modelColor = col;
        }

        // queries
        public string GetPlayerName(int index) {
            return _playerUi[index].Name;
        }


        // other

    }

    public struct PlayerUIStruct {
        // current
        public string Name;
        public int CarryingValue;

        public int CarryingWeight;
        public float Stamina;

        // max
        public float MaxStamina;
        public int MaxCapacity;

        // helper
        public bool CanAttack;

        // UI model
        public Texture2D modelBack;
        public Texture2D modelPartColor;
        public Color modelColor;

        public int level;
    }

}