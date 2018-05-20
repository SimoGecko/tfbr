// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Menu;
using Microsoft.Xna.Framework.Input;
using System;

namespace BRS.Scripts.Managers {
    static class GameManager {
        ////////// controls the game state ... //////////

        // --------------------- VARIABLES ---------------------
        public enum State { Menu, Playing, Paused, Ended };

        public static State state; // CONTROLS STATE OF THE GAME

        //public
        public static int NumPlayers = 2; // TODO always check it works with 1, 2, and 4 players
        public static int LvlScene = 1;
        public static int lvlDifficulty = 0; // 0 for Easy, 1 for Normal, 2 for Hard


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public static void Start() {
            state = State.Playing;
        }

        public static void Update() {//TODO call this
            CheckForPause();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands 
        static Tuple<bool, int> CheckPauseForAllControllers() {
            bool pressed = false;
            int controllerNo = 0;

            for (int i = 0; i < NumPlayers; ++i) {
                if (Input.GetButtonDown(Buttons.Start, i)) {
                    pressed = true;
                    controllerNo = i;
                }
            }

            return new Tuple<bool, int>(pressed, controllerNo);
        }

        static void CheckForPause() {
            Tuple<bool, int> controllerPaused = CheckPauseForAllControllers();
            if (Input.GetKeyDown(Keys.P) || controllerPaused.Item1) {
                if (state == State.Playing || state == State.Paused)
                    state = GamePaused ? State.Playing : State.Paused;

                if (MenuManager.Instance.MenuRect.ContainsKey("pause")) {
                    MenuManager.Instance.MenuRect["pause"].active = GamePaused;

                    foreach (Component comp in MenuManager.Instance.MenuRect["pause"].components)
                        if (comp is MenuComponent MC)
                            MC.IndexAssociatedPlayerScreen = controllerPaused.Item2;
                    
                }
            }
        }



        public static void RestartCustom() { // TODO refactor
            ElementManager.Instance.Restart();
            //Time.ClearTimers();
            Spawner.Instance.Start();
            //RoundManager.Instance.Start();
            //PowerupUI.instance.Start();
            PoliceManager.Instance.Reset();

            foreach (var g in GameObject.All) g.Reset();

            state = State.Playing;
        }







        // queries
        public static bool GameActive { get { return state == State.Playing; } }
        public static bool GamePaused { get { return state == State.Paused; } }
        public static bool GameEnded { get { return state == State.Ended || state==State.Menu; } }
        public static int NumTeams { get { return NumPlayers == 1 ? 1 : 2; } }
        public static int TeamIndex(int playerIndex) { return playerIndex % 2; }

        // other

    }

}