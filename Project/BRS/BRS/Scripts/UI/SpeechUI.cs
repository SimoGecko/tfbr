// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts {
    class SpeechUI : Component {
        ////////// shows speech bubbles //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float animationDuration = .2f;


        //private
        private Texture2D _comicBubble;
        private Texture2D[] _comicCharacter;

        bool[] showBubble;
        string[] bubbleString;
        float[] showCharacterPercent;

        //reference
        public static SpeechUI Instance;
        bool[] inTransition;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            Instance = this;
        }

        public override void Start() {
            showBubble = new bool[GameManager.NumPlayers];
            inTransition = new bool[GameManager.NumPlayers];
            bubbleString = new string[GameManager.NumPlayers];
            showCharacterPercent = new float[GameManager.NumPlayers];
            _comicCharacter = new Texture2D[2];

            _comicBubble = File.Load<Texture2D>("Images/comic/bubble");
            _comicCharacter[0] = File.Load<Texture2D>("Images/comic/louis");
            _comicCharacter[1] = File.Load<Texture2D>("Images/comic/ted");

        }

        public override void Update() {
            if (Input.GetKeyDown(Keys.T)) StartShowBubble("Let's get\nthis robbery\nstarted!", 0);
            if (Input.GetKeyDown(Keys.Z)) EndShowBubble(0);
            if (Input.GetKeyDown(Keys.U)) StartShowBubble("Let's go\npick up cash!", 1);
            if (Input.GetKeyDown(Keys.I)) EndShowBubble(1);
        }

        public override void Draw(int index) {
            if (index == 0) return;
            index--;

            //int posX = (int)System.Math.Round(MathHelper.LerpPrecise(-256, 0, showCharacterPercent[index]));
            int posY = (int)System.Math.Round(MathHelper.LerpPrecise(256, 0, showCharacterPercent[index]));
            UserInterface.DrawPicture(_comicCharacter[index%2], new Rectangle(0, posY, 256, 256), null, Align.BotLeft);
            if (showBubble[index]) {
                UserInterface.DrawPicture(_comicBubble, new Rectangle(125, -195, 190, 170), null, Align.BotLeft);
                UserInterface.DrawString(bubbleString[index], new Rectangle(140, -245, 155, 100), Align.BotLeft, paragraph: Align.Center, col: Color.Black, scale:.85f);
            }
            

        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public async void StartShowBubble(string _bubbleString, int index) {
            bubbleString[index] = _bubbleString;
            //showBubble[index] = false;
            //showCharacterPercent[index] = 0;
            if (inTransition[index]) return;
            inTransition[index] = true;
            while (showCharacterPercent[index] < 1) {
                showCharacterPercent[index] += Time.DeltaTime / animationDuration;
                await Time.WaitForFrame();
            }
            showCharacterPercent[index] = 1f;
            showBubble[index] = true;
            inTransition[index] = false;

            //TODO play sound
        }

        public async void EndShowBubble(int index) {
            if (inTransition[index]) return;
            inTransition[index] = true;
            showBubble[index] = false;
            //showCharacterPercent[index] = 1;
            while (showCharacterPercent[index] > 0) {
                showCharacterPercent[index] -= Time.DeltaTime / animationDuration;
                await Time.WaitForFrame();
            }
            showCharacterPercent[index] = 0f;
            inTransition[index] = false;
        }



        // queries



        // other

    }
}