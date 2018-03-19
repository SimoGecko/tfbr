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
        const int roundTime = 120;

        //private
        public static bool gameActive;

        //reference
        public static GameManager instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            gameActive = true;
            Timer rt = new Timer(0, roundTime, OnRoundEnd);
            UserInterface.instance.roundtime = rt;
        }

        public override void Update() {
            
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void OnRoundEnd() {
            gameActive = false;
            int winner = FindWinner();
            UserInterface.instance.UpdateGameWinnerUI(winner);
            new Timer(1f, () => Restart());
        }

        void Restart() { // TODO fix this shit
            //UserInterface.instance.Start();
            GameObject.ClearAll();
            //Game1.instance.Reset();

            //scene.Start();
            /*
            foreach (GameObject go in GameObject.All) {
                Debug.Log("restart " + go.Name);
                go.Start();
            }*/
        }




        // queries
        int FindWinner() {
            GameObject[] bases = GameObject.FindGameObjectsWithTag("base");
            if (bases.Length < 1) {
                Debug.LogError("could not find the bases"); // avoids tag messup
                return 0;
            }
            int winner = 0;

            int maxCash = bases[0].GetComponent<Base>().TotalMoney;
            for (int i = 1; i < numPlayers; i++) {
                int totmoney = bases[i].GetComponent<Base>().TotalMoney;
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