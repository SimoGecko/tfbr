// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class GameManager : Component {
        ////////// controls the game state, ie round time, ... //////////

        // --------------------- VARIABLES ---------------------
        enum State { playing, paused, finished, menu};

        //public
        public static int numPlayers = 2;
        public static int lvlScene = 3;
        public const int roundTime = 3;
        public const int timeBeforePolice = 20;

        //private
        public static bool gameActive;
        Timer rt;

        //reference
        public static GameManager instance;
        Base[] bases;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            gameActive = true;
            rt = new Timer(0, roundTime, OnRoundEnd);
            UserInterface.instance.roundtime = rt;

            FindBases();
        }

        public override void Update() {
            if (rt.span.TotalSeconds < timeBeforePolice) {
                UserInterface.instance.UpdatePoliceComing();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void OnRoundEnd() {
            gameActive = false;
            NotifyBases();
            int winner = FindWinner();
            UserInterface.instance.UpdateGameWinnerUI(winner);
            new Timer(1f, () => RestartCustom());
        }

        void Restart() { // TODO fix this shit
            //UserInterface.instance.Start();
            //GameObject.ClearAll();
            //Game1.instance.Reset();

            //scene.Start();
            /*
            foreach (GameObject go in GameObject.All) {
                Debug.Log("restart " + go.Name);
                go.Start();
            }*/
        }

        void RestartCustom() { // it still slows down for some reason
            Elements.instance.Restart(); 
            Elements.instance.Start();
            Spawner.instance.Start();
            foreach (Base b in bases) b.Start();
            GameObject[] players = GameObject.FindGameObjectsWithTag(ObjectTag.Player);
            foreach (var p in players) p.Start();
            GameObject vault = GameObject.FindGameObjectWithName("vault");
            if (vault != null) vault.Start();
            UserInterface.instance.Start();

            Start();
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
            for (int i = 1; i < numPlayers; i++) {
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