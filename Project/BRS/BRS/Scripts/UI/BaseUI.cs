// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    class BaseUI : Component {
        ////////// draws UI for the base stats //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private BaseUIStruct[] _baseUi;
        private readonly int[] _baseUIwins = new int[2]; // persists between plays

        private Texture2D _baseIcon;
        private Texture2D _ribbon;
        private Texture2D _barIcons; // 256x256


        //reference
        public static BaseUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            Instance = this;
            _baseUi = new BaseUIStruct[2];
        }

        public override void Start() {
            _baseIcon = File.Load<Texture2D>("Images/UI/base_icon");
            _ribbon = File.Load<Texture2D>("Images/UI/ribbon");
            _barIcons = File.Load<Texture2D>("Images/UI/bar_icons");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------

        public override void Draw2D(int index) {
            if (index == 0) return;
            index--;
            bool flip = false;// index % 2 != 0;

            UserInterface.DrawString("Team " + (index+1), new Rectangle(-20, 10, 300, 40), Align.TopRight, scale: .5f, bold: true, flip: flip);
            UserInterface.DrawPicture(_baseIcon, new Rectangle(-20, 40, 100, 100), null, Align.TopRight, flip: flip);

            int rank = RoundManager.GetRank(index%2);
            if(rank==1)
                UserInterface.DrawPicture(_ribbon, new Rectangle(-120, 18, 60, 60), null, Align.TopRight, flip: flip);
            string rankString = RoundManager.RankToString(rank);
            UserInterface.DrawString(rankString, new Rectangle(-23, 45, 40, 40), Align.TopRight, Align.TopRight, Align.Center, scale: .7f, bold: true, flip: flip);

            UserInterface.DrawString("base", new Rectangle(-145, 32, 175, 25), Align.TopRight, Align.TopRight, Align.Bottom, scale: .7f, flip: flip);
            float capacityPercent = (float)_baseUi[index % 2].TotalMoneyInBase / RoundManager.MoneyToWinRound;
            UserInterface.DrawBarStriped(capacityPercent, new Rectangle(-320, 57, 175, 25), Graphics.Yellow, Align.TopRight, flip: flip);
            string baseValueString = Utility.IntToMoneyString(_baseUi[index % 2].TotalMoneyInBase);
            UserInterface.DrawString(baseValueString, new Rectangle(-145, 82, 175, 25), Align.TopRight, Align.TopRight, Align.Top, flip: flip);
            UserInterface.DrawPicture(_barIcons, new Rectangle(-120, 57, 25, 25), new Rectangle(200, 0, 200, 200), Align.TopRight, flip: flip);

            //wins
            string winsString = "wins: " + _baseUIwins[index % 2];
            UserInterface.DrawString(winsString, new Rectangle(-20, 165, 100, 25), Align.TopRight, Align.TopRight, Align.Center, scale: .7f, flip: flip);
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

        // queries



        // other

    }

    public struct BaseUIStruct {
        public int TotalMoneyInBase;
        public float BaseHealth;
        public float BaseMaxHealth;
    }
}