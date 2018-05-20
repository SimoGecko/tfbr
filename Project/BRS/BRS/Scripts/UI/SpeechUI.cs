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

        SpeechUIStruct[] _speechUI;

        //reference
        public static SpeechUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            Instance = this;
        }

        public override void Start() {
            _speechUI = new SpeechUIStruct[GameManager.NumPlayers];

            _comicCharacter = new Texture2D[2];
            _comicBubble       = File.Load<Texture2D>("Images/comic/bubble");
            _comicCharacter[0] = File.Load<Texture2D>("Images/comic/louis");
            _comicCharacter[1] = File.Load<Texture2D>("Images/comic/ted");
        }

        public override void Update() {
            
        }

        public override void Draw2D(int index) {
            if (index == -1) return;

            //int posX = (int)System.Math.Round(MathHelper.LerpPrecise(-256, 0, showCharacterPercent[index])); // comes from the left
            int posY = (int)System.Math.Round(MathHelper.LerpPrecise(256, 0, _speechUI[index].showCharacterPercent)); // comes from the bottom
            UserInterface.DrawPicture(_comicCharacter[index%2], new Rectangle(0, posY, 256, 256), null, Align.BotLeft);
            if (_speechUI[index].showBubble) {
                UserInterface.DrawPicture(_comicBubble, new Rectangle(125, -195, 190, 170), null, Align.BotLeft);
                UserInterface.DrawString(_speechUI[index].bubbleString, new Rectangle(140, -245, 155, 100), Align.BotLeft, paragraph: Align.Center, col: Color.Black, scale:.85f);
            }
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void StartShowBubble(string _bubbleString, int index) {
            _speechUI[index].bubbleString = _bubbleString;
            //showBubble[index] = false;
            //showCharacterPercent[index] = 0;
            if (_speechUI[index].inTransition) return;
            AnimationUp(index);
            Audio.Play("characters_popup", ElementManager.Instance.Player(index).transform.position);
        }

        public void EndShowBubble(int index) {
            if (_speechUI[index].inTransition) return;
            _speechUI[index].showBubble = false;
            AnimationDown(index);
        }

        public async void AnimationUp(int index) {
            _speechUI[index].inTransition = true;
            while (_speechUI[index].showCharacterPercent < 1) {
                _speechUI[index].showCharacterPercent += Time.DeltaTime / animationDuration;
                await Time.WaitForFrame();
            }
            _speechUI[index].showCharacterPercent = 1f;
            _speechUI[index].showBubble = true;
            _speechUI[index].inTransition = false;
        }

        public async void AnimationDown(int index) {
            _speechUI[index].inTransition = true;
            //showCharacterPercent[index] = 1;
            while (_speechUI[index].showCharacterPercent > 0) {
                _speechUI[index].showCharacterPercent -= Time.DeltaTime / animationDuration;
                await Time.WaitForFrame();
            }
            _speechUI[index].showCharacterPercent = 0f;
            _speechUI[index].inTransition = false;
        }



        // queries



        // other
    }

    public struct SpeechUIStruct {
        public bool showBubble;
        public string bubbleString;
        public bool inTransition;
        public float showCharacterPercent;
    }
}