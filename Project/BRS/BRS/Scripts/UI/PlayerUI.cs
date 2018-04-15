// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    class PlayerUI : Component {
        ////////// draws UI about player stats //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private PlayerUIStruct[] _playerUi;
        private Texture2D _forkliftIcon; // 256x256
        private Texture2D _barIcons; // 256x256

        //reference
        public static PlayerUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _playerUi = new PlayerUIStruct[GameManager.NumPlayers];

            _forkliftIcon = File.Load<Texture2D>("Images/UI/forklift_icon");
            _barIcons = File.Load<Texture2D>("Images/UI/bar_icons");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void Draw(int index) {
            if (index == 0) return;
            index--;

            bool flip = index % 2 != 0;

            UserInterface.DrawString(_playerUi[index].Name, new Rectangle(20, 10, 200, 30), Align.TopLeft, scale: .5f, bold: true, flip: flip);
            UserInterface.DrawPicture(_forkliftIcon, new Rectangle(20, 40, 80, 80), null, Align.TopLeft, flip: flip);

            //capacity
            UserInterface.DrawPicture(_barIcons, new Rectangle(100, 55, 20, 20), new Rectangle(0, 0, 200, 200), Align.TopLeft, flip: flip);
            UserInterface.DrawString("carrying", new Rectangle(120, 35, 100, 20), Align.TopLeft, Align.TopLeft, Align.Bottom, scale: .7f, flip: flip);
            float capacityPercent = (float)_playerUi[index].CarryingWeight / _playerUi[index].MaxCapacity;
            UserInterface.DrawBarStriped(capacityPercent, new Rectangle(120, 55, 100, 20), Graphics.Green, Align.TopLeft, flip: flip);
            string capacityString = _playerUi[index].CarryingWeight +  "/" + _playerUi[index].MaxCapacity;
            UserInterface.DrawString(capacityString, new Rectangle(225, 55, 60, 20), Align.TopLeft, Align.TopLeft, Align.Left, flip: flip);

            //fuel
            UserInterface.DrawPicture(_barIcons, new Rectangle(100, 95, 20, 20), new Rectangle(0, 200, 200, 200), Align.TopLeft, flip: flip);
            UserInterface.DrawString("fuel", new Rectangle(120, 75, 100, 20), Align.TopLeft, Align.TopLeft, Align.Bottom, scale: .7f, flip: flip);
            float staminaPercent = _playerUi[index].Stamina / _playerUi[index].MaxStamina;
            UserInterface.DrawBarStriped(staminaPercent, new Rectangle(120, 95, 100, 20), Graphics.Blue, Align.TopLeft, flip: flip);
            string staminaString = (int)(_playerUi[index].Stamina / _playerUi[index].MaxStamina*100) + "%";
            UserInterface.DrawString(staminaString, new Rectangle(225, 95, 60, 20), Align.TopLeft, Align.TopLeft, Align.Left, flip: flip);

            //small bars
            //TODO works only with player one
            Vector2 screenPosition = Camera.GetCamera(index).WorldToScreenPoint(ElementManager.Instance.Player(index).transform.position);
            Rectangle smallBar = new Rectangle(screenPosition.ToPoint() + new Point(-25, -60), new Point(50, 5));
            UserInterface.DrawBarStriped(capacityPercent, smallBar, Graphics.Green);
            smallBar.Y += 6;
            UserInterface.DrawBarStriped(staminaPercent, smallBar, Graphics.Blue);


            //suggestions
            //stamina button suggestions
            if (_playerUi[index].CanAttack) {
                Suggestions.Instance.GiveCommand(index, new Rectangle(60, 135, 40, 40), XboxButtons.A, Align.TopLeft, flip);
            } else if (staminaPercent == 1) {
                Suggestions.Instance.GiveCommand(index, new Rectangle(60, 135, 40, 40), XboxButtons.RT, Align.TopLeft, flip);
            }

        }

        public void UpdatePlayerUI(int index, float health, float maxHealth, float stamina, float maxStamina, int maxCapacity, int carryingValue, int carryingWeight, string name, bool canAttack) {
            // current
            _playerUi[index].CarryingValue = carryingValue;

            _playerUi[index].Health = health;
            _playerUi[index].Stamina = stamina;
            _playerUi[index].CarryingWeight = carryingWeight;
            // max
            _playerUi[index].MaxHealth = maxHealth;
            _playerUi[index].MaxStamina = maxStamina;
            _playerUi[index].MaxCapacity = maxCapacity;

            _playerUi[index].Name = name;
            _playerUi[index].CanAttack = canAttack;
        }



        // queries
        

        //i know we have git, this is faster for copy-paste
        public void DrawOldUI2(int index) {
            /*
            Vector2 position = new Vector2(130, 130);
            //position += Vector2.UnitX * UserInterface.GetOffset(index);
            Vector2 screenPosition = Camera.Main.WorldToScreenPoint(ElementManager.Instance.Player(index).transform.position);

            //UserInterface.DrawPicture(position, _forkliftIcon, _forkliftIcon.Bounds.GetCenter(), .5f);

            //name
            string playerName = _playerUi[index].Name;
            UserInterface.DrawStringOLD(position + new Vector2(-70, -110), playerName);

            //health
            float healthPercent = _playerUi[index].Health / _playerUi[index].MaxHealth;
            UserInterface.DrawBar(position + new Vector2(-70, -70), healthPercent, Color.Green);
            UserInterface.DrawBarSmall(screenPosition + new Vector2(-35, -60), healthPercent, Color.Green);

            //stamina
            float staminaPercent = _playerUi[index].Stamina / _playerUi[index].MaxStamina;
            UserInterface.DrawBar(position + new Vector2(-70, 50), staminaPercent, Color.Red);
            UserInterface.DrawBarSmall(screenPosition + new Vector2(-35, -55), staminaPercent, Color.Red);

            //stamina button suggestions
            if (_playerUi[index].CanAttack) {
                Suggestions.Instance.GiveCommand(index, position + new Vector2(80, 55), XboxButtons.A);
            } else if (staminaPercent == 1) {
                Suggestions.Instance.GiveCommand(index, position + new Vector2(80, 55), XboxButtons.RT);
            }

            //capacity
            float capacityPercent = (float)_playerUi[index].CarryingWeight / _playerUi[index].MaxCapacity;
            UserInterface.DrawBarVertical(position + new Vector2(-95, 60), capacityPercent, Color.Blue);
            UserInterface.DrawBarSmall(screenPosition + new Vector2(-35, -50), capacityPercent, Color.Blue);

            //cash
            string playerValueString = "$" + _playerUi[index].CarryingValue.ToString("N0");//ToString("#,##0")
            UserInterface.DrawStringOLD(position + new Vector2(50, -20), playerValueString);
            */
        }

        public void DrawOldUI(int index) {
            /*
            string playerValueString = "carrying: " + _playerUi[index].CarryingValue;
            UserInterface.DrawStringOLD(new Vector2(10 + offset, 120), playerValueString);

            //health
            float healthPercent = _playerUi[index].Health / _playerUi[index].MaxHealth;
            UserInterface.DrawBarBig(new Vector2(10 + offset, 170), healthPercent, Color.Green);
            string healthString = _playerUi[index].Health + "/" + _playerUi[index].MaxHealth;
            UserInterface.DrawStringOLD(new Vector2(75 + offset, 160), healthString);

            //stamina
            float staminaPercent = _playerUi[index].Stamina / _playerUi[index].MaxStamina;
            UserInterface.DrawBarBig(new Vector2(10 + offset, 220), staminaPercent, Color.Red);
            string staminaString = Math.Round(_playerUi[index].Stamina * 100) + "/" + _playerUi[index].MaxStamina * 100; // TODO make stamina range 100
            UserInterface.DrawStringOLD(new Vector2(75 + offset, 210), staminaString);

            //capacity
            float capacityPercent = (float)_playerUi[index].CarryingWeight / _playerUi[index].MaxCapacity;
            UserInterface.DrawBarBig(new Vector2(10 + offset, 270), capacityPercent, Color.Blue);
            string capacityString = _playerUi[index].CarryingWeight + "/" + _playerUi[index].MaxCapacity;
            UserInterface.DrawStringOLD(new Vector2(100 + offset, 260), capacityString);
            */
        }


        public string GetPlayerName(int index) {
            return _playerUi[index].Name;
        }


        // other

    }
}