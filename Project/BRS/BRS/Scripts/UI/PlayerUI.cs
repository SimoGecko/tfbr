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

        //reference
        public static PlayerUI instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            playerUI = new PlayerUIStruct[GameManager.numPlayers];
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(int index) {
            int offset = UserInterface.instance.GetOffset(index);

            string playerValueString = "carrying: " + playerUI[index].carryingValue;
            UserInterface.instance.DrawString(new Vector2(10 + offset, 120), playerValueString);

            //health
            float healthPercent = playerUI[index].health / playerUI[index].maxHealth;
            UserInterface.instance.DrawBar(new Vector2(10 + offset, 170), healthPercent, Color.Green);
            string healthString = playerUI[index].health + "/" + playerUI[index].maxHealth;
            UserInterface.instance.DrawString(new Vector2(75 + offset, 160), healthString);

            //stamina
            float staminaPercent = playerUI[index].stamina / playerUI[index].maxStamina;
            UserInterface.instance.DrawBar(new Vector2(10 + offset, 220), staminaPercent, Color.Red);
            string staminaString = Math.Round(playerUI[index].stamina * 100) + "/" + playerUI[index].maxStamina * 100; // TODO make stamina range 100
            UserInterface.instance.DrawString(new Vector2(75 + offset, 210), staminaString);

            //capacity
            float capacityPercent = (float)playerUI[index].carryingWeight / playerUI[index].maxCapacity;
            UserInterface.instance.DrawBar(new Vector2(10 + offset, 270), capacityPercent, Color.Blue);
            string capacityString = playerUI[index].carryingWeight + "/" + playerUI[index].maxCapacity;
            UserInterface.instance.DrawString(new Vector2(100 + offset, 260), capacityString);

        }

        public void UpdatePlayerUI(int index, float health, float maxHealth, float stamina, float maxStamina, int maxCapacity, int carryingValue, int carryingWeight) {
            // current
            playerUI[index].carryingValue = carryingValue;

            playerUI[index].health = health;
            playerUI[index].stamina = stamina;
            playerUI[index].carryingWeight = carryingWeight;
            // max
            playerUI[index].maxHealth = maxHealth;
            playerUI[index].maxStamina = maxStamina;
            playerUI[index].maxCapacity = maxCapacity;
        }



        // queries



        // other

    }
    public struct PlayerUIStruct {
        // current
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