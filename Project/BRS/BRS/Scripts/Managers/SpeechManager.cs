// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.Elements;
using System.Linq;
using System;

namespace BRS.Scripts {
    class SpeechManager : Component {
        ////////// provides strings to say in speech bubbles //////////
        //there must be one per player


        const int maxCharsPerLine = 11;
        const int maxLines = 4;

        public enum EventType {
            Begin, AlmostEnd, PoliceComing, Win, Lose, Random,
            CloseToVault, Powerup, Diamond, EnemyClose, Attack,
            OpenCrate, OpenVault, HitEnemy, Damage, Full,
            BringBase, Tutorial }
        float[] SpeechProbability = new float[] {
            .6f, .8f, .8f, 1f, 1f, .1f,
            1f, .2f, .9f, .3f, .1f,
            .2f, .9f, .6f, .5f, .6f,
            .6f, .2f };

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
            RandomEvents();
            RandomTutorial();
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
            //FillTestSpeech();
        }

        void FillTestSpeech() {
            for(int i=0; i<numEvents; i++) {
                for (int j = 0; j < 3; j++) AddString(i, "speech_" + j);
            }
        }

        void FillActualSpeech() {
            string[] allSpeechLines = System.IO.File.ReadAllLines("Load/robber_speech.txt");
            int eventIndex = -1;
            for(int i=0; i<allSpeechLines.Length; i++) {
                string line = allSpeechLines[i];
                if (line.Length < 1) continue; // empty line
                if (line[0] == '-') { eventIndex++; continue; } // line with event type
                string lineEscaped = EscapeLineAutomatically(11, line);
                //string lineEscaped = line.Replace('|', '\n'); // newline
                Debug.Assert(UserInterface.FontSupportsString(lineEscaped, Font.comic), "string not supported");

                AddString(eventIndex, lineEscaped);
            }
        }

        string EscapeLineAutomatically(int maxStringLength, string input) {
            string result = "";
            string[] splitstring = input.Split(' ');
            List<string> lines = new List<string>();
            lines.Add(splitstring[0]);
            for(int i=1; i<splitstring.Length; i++) {
                bool newWordFits = lines[lines.Count - 1].Length + splitstring[i].Length <= maxStringLength;
                if (newWordFits) {
                    lines[lines.Count - 1] += " " + splitstring[i];
                } else {
                    lines.Add(splitstring[i]);
                }
            }
            //center
            for(int i=0; i<lines.Count; i++) {
                lines[i] = SpacePad((maxStringLength - lines[i].Length) / 2) + lines[i];
                if (i < lines.Count - 1) lines[i] += '\n';
            }
            Debug.Assert(lines.Count <= maxLines, "text too long: " + input);
            //merge
            for(int i=0; i<lines.Count; i++) {
                result += lines[i];
            }
            return result;
        }

        string SpacePad(int num) {
            string result = "";
            for (int i = 0; i < num; i++) result += " ";
            return result;
        }

        public void AddString(int index, string speech) {
            speechString[index].Add(speech);
        }

        void PlugEvents() {
            RoundManager.Instance.OnRoundStartAction += (() => OnEvent(EventType.Begin));
            RoundManager.Instance.OnRoundAlmostEndAction += (() => OnEvent(EventType.AlmostEnd));
            RoundManager.Instance.OnPoliceComingAction += (() => OnEvent(EventType.PoliceComing));
            RoundManager.Instance.OnRoundEndAction += (() => OnRoundEnd());

            Player p = ElementManager.Instance.Player(index);
            p.gameObject.GetComponent<PlayerAttack>().OnAttackBegin += (() => OnEvent(EventType.Attack));
            p.gameObject.GetComponent<PlayerAttack>().OnEnemyHit += (() => OnEvent(EventType.HitEnemy));
            p.gameObject.GetComponent<PlayerPowerup>().OnPowerupPickup += (() => OnEvent(EventType.Powerup));
            p.gameObject.GetComponent<PlayerInventory>().OnInventoryFull += (() => OnEvent(EventType.Full));
            p.OnTakeDamage += (() => OnEvent(EventType.Damage));

            if (GameObject.NameExists("vault")) {
                GameObject.FindGameObjectWithName("vault").GetComponent<Vault>().OnVaultOpen += (() => OnEvent(EventType.OpenVault));
                GameObject.FindGameObjectWithName("vault").GetComponent<Vault>().OnClosedClose += (() => { if (Vault.OnClosedCloseIndex == index) OnEvent(EventType.CloseToVault); });
            }

            if (GameObject.NameExists("base_"+index%2)) {
                GameObject.FindGameObjectWithName("base_"+index%2).GetComponent<Base>().OnBringBase += (() => OnEvent(EventType.BringBase));
            }
            //still not plugged
            /*Diamond, EnemyClose, OpenCrate, CloseToVault }*/

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



        async void RandomEvents() {
            while (true) {
                if (GameManager.GameActive) {
                    OnEvent(EventType.Random);
                }
                await Time.WaitForSeconds(MyRandom.Value * 10);
            }
        }

        async void RandomTutorial() {
            while (true) {
                if (GameManager.GameActive) {
                    OnEvent(EventType.Tutorial);
                }
                await Time.WaitForSeconds(MyRandom.Value * 100);
            }
        }


        // other

    }
}
 