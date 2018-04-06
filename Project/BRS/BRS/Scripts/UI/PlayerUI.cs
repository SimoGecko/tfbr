// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BRS.Scripts {
    class PlayerUI : Component {
        ////////// draws UI about player stats //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private PlayerUIStruct[] _playerUi;
        private Texture2D _forkliftIcon;

        //reference
        public static PlayerUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _playerUi = new PlayerUIStruct[GameManager.NumPlayers];

            _forkliftIcon = File.Load<Texture2D>("Images/UI/forklift_icon");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(int index) {
            Vector2 position = new Vector2(130, 130);
            position += Vector2.UnitX * UserInterface.Instance.GetOffset(index);
            Vector2 screenPosition = Camera.Main.WorldToScreenPoint(Elements.Instance.Player(index).transform.position);

            UserInterface.Instance.DrawPicture(position, _forkliftIcon, _forkliftIcon.Bounds.GetCenter(), .5f);

            //name
            string playerName = _playerUi[index].Name;
            UserInterface.Instance.DrawString(position + new Vector2(-70, -110), playerName);

            //health
            float healthPercent = _playerUi[index].Health / _playerUi[index].MaxHealth;
            UserInterface.Instance.DrawBar(position + new Vector2(-70, -70), healthPercent, Color.Green);
            UserInterface.Instance.DrawBarSmall(screenPosition + new Vector2(-35, -60), healthPercent, Color.Green);

            //stamina
            float staminaPercent = _playerUi[index].Stamina / _playerUi[index].MaxStamina;
            UserInterface.Instance.DrawBar(position + new Vector2(-70, 50), staminaPercent, Color.Red);
            UserInterface.Instance.DrawBarSmall(screenPosition + new Vector2(-35, -55), staminaPercent, Color.Red);

            //stamina button suggestions
            if (_playerUi[index].CanAttack) {
                Suggestions.Instance.GiveCommand(index, position + new Vector2(80, 55), XboxButtons.A);
            } else if (staminaPercent == 1) {
                Suggestions.Instance.GiveCommand(index, position + new Vector2(80, 55), XboxButtons.RT);
            }

            //capacity
            float capacityPercent = (float)_playerUi[index].CarryingWeight / _playerUi[index].MaxCapacity;
            UserInterface.Instance.DrawBarVertical(position + new Vector2(-95 , 60), capacityPercent, Color.Blue);
            UserInterface.Instance.DrawBarSmall(screenPosition + new Vector2(-35, -50), capacityPercent, Color.Blue);

            //cash
            string playerValueString = "$" + _playerUi[index].CarryingValue.ToString("N0");//ToString("#,##0")
            UserInterface.Instance.DrawString(position + new Vector2(50, -20), playerValueString);

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
        // todo: used? otherwise delete.. since we have old code in git
        public void DrawOldUI(int index) {
            
            int offset = UserInterface.Instance.GetOffset(index);

            string playerValueString = "carrying: " + _playerUi[index].CarryingValue;
            UserInterface.Instance.DrawString(new Vector2(10 + offset, 120), playerValueString);

            //health
            float healthPercent = _playerUi[index].Health / _playerUi[index].MaxHealth;
            UserInterface.Instance.DrawBarBig(new Vector2(10 + offset, 170), healthPercent, Color.Green);
            string healthString = _playerUi[index].Health + "/" + _playerUi[index].MaxHealth;
            UserInterface.Instance.DrawString(new Vector2(75 + offset, 160), healthString);

            //stamina
            float staminaPercent = _playerUi[index].Stamina / _playerUi[index].MaxStamina;
            UserInterface.Instance.DrawBarBig(new Vector2(10 + offset, 220), staminaPercent, Color.Red);
            string staminaString = Math.Round(_playerUi[index].Stamina * 100) + "/" + _playerUi[index].MaxStamina * 100; // TODO make stamina range 100
            UserInterface.Instance.DrawString(new Vector2(75 + offset, 210), staminaString);

            //capacity
            float capacityPercent = (float)_playerUi[index].CarryingWeight / _playerUi[index].MaxCapacity;
            UserInterface.Instance.DrawBarBig(new Vector2(10 + offset, 270), capacityPercent, Color.Blue);
            string capacityString = _playerUi[index].CarryingWeight + "/" + _playerUi[index].MaxCapacity;
            UserInterface.Instance.DrawString(new Vector2(100 + offset, 260), capacityString);
            
        }

        public string GetPlayerName(int index) {
            return _playerUi[index].Name;
        }


        // other

    }
}