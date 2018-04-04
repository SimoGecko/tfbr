// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class PlayerUI : Component {
        ////////// draws UI about player stats //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        PlayerUIStruct[] playerUI;
        Texture2D forkliftIcon;

        //reference
        public static PlayerUI instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            playerUI = new PlayerUIStruct[GameManager.numPlayers];

            forkliftIcon = File.Load<Texture2D>("Images/UI/forklift_icon");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(int index) {
            Vector2 position = new Vector2(130, 130);
            position += Vector2.UnitX * UserInterface.instance.GetOffset(index);

            UserInterface.instance.DrawPicture(position, forkliftIcon, forkliftIcon.Bounds.GetCenter(), .5f);

            //name
            string playerName = playerUI[index].name;
            UserInterface.instance.DrawString(position + new Vector2(-70, -110), playerName);

            //health
            float healthPercent = playerUI[index].health / playerUI[index].maxHealth;
            UserInterface.instance.DrawBar(position + new Vector2(-70, -70), healthPercent, Color.Green);

            //stamina
            float staminaPercent = playerUI[index].stamina / playerUI[index].maxStamina;
            UserInterface.instance.DrawBar(position + new Vector2(-70, 50), staminaPercent, Color.Red);

            //capacity
            float capacityPercent = (float)playerUI[index].carryingWeight / playerUI[index].maxCapacity;
            UserInterface.instance.DrawBarVertical(position + new Vector2(-95 , 60), capacityPercent, Color.Blue);

            //cash
            string playerValueString = "$" + playerUI[index].carryingValue.ToString("N0");//ToString("#,##0")
            UserInterface.instance.DrawString(position + new Vector2(50, -20), playerValueString);

        }

        public void UpdatePlayerUI(int index, float health, float maxHealth, float stamina, float maxStamina, int maxCapacity, int carryingValue, int carryingWeight, string name) {
            // current
            playerUI[index].carryingValue = carryingValue;

            playerUI[index].health = health;
            playerUI[index].stamina = stamina;
            playerUI[index].carryingWeight = carryingWeight;
            // max
            playerUI[index].maxHealth = maxHealth;
            playerUI[index].maxStamina = maxStamina;
            playerUI[index].maxCapacity = maxCapacity;

            playerUI[index].name = name;

        }



        // queries
        public void DrawOldUI(int index) {
            /*
            int offset = UserInterface.instance.GetOffset(index);

            string playerValueString = "carrying: " + playerUI[index].carryingValue;
            UserInterface.instance.DrawString(new Vector2(10 + offset, 120), playerValueString);

            //health
            float healthPercent = playerUI[index].health / playerUI[index].maxHealth;
            UserInterface.instance.DrawBarBig(new Vector2(10 + offset, 170), healthPercent, Color.Green);
            string healthString = playerUI[index].health + "/" + playerUI[index].maxHealth;
            UserInterface.instance.DrawString(new Vector2(75 + offset, 160), healthString);

            //stamina
            float staminaPercent = playerUI[index].stamina / playerUI[index].maxStamina;
            UserInterface.instance.DrawBarBig(new Vector2(10 + offset, 220), staminaPercent, Color.Red);
            string staminaString = Math.Round(playerUI[index].stamina * 100) + "/" + playerUI[index].maxStamina * 100; // TODO make stamina range 100
            UserInterface.instance.DrawString(new Vector2(75 + offset, 210), staminaString);

            //capacity
            float capacityPercent = (float)playerUI[index].carryingWeight / playerUI[index].maxCapacity;
            UserInterface.instance.DrawBarBig(new Vector2(10 + offset, 270), capacityPercent, Color.Blue);
            string capacityString = playerUI[index].carryingWeight + "/" + playerUI[index].maxCapacity;
            UserInterface.instance.DrawString(new Vector2(100 + offset, 260), capacityString);
            */
        }

        public string GetPlayerName(int index) {
            return playerUI[index].name;
        }


        // other

    }
    public struct PlayerUIStruct {
        // current
        public string name;
        public int carryingValue;

        public int carryingWeight;
        public float health;
        public float stamina;
        // max
        public float maxHealth;
        public float maxStamina;
        public int maxCapacity;
    }
}