// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using System;

namespace BRS.Scripts {
    class SpeechManager : Component {
        ////////// provides strings to say in speech bubbles //////////
        //there must be one per player

        public enum EventType {
            Begin, AlmostEnd, Win, Lose, Random,
            Powerup, Valuable, Diamond, EnemyClose, Attack,
            OpenCrate, OpenVault, HitEnemy, Damage, Full,
            BringBase, Tutorial }
        float[] SpeechProbability = new float[] {
            .5f, .5f, 1f, 1f, .5f,
            .3f, .1f, 1f, .3f, .5f,
            .2f, .8f, .5f, .5f, .8f,
            .8f, .3f };

        // --------------------- VARIABLES ---------------------

        //public
        const int numThoughtsBeforeRepeating = 20;
        static Vector2 minmaxTimeBetweenSpeech = new Vector2(1, 2);
        const float readingSpeed = 16f;//characters/second
        const float readingDefaultTime = 3f;

        static Queue<string> lastNThoughts = new Queue<string>(); // shared to avoid same speech
        static List<string>[] speechString;
        static int numEvents;
        //private
        float nextTalkTime;
        bool isTalking;
        int index;

        //reference


        // --------------------- BASE METHODS ------------------
        public SpeechManager(int _index) {
            index = _index;
        }

        public override void Start() {
            nextTalkTime = Time.CurrentTime;
            FillSpeechStrings();
            PlugEvents();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void FillSpeechStrings() {
            if (speechString != null) return; // already filled
            numEvents = Enum.GetNames(typeof(EventType)).Length;
            speechString = new List<string>[numEvents];
            for (int i = 0; i < numEvents; i++) speechString[i] = new List<string>();
            //TODO fill speech strings
            FillActualSpeech();
        }

        void FillActualSpeech() {
            for(int i=0; i<numEvents; i++) {
                for(int j=0; j<10; j++) {
                    AddString(i, ((EventType)i).ToString() + " " + (j+1));
                }
            }
        }

        public void AddString(int index, string speech) {
            speechString[index].Add(speech);
        }

        void PlugEvents() {
            RoundManager.Instance.OnRoundStartAction += (() => OnEvent(EventType.Begin));
            RoundManager.Instance.OnRoundAlmostEndAction += (() => OnEvent(EventType.AlmostEnd));
            RoundManager.Instance.OnRoundEndAction += (() => OnEvent(EventType.Win));

            Player p = ElementManager.Instance.Player(index);
            p.gameObject.GetComponent<PlayerAttack>().OnAttackBegin += (() => OnEvent(EventType.Attack));


        }

        void OnEvent(EventType st) {
            int sti = (int)st;
            //if(MyRandom.Value<SpeechProbability[sti])
                TrySpeech(speechString[sti][MyRandom.Range(0, speechString[sti].Count)], true);
        }

        void TrySpeech(string speech, bool interrupt) {
            if (ReadyToTalk() || interrupt) {
                if (!lastNThoughts.Contains(speech)) {
                    lastNThoughts.Enqueue(speech);
                    if (lastNThoughts.Count > numThoughtsBeforeRepeating) {
                        lastNThoughts.Dequeue();
                    }

                    ShowSpeech(speech);

                    //call to turn off
                    float duration = speech.Length / readingSpeed + readingDefaultTime;
                    new Timer(duration, () => TurnOffSpeech());
                }
            }
        }

        void ShowSpeech(string speech) {
            //CALL TO BUBBLE DISPLAY
            isTalking = true;
            nextTalkTime = Time.CurrentTime + MyRandom.Range(minmaxTimeBetweenSpeech.X, minmaxTimeBetweenSpeech.Y);
            SpeechUI.Instance.StartShowBubble(speech, index);
        }

        void TurnOffSpeech() {
            isTalking = false;
            SpeechUI.Instance.EndShowBubble(index);

        }




        // queries
        bool ReadyToTalk() {
            return !isTalking && (Time.CurrentTime > nextTalkTime);
        }


        // other

    }
}