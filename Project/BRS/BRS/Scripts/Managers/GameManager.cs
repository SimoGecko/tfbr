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
        public static int numPlayers = 1;
        public static int lvlScene = 3;
        public const int numRounds = 3;



        //private
        int roundNumber;
        int[] teamWins;
        static State gameState; // CONTROLS STATE OF THE GAME
        bool paused;


        //reference
        public static GameManager instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            gameState = State.playing;
            teamWins = new int[2];
            roundNumber = 1;
        }

        public override void Update() {
            CheckForPause();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands 
        void CheckForPause() {
            if(Input.GetKeyDown(Keys.P) || Input.GetButtonDown(Buttons.Start)) {
                paused = !paused;
                gameState = paused ? State.paused : State.playing;
            }
        }

        public void OnRoundEnd(int winner) {
            teamWins[winner]++;
            gameState = State.finished;
            new Timer(1f, () => RestartCustom(), true);
            BaseUI.instance.UpdateBaseUIWins(winner);
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
            Spawner.instance.Start();
            UserInterface.instance.Start();
            RoundManager.instance.Start();
            PowerupUI.instance.Start();

            GameObject[] bases = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            foreach (var b in bases) b.Start();
            GameObject[] players = GameObject.FindGameObjectsWithTag(ObjectTag.Player);
            foreach (var p in players) p.Start();

            GameObject vault = GameObject.FindGameObjectWithName("vault");
            if (vault != null) vault.Start();

            gameState = State.playing;
            roundNumber++;
        }







        // queries
        public int RoundNumber { get { return roundNumber; } }
        public static bool GameActive { get { return gameState == State.playing; } }
        public static bool GamePaused { get { return gameState == State.paused; } }



        // other

    }

}