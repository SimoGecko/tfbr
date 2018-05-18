// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Elements;

namespace BRS.Scripts {
    class GameMode  {
        ////////// class that represents a game mode with all parameters //////////
        public enum WinCondition { Time, Cash} // if you win after a certain time or by accumulating cash

        // --------------------- VARIABLES ---------------------
        //static
        static Dictionary<string, GameMode> gameModes = new Dictionary<string, GameMode>();
        public static string currentGameMode = "default";

        //gamemode params
        string gameModeName;

        int roundTime;
        int maxCashToWin;
        WinCondition winCondition;

        Dictionary<string, float> ValuableDistribution;
        Dictionary<string, float> PowerupDistribution;

        float percentAtBeginning;

        int ValuableAmount;
        int CrateAmount;
        int PowerupAmount;


        // --------------------- BASE METHODS ------------------

        public static void Start() {
            ReadGameModes();
        }



        // --------------------- CREATION METHODS ------------------
        GameMode(string name) {
            gameModeName = name;
            gameModes.Add(name, this);
        }

        void SetBaseParams(int rt, int maxcash, WinCondition wincon = WinCondition.Time) {
            roundTime = rt; maxCashToWin = maxcash; winCondition = wincon;
        }

        void SetMoneyDistrib(float cash, float gold, float diamond) {
            ValuableDistribution = new Dictionary<string, float> {
                { "cash", cash }, { "gold", gold }, { "diamond", diamond } };
        }

        void SetPowerupDistribution(float bomb, float key, float weight, float magnet, float trap, float explodingbox, float stamina) {
            PowerupDistribution = new Dictionary<string, float> {
                { "bomb", bomb }, { "key", key }, { "weight", weight }, { "magnet", magnet },
                { "trap", trap }, { "explodingbox", explodingbox }, { "stamina", stamina } };

        }

        void SetAmount(int money, int crate, int powerup, float pab) {
            percentAtBeginning = pab;
            ValuableAmount = money; CrateAmount = crate; PowerupAmount = powerup;
        }

        // --------------------- QUERIES ------------------
        public int StartValuableAmount { get { return (int)System.Math.Round(ValuableAmount * percentAtBeginning); } }
        public int StartCrateAmount    { get { return (int)System.Math.Round(CrateAmount    * percentAtBeginning); } }
        public int StartPowerupAmount  { get { return (int)System.Math.Round(PowerupAmount  * percentAtBeginning); } }

        public float TimeBetweenValuables { get { return ((float)roundTime) / (ValuableAmount - StartValuableAmount+1); } } // +1 to avoid division by 0
        public float TimeBetweenCrates    { get { return ((float)roundTime) / (CrateAmount - StartCrateAmount+1); } }
        public float TimeBetweenPowerups  { get { return ((float)roundTime) / (PowerupAmount - StartPowerupAmount+1); } }

        public string RandomValuable { get { return Utility.EvaluateDistribution(ValuableDistribution); } }
        public string RandomPowerup  { get { return Utility.EvaluateDistribution(PowerupDistribution); } }

        public string RandomPowerupBiased { get { return ConvertKeyToBomb(RandomPowerup); } }
        public static string ConvertKeyToBomb(string s) { return (s == "key" && Vault.instance.IsOpen()) ? "bomb" : s; }

        // --------------------- CREATION METHODS ----------------


        // commands
        public static void ReadGameModes() {
            CreateDefaultMode();
            CreateSurvivalMode();
            CreateBomberMode();
            CreateCrateOnlyMode();
        }

        static void CreateDefaultMode() {
            GameMode gm = new GameMode("default");
            gm.SetBaseParams(150, 20000, WinCondition.Time);
            gm.SetAmount(80, 30, 50, .5f);
            gm.SetMoneyDistrib(.8f, .2f, .05f);
            gm.SetPowerupDistribution(.1f, .1f, .1f, .1f, .1f, .1f, .1f);
        }

        static void CreateSurvivalMode() {
            GameMode gm = new GameMode("survival");
            gm.SetBaseParams(150, 20000, WinCondition.Time);
            gm.SetAmount(20, 0, 4, .3f);
            gm.SetMoneyDistrib(0f, .3f, .0f);
            gm.SetPowerupDistribution(1, 0, 1, 0, 1, 1, 0);
        }

        static void CreateBomberMode() {
            GameMode gm = new GameMode("bomber");
            gm.SetBaseParams(150, 20000, WinCondition.Time);
            gm.SetAmount(100, 0, 20, 1f);
            gm.SetMoneyDistrib(1f, .3f, .02f);
            gm.SetPowerupDistribution(1, 0, 0, 0, 0, 0, 0);
        }
        static void CreateCrateOnlyMode() {
            GameMode gm = new GameMode("crateonly");
            gm.SetBaseParams(150, 20000, WinCondition.Time);
            gm.SetAmount(0, 50, 0, 1f);
            gm.SetMoneyDistrib(1f, .3f, .05f);
            gm.SetPowerupDistribution(1, 0, 1, 1, 1, 3, 0);
        }




        // --------------------- ACCESS ----------------


        public static GameMode GetGameMode(string name) {
            Debug.Assert(gameModes.ContainsKey(name), "No gamemode exists with name " + name);
            return gameModes[name];
        }
        public static GameMode GetCurrentGameMode() {
            return GetGameMode(currentGameMode);
        }
    }
}