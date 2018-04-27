﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts.Managers {
    class GameManager : Component {
        ////////// controls the game state, ie round time, ... //////////

        // --------------------- VARIABLES ---------------------
        public enum State { Menu, Playing, Paused, Finished };

        public static State state = State.Playing; // CONTROLS STATE OF THE GAME

        //public
        public static int NumPlayers = 2; // TODO always check it works with 1, 2, and 4 players
        public static int LvlScene = 4;
        public const int NumRounds = 3;



        //private
        private static int _roundNumber;
        private int[] _teamWins;
        private bool _paused;


        //reference
        public static GameManager Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            state = State.Playing;
            _teamWins = new int[2];
            _roundNumber = 1;
        }

        public override void Update() {
            CheckForPause();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands 
        void CheckForPause() {
            if (!(state == State.Playing || state == State.Paused)) return;
            if(Input.GetKeyDown(Keys.P) || Input.GetButtonDown(Buttons.Start)) {
                _paused = !_paused;
                state = _paused ? State.Paused : State.Playing;
            }
        }

        public void OnRoundEnd(int winner) {
            _teamWins[winner]++;
            state = State.Finished;
            BaseUI.Instance.UpdateBaseUIWins(winner);
            new Timer(5, RestartCustom, true);
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
            ElementManager.Instance.Restart(); 
            Spawner.Instance.Start();
            RoundManager.Instance.Start();
            //PowerupUI.instance.Start();

            Time.ClearTimers();

            // Reset the comopnents: positions, state of vault, player, etc..
            foreach (var g in GameObject.All) g.Reset();

            _roundNumber++;
        }







        // queries
        public static int RoundNumber { get { return _roundNumber; } }
        public static bool GameActive { get { return state == State.Playing; } }
        public static bool GamePaused { get { return state == State.Paused; } }



        // other

    }

}