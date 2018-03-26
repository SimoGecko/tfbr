// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class RoundManager : Component {
        ////////// deals with round stuff, ie time, current winner of rounds and score, ... (just for one round) //////////
        ////////// also stores the mode of the round, ie BOMBER / SPEEDER / SUPERCASH / POWERUP RUSH / ... //////////

        // --------------------- VARIABLES ---------------------

        //public
        public const int roundTime = 10;
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
            int winner = FindWinner();
            GameUI.instance.UpdateGameWinnerUI(winner);
            GameManager.instance.OnRoundEnd(winner);
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
        int FindWinner() {
            int winner = 0;
            int maxCash = bases[0].TotalMoney;
            for (int i = 1; i < bases.Length; i++) {
                int totmoney = bases[i].TotalMoney;
                if (totmoney > maxCash) { // TODO deal with tie
                    winner = i;
                    maxCash = totmoney;
                }
            }
            return winner;
        }


        // other

    }
}