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

        //public
        public static int numPlayers = 2;
        public static int lvlScene = 3;

        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Timer rt = new Timer(2, 00, OnRoundEnd);
            UserInterface.instance.roundtime = rt;
        }

        public override void Update() {
            
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void OnRoundEnd() {
            Debug.Log("round end");
            int winner = FindWinner();
            Debug.Log("Player " + winner + " won!");

        }

        int FindWinner() {
            GameObject[] bases = GameObject.FindGameObjectsWithTag("base");
            int winner = 0;
            int maxCash = bases[0].GetComponent<Base>().TotalMoney;
            for (int i = 1; i < numPlayers; i++) {
                int totmoney = bases[i].GetComponent<Base>().TotalMoney;
                if (totmoney > maxCash) {
                    winner = i;
                    maxCash = totmoney;
                }
            }
            return winner;
        }


        // queries



        // other

    }

}