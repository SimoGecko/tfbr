// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts.Managers {
    static class GameManager {
        ////////// controls the game state ... //////////

        // --------------------- VARIABLES ---------------------
        public enum State { Menu, Playing, Paused, Finished };

        public static State state = State.Playing; // CONTROLS STATE OF THE GAME

        //public
        public static int NumPlayers = 2; // TODO always check it works with 1, 2, and 4 players
        public static int LvlScene = 4;


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
        static void CheckForPause() {
            if(Input.GetKeyDown(Keys.P) || Input.GetButtonDown(Buttons.Start)) {
                if (state == State.Playing || state == State.Paused)
                    state = GamePaused ? State.Playing : State.Paused;
            }
        }



        public static void RestartCustom() { // TODO refactor
            ElementManager.Instance.Restart(); 
            Spawner.Instance.Start();
            //RoundManager.Instance.Start();
            //PowerupUI.instance.Start();
            PoliceManager.Instance.Reset();

            foreach (var b in ElementManager.Instance.Bases()) b.Start();
            foreach (var p in ElementManager.Instance.Players()) p.Start();

            GameObject vault = GameObject.FindGameObjectWithName("vault");
            if (vault != null) vault.Start();

            state = State.Playing;
        }







        // queries
        public static bool GameActive { get { return state == State.Playing; } }
        public static bool GamePaused { get { return state == State.Paused; } }
        public static int NumTeams { get { return NumPlayers == 1 ? 1 : 2; } }


        // other

    }

}