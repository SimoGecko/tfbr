// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Engine;
using BRS.Scripts.Managers;

namespace BRS.Scripts {
    class RoundUI : Component {
        ////////// draws the UI for the round //////////
        public enum EndRoundCondition { Timesup, Busted, Success, Youwon};

        // --------------------- VARIABLES ---------------------

        //public
        const int countdownSize = 256;
        const int roundEndWidth = 600;
        const int roundEndHeight = 256;

        //private
        Texture2D countdownTex, endroundTex;
        bool showCountdown = false;
        int countdownNumber = 0;
        int[] endRoundPlayerText;

        bool showEndRound;

        //reference
        public static RoundUI instance;

        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            instance = this;
        }

        public override void Start() {
            endRoundPlayerText = new int[GameManager.NumPlayers];
            countdownTex = File.Load<Texture2D>("Images/UI/countdown");
            endroundTex = File.Load<Texture2D>("Images/UI/end_round_text");
        }

        public override void Update() {

        }

        public override void Draw2D(int i) {
            if (i == 0) return;
            i--;
            if (showCountdown) {
                Rectangle source = SpriteFromNumber(countdownNumber);
                UserInterface.DrawPicture(countdownTex, Vector2.Zero, source, Align.Center);
            }
            if (showEndRound) {
                Rectangle source = TextFromNumber(endRoundPlayerText[i]);
                UserInterface.DrawPicture(endroundTex, Vector2.Zero, source, Align.Center);
                string finalCash = Utility.IntToMoneyString( ElementManager.Instance.Base(i%2).TotalMoney);
                UserInterface.DrawString("you collected " + finalCash, new Vector2(0, 151), Align.Center, bold:true, scale:.6f, col:Color.Black);
                UserInterface.DrawString("you collected " + finalCash, new Vector2(0, 149), Align.Center, bold:true, scale:.6f, col: Color.Black);
                UserInterface.DrawString("you collected " + finalCash, new Vector2(-1, 150), Align.Center, bold:true, scale:.6f, col: Color.Black);
                UserInterface.DrawString("you collected " + finalCash, new Vector2(1, 150), Align.Center, bold:true, scale:.6f, col: Color.Black);
                UserInterface.DrawString("you collected " + finalCash, new Vector2(0, 150), Align.Center, bold:true, scale:.6f);
            }
        }

        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void ShowCountDown(int i) {
            if (i >= 0 && i <= 3) {
                countdownNumber = i;
                showCountdown = true;
            } else {
                showCountdown = false;
            }
        }

        public void ShowEndRound(int player, EndRoundCondition text) {
            endRoundPlayerText[player] = (int)text;
            showEndRound = true;
        }

        public void ShowEndRound(bool b) {
            showEndRound = b;
        }


        // queries
        Rectangle SpriteFromNumber(int num) {
            num = 3 - num;
            int col = num % 2;
            int row = num / 2;
            return new Rectangle(col * countdownSize, row * countdownSize, countdownSize, countdownSize);
        }

        Rectangle TextFromNumber(int num) {
            return new Rectangle(0, num * roundEndHeight, roundEndWidth, roundEndHeight);
        }


        // other

    }
}