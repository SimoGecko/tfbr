// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class BaseUI : Component {
        ////////// draws UI for the base stats //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        BaseUIStruct[] baseUI;
        Texture2D baseIcon;
        int[] baseUIwins = new int[2];


        //reference
        public static BaseUI instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            baseUI = new BaseUIStruct[2];
            baseIcon = File.Load<Texture2D>("Images/UI/base_icon");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------

        public void Draw(int index) {
            Vector2 position = new Vector2(100, 970);
            position += Vector2.UnitX* UserInterface.instance.GetOffset(index);

            UserInterface.instance.DrawPicture(position, baseIcon, baseIcon.Bounds.GetCenter(), .4f);

            //name
            string baseName = "Team " + (index + 1).ToString();
            UserInterface.instance.DrawString(position + new Vector2(-70, -110), baseName);

            //health
            float healthPercent = baseUI[index].baseHealth / baseUI[index].baseMaxHealth;
            UserInterface.instance.DrawBar(position + new Vector2(-70, -70), healthPercent, Color.Orange);

            //cash
            string baseValueString = "$" + baseUI[index].totalMoneyInBase.ToString("N0");//ToString("#,##0")
            UserInterface.instance.DrawString(position + new Vector2(50, -20), baseValueString);
            //wins
            string winsString = "wins: " + baseUIwins[index];
            UserInterface.instance.DrawString(position + new Vector2(-70, 50), winsString);
        }


        // commands
        public void UpdateBaseUI(int index, float baseHealth, float baseMaxHealth, int value) {
            if (index < baseUI.Length) {
                baseUI[index].baseHealth = baseHealth;
                baseUI[index].baseMaxHealth = baseMaxHealth;
                baseUI[index].totalMoneyInBase = value;
            }
        }

        public void UpdateBaseUIWins(int index) {
            baseUIwins[index]++;
        }

        void DrawOld(int index) {
            /*
            int offset = UserInterface.instance.GetOffset(index);

            string baseValueString = "cash: " + baseUI[index].totalMoneyInBase;
            UserInterface.instance.DrawString(new Vector2(10 + offset, 80), baseValueString);

            float baseHealthPercent = baseUI[index].baseHealth / baseUI[index].baseMaxHealth;
            UserInterface.instance.DrawBar(new Vector2(10 + offset, 320), baseHealthPercent, Color.Orange);

            string baseHealthstring = baseUI[index].baseHealth + "/" + baseUI[index].baseMaxHealth;
            UserInterface.instance.DrawString(new Vector2(75 + offset, 310), baseHealthstring);
            */
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