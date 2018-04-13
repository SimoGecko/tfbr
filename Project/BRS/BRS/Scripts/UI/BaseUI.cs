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
        private Texture2D _baseIcon;
        private Texture2D _ribbon;
        private readonly int[] _baseUIwins = new int[2];
        private Texture2D _barIcons; // 256x256


        //reference
        public static BaseUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _baseUi = new BaseUIStruct[2];
            _baseIcon = File.Load<Texture2D>("Images/UI/base_icon");
            _ribbon = File.Load<Texture2D>("Images/UI/ribbon");
            _barIcons = File.Load<Texture2D>("Images/UI/bar_icons");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------

        public void Draw(int index) {
            bool flip = index % 2 != 0;

            UserInterface.Instance.DrawString("Team " + (index+1), new Rectangle(-20, 10, 200, 30), Align.TopRight, scale: .5f, bold: true, flip: flip);
            UserInterface.Instance.DrawPicture(_baseIcon, new Rectangle(-20, 40, 80, 80), null, Align.TopRight, flip: flip);

            int rank = RoundManager.GetRank(index);
            if(rank==1)
                UserInterface.Instance.DrawPicture(_ribbon, new Rectangle(-120, 18, 60, 60), null, Align.TopRight, flip: flip);
            string rankString = RoundManager.RankToString(rank);
            UserInterface.Instance.DrawString(rankString, new Rectangle(-135, 28, 30, 30), Align.TopRight, Align.TopRight, Align.Center, scale: .5f, bold: true, flip: flip);

            UserInterface.Instance.DrawString("base", new Rectangle(-120, 75, 150, 20), Align.TopRight, Align.TopRight, Align.Bottom, scale: .7f, flip: flip);
            float capacityPercent = (float)_baseUi[index].TotalMoneyInBase / RoundManager.MoneyToWinRound;
            Color yellowColor = new Color(255, 198, 13);
            UserInterface.Instance.DrawBarStriped(capacityPercent, new Rectangle(-270, 95, 150, 20), yellowColor, Align.TopRight, flip: flip);
            string baseValueString = Utility.IntToMoneyString(_baseUi[index].TotalMoneyInBase);
            UserInterface.Instance.DrawString(baseValueString, new Rectangle(-120, 115, 150, 20), Align.TopRight, Align.TopRight, Align.Top, flip: flip);
            UserInterface.Instance.DrawPicture(_barIcons, new Rectangle(-100, 95, 20, 20), new Rectangle(200, 0, 200, 200), Align.TopRight, flip: flip);

            //wins
            string winsString = "wins: " + _baseUIwins[index];
            UserInterface.Instance.DrawString(winsString, new Rectangle(-20, 120, 80, 20), Align.TopRight, Align.TopRight, Align.Center, scale: .7f, flip: flip);
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
        void DrawOld2(int index) {
            /*
            Vector2 position = new Vector2(100, 970);
            //position += Vector2.UnitX* UserInterface.Instance.GetOffset(index);

            //UserInterface.Instance.DrawPicture(position, _baseIcon, _baseIcon.Bounds.GetCenter(), .4f);

            //name
            string baseName = "Team " + (index + 1).ToString();
            UserInterface.Instance.DrawStringOLD(position + new Vector2(-70, -110), baseName);

            //health
            float healthPercent = _baseUi[index].BaseHealth / _baseUi[index].BaseMaxHealth;
            UserInterface.Instance.DrawBar(position + new Vector2(-70, -70), healthPercent, Color.Orange);

            //cash
            string baseValueString = "$" + _baseUi[index].TotalMoneyInBase.ToString("N0");//ToString("#,##0")
            UserInterface.Instance.DrawStringOLD(position + new Vector2(50, -20), baseValueString);
            //wins
            string winsString = "wins: " + _baseUIwins[index];
            UserInterface.Instance.DrawStringOLD(position + new Vector2(-70, 50), winsString);
            */
        }

        // queries



        // other

        }
    }