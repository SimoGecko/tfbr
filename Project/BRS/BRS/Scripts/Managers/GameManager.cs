// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts.Managers {
    class GameManager : Component {
        ////////// controls the game state, ie round time, ... //////////

        // --------------------- VARIABLES ---------------------
        enum State { Playing, Paused, Finished, Menu};

        //public
        public static int NumPlayers = 2;
        public static int LvlScene = 3;
        public const int NumRounds = 3;



        //private
        private int _roundNumber;
        private int[] _teamWins;
        private static State _gameState; // CONTROLS STATE OF THE GAME
        private bool _paused;


        //reference
        public static GameManager Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _gameState = State.Playing;
            _teamWins = new int[2];
            _roundNumber = 1;
        }

        public override void Update() {
            CheckForPause();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands 
        void CheckForPause() {
            if (!(_gameState == State.Playing || _gameState == State.Paused)) return;
            if(Input.GetKeyDown(Keys.P) || Input.GetButtonDown(Buttons.Start)) {
                _paused = !_paused;
                _gameState = _paused ? State.Paused : State.Playing;
            }
        }

        public void OnRoundEnd(int winner) {
            _teamWins[winner]++;
            _gameState = State.Finished;
            new Timer(1, RestartCustom, true);
            BaseUI.Instance.UpdateBaseUIWins(winner);
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
            //UserInterface.instance.Start();
            RoundManager.Instance.Start();
            //PowerupUI.instance.Start();

            //GameObject[] bases = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            foreach (var b in ElementManager.Instance.Bases()) b.Start();
            //GameObject[] players = GameObject.FindGameObjectsWithTag(ObjectTag.Player);
            foreach (var p in ElementManager.Instance.Players()) p.Start();

            GameObject vault = GameObject.FindGameObjectWithName("vault");
            if (vault != null) vault.Start();

            _gameState = State.Playing;
            _roundNumber++;
        }







        // queries
        public int RoundNumber { get { return _roundNumber; } }
        public static bool GameActive { get { return _gameState == State.Playing; } }
        public static bool GamePaused { get { return _gameState == State.Paused; } }



        // other

    }

}