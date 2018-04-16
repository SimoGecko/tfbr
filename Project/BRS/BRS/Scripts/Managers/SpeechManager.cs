// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.Elements;
using System;

namespace BRS.Scripts {
    class SpeechManager : Component {
        ////////// provides strings to say in speech bubbles //////////
        //there must be one per player

        public enum EventType {
            Begin, AlmostEnd, Win, Lose, Random,
            Powerup, Diamond, EnemyClose, Attack,
            OpenCrate, OpenVault, HitEnemy, Damage, Full,
            BringBase, Tutorial }
        float[] SpeechProbability = new float[] {
            .6f, .6f, 1f, 1f, .5f,
            .2f, .9f, .3f, .1f,
            .2f, .8f, .6f, .5f, .6f,
            .6f, .3f };

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
        float turnOffBubbleTime;

        //reference


        // --------------------- BASE METHODS ------------------
        public SpeechManager(int _index) {
            index = _index;
        }

        public override void Start() {
            //nextTalkTime = Time.CurrentTime;
            FillSpeechStrings();
            PlugEvents();
        }

        public override void Update() {
            if (Time.CurrentTime > turnOffBubbleTime)
                TurnOffSpeech();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void FillSpeechStrings() {
            if (speechString != null) return; // already filled
            numEvents = Enum.GetNames(typeof(EventType)).Length;
            speechString = new List<string>[numEvents];
            for (int i = 0; i < numEvents; i++) speechString[i] = new List<string>();
            FillActualSpeech();
        }

        void FillActualSpeech() {
            string[] allSpeechLines = System.IO.File.ReadAllLines("Load/robber_speech.txt");
            int eventIndex = -1;
            for(int i=0; i<allSpeechLines.Length; i++) {
                string line = allSpeechLines[i];
                if (line.Length < 1) continue; // empty line
                if (line[0] == '-') { eventIndex++; continue; } // line with event type
                string lineEscaped = line.Replace('|', '\n'); // newline
                AddString(eventIndex, lineEscaped);
            }
        }

        public void AddString(int index, string speech) {
            speechString[index].Add(speech);
        }

        void PlugEvents() {
            RoundManager.Instance.OnRoundStartAction += (() => OnEvent(EventType.Begin));
            RoundManager.Instance.OnRoundAlmostEndAction += (() => OnEvent(EventType.AlmostEnd));
            RoundManager.Instance.OnRoundEndAction += (() => OnRoundEnd());

            Player p = ElementManager.Instance.Player(index);
            p.gameObject.GetComponent<PlayerAttack>().OnAttackBegin += (() => OnEvent(EventType.Attack));
            p.gameObject.GetComponent<PlayerAttack>().OnEnemyHit += (() => OnEvent(EventType.HitEnemy));
            p.gameObject.GetComponent<PlayerPowerup>().OnPowerupPickup += (() => OnEvent(EventType.Powerup));
            p.gameObject.GetComponent<PlayerInventory>().OnInventoryFull += (() => OnEvent(EventType.Full));
            p.OnTakeDamage += (() => OnEvent(EventType.Damage));

            if (GameObject.NameExists("vault")) {
                GameObject.FindGameObjectWithName("vault").GetComponent<Vault>().OnVaultOpen += (() => OnEvent(EventType.OpenVault));
            }

            if (GameObject.NameExists("base_"+index%2)) {
                GameObject.FindGameObjectWithName("base_"+index%2).GetComponent<Base>().OnBringBase += (() => OnEvent(EventType.BringBase));
            }
            //still not plugged
            /* Random, Diamond, EnemyClose, OpenCrate, Tutorial }*/

    }

        void OnRoundEnd() {
            if (RoundManager.Instance.Winner == index) OnEvent(EventType.Win);
            else OnEvent(EventType.Lose);
        }

        void OnEvent(EventType st) {
            int sti = (int)st;
            if(MyRandom.Value<SpeechProbability[sti])
                TrySpeech(speechString[sti][MyRandom.Range(0, speechString[sti].Count)], false);
        }

        void TrySpeech(string speech, bool interrupt) {
            if (ReadyToTalk() || interrupt) {
                if (!lastNThoughts.Contains(speech)) {
                    lastNThoughts.Enqueue(speech);
                    if (lastNThoughts.Count > numThoughtsBeforeRepeating) {
                        lastNThoughts.Dequeue();
                    }
                    //turn on
                    ShowSpeech(speech);
                    //call to turn off
                    float duration = speech.Length / readingSpeed + readingDefaultTime;
                    turnOffBubbleTime = Time.CurrentTime + duration;
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
 