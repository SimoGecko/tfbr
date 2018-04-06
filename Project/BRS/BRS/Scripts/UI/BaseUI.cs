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
        private BaseUIStruct[] _baseUi;
        private Texture2D _baseIcon;
        private readonly int[] _baseUIwins = new int[2];


        //reference
        public static BaseUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _baseUi = new BaseUIStruct[2];
            _baseIcon = File.Load<Texture2D>("Images/UI/base_icon");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------

        public void Draw(int index) {
            Vector2 position = new Vector2(100, 970);
            position += Vector2.UnitX* UserInterface.Instance.GetOffset(index);

            UserInterface.Instance.DrawPicture(position, _baseIcon, _baseIcon.Bounds.GetCenter(), .4f);

            //name
            string baseName = "Team " + (index + 1).ToString();
            UserInterface.Instance.DrawString(position + new Vector2(-70, -110), baseName);

            //health
            float healthPercent = _baseUi[index].BaseHealth / _baseUi[index].BaseMaxHealth;
            UserInterface.Instance.DrawBar(position + new Vector2(-70, -70), healthPercent, Color.Orange);

            //cash
            string baseValueString = "$" + _baseUi[index].TotalMoneyInBase.ToString("N0");//ToString("#,##0")
            UserInterface.Instance.DrawString(position + new Vector2(50, -20), baseValueString);
            //wins
            string winsString = "wins: " + _baseUIwins[index];
            UserInterface.Instance.DrawString(position + new Vector2(-70, 50), winsString);
        }


        // commands
        public void UpdateBaseUI(int index, float baseHealth, float baseMaxHealth, int value) {
            if (index < _baseUi.Length) {
                _baseUi[index].BaseHealth = baseHealth;
                _baseUi[index].BaseMaxHealth = baseMaxHealth;
                _baseUi[index].TotalMoneyInBase = value;
            }
        }

        public void UpdateBaseUIWins(int index) {
            _baseUIwins[index]++;
        }

        // Todo: Used?
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
}