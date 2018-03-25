// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class BaseUI : Component {
        ////////// draws UI for the base stats //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        BaseUIStruct[] baseUI;

        //reference
        public static BaseUI instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            baseUI = new BaseUIStruct[2];
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------

        public void Draw(int index) {
            int offset = UserInterface.instance.GetOffset(index);

            string baseValueString = "cash: " + baseUI[index].totalMoneyInBase;
            UserInterface.instance.DrawString(new Vector2(10 + offset, 80), baseValueString);

            float baseHealthPercent = baseUI[index].baseHealth / baseUI[index].baseMaxHealth;
            UserInterface.instance.DrawBar(new Vector2(10 + offset, 320), baseHealthPercent, Color.Orange);

            string baseHealthstring = baseUI[index].baseHealth + "/" + baseUI[index].baseMaxHealth;
            UserInterface.instance.DrawString(new Vector2(75 + offset, 310), baseHealthstring);
        }


        // commands
        public void UpdateBaseUI(int index, float baseHealth, float baseMaxHealth, int value) {
            if (index < baseUI.Length) {
                baseUI[index].baseHealth = baseHealth;
                baseUI[index].baseMaxHealth = baseMaxHealth;
                baseUI[index].totalMoneyInBase = value;
            }
        }


        // queries



        // other

    }

    public struct BaseUIStruct {
        public int totalMoneyInBase;
        public float baseHealth;
        public float baseMaxHealth;
    }
}