// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class RoundManager : Component {
        ////////// deals with round stuff, ie time, current winner of rounds and score, ... (just for one round) //////////
        ////////// also stores the mode of the round, ie BOMBER / SPEEDER / SUPERCASH / POWERUP RUSH / ... //////////

        // --------------------- VARIABLES ---------------------

        //public
        public static int roundTime = 120;
        public const int timeBeforePolice = 5;

        //private
        Timer rt;

        //reference
        Base[] bases;
        public static RoundManager instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            rt = new Timer(0, roundTime, OnRoundEnd);
            GameUI.instance.StartMatch(rt);
            //FindBases(); // data race
        }

        public override void Update() {
            if (rt.span.TotalSeconds < timeBeforePolice) {
                GameUI.instance.UpdatePoliceComing();
            }
        }



        // --------------------- CUSTOM METHODS ---------b -------


        // commands
        void OnRoundEnd() {
            FindBases();
            NotifyBases();
            Debug.Log("notified bases");
            Tuple<int, int> winner = FindWinner();
            GameUI.instance.UpdateGameWinnerUI(winner.Item1);
            UpdateRanking();
            GameManager.instance.OnRoundEnd(winner.Item1);
        }

        void UpdateRanking() {
            List<Tuple<string, string>> rankinglist;
            rankinglist = File.ReadRanking("Load/Rankings/ranking" + (roundTime / 60).ToString() + "min.txt");

            for (int i = 0; i < GameManager.numPlayers; ++i) {
                rankinglist.Add(new Tuple<string, string>(PlayerUI.instance.GetPlayerName(i), bases[i%2].TotalMoney.ToString()));
            }

            rankinglist.Sort((x,y) => -1* Int32.Parse(x.Item2).CompareTo(Int32.Parse(y.Item2)));

            File.WriteRanking("Load/Rankings/ranking" + (roundTime / 60).ToString() + "min.txt", rankinglist, 5);
        }

        void FindBases() {
            //find bases
            GameObject[] basesObject = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            if (basesObject.Length < 1) {
                Debug.LogError("could not find the bases"); // avoids tag messup
            } else {
                bases = new Base[basesObject.Length];
                for (int i = 0; i < bases.Length; i++)
                    bases[i] = basesObject[i].GetComponent<Base>();
            }
        }

        void NotifyBases() {
            for (int i = 0; i < bases.Length; i++)
                bases[i].NotifyRoundEnd();
        }

        // queries
        Tuple<int, int> FindWinner() {
            int winner = 0;
            int maxCash = bases[0].TotalMoney;
            for (int i = 1; i < bases.Length; i++) {
                int totmoney = bases[i].TotalMoney;
                if (totmoney > maxCash) { // TODO deal with tie
                    winner = i;
                    maxCash = totmoney;
                }
            }
            return new Tuple<int, int>(winner, maxCash);
        }


        // other

    }
}