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
        private readonly int[] _baseUIwins = new int[2]; // persists between rounds

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
            _barIcons = File.Load<Texture2D>("Images/UI/bar_icons");
            _ribbon   = File.Load<Texture2D>("Images/UI/ribbon");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------

        public override void Draw2D(int index) {
            if (index == -1) return;
            bool flip = false;// index % 2 != 0;
            int baseIndex = GameManager.TeamIndex(index);

            UserInterface.DrawString("Team " + (index+1), new Rectangle(-20, 10, 300, 40), Align.TopRight, Align.TopRight, Align.Right, scale: .5f, bold: true, flip: flip);
            UserInterface.DrawPicture(_baseIcon, new Rectangle(-20, 40, 100, 100), null, Align.TopRight, flip: flip);

            int rank = RoundManager.GetRank(baseIndex);
            //if(rank==1)
                UserInterface.DrawPicture(_ribbon, new Rectangle(-138, 15, 80, 80), null, Align.TopRight, flip: flip);
            string rankString = RoundManager.RankToString(rank);
            UserInterface.DrawString(rankString, new Rectangle(-157, 33, 40, 40), Align.TopRight, Align.TopRight, Align.Center, scale: .7f, bold: true, flip: flip);

            UserInterface.DrawString("base", new Rectangle(-145, 82, 175, 25), Align.TopRight, Align.TopRight, Align.Bottom, scale: .7f, flip: flip);
            float capacityPercent = (float)_baseUi[baseIndex].TotalMoneyInBase / RoundManager.MoneyToWinRound;
            UserInterface.DrawBarStriped(capacityPercent, new Rectangle(-320, 107, 175, 25), Graphics.Yellow, Align.TopRight, flip: flip);
            string baseValueString = Utility.IntToMoneyString(_baseUi[baseIndex].TotalMoneyInBase);
            UserInterface.DrawString(baseValueString, new Rectangle(-145, 132, 175, 25), Align.TopRight, Align.TopRight, Align.Top, flip: flip);
            UserInterface.DrawPicture(_barIcons, new Rectangle(-120, 107, 25, 25), new Rectangle(200, 0, 200, 200), Align.TopRight, flip: flip);

            //wins
            string winsString = "wins: " + _baseUIwins[baseIndex];
            UserInterface.DrawString(winsString, new Rectangle(-20, 165, 100, 25), Align.TopRight, Align.TopRight, Align.Center, scale: .7f, flip: flip);
        }


        // commands
        public void UpdateBaseUI(int index, int value) {
            if (0<=index && index < _baseUi.Length) {
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
    }
}