// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;

namespace BRS.Scripts {
    class GameMode  {
        ////////// class that represents a game mode with all parameters //////////
        public enum WinCondition { Time, Cash} // if you win after a certain time or by accumulating cash

        // --------------------- VARIABLES ---------------------

        string gameModeName;

        int roundTime;
        int maxCashToWin;
        float percentAtBeginning;
        WinCondition winCondition;

        //public
        Dictionary<string, float> ValuableDistribution;
        Dictionary<string, float> PowerupDistribution;
        /*
        public readonly Dictionary<string, float> MoneyDistribution = new Dictionary<string, float> {
            { "cash", .8f }, { "gold", .2f }, { "diamond", .01f} };

        public readonly Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> {
            { "bomb", .3f }, { "key", .1f }, { "weight", .3f }, { "magnet", .3f },
            { "trap", .3f }, { "explodingbox", .2f },  { "stamina", .1f },
            { "health", .0f }, { "shield", .0f }, { "capacity", .0f }, { "speed", .0f }};
        */


        //private
        int ValuableAmount;
        int CrateAmount;
        int PowerupAmount;


        //STATIC
        static Dictionary<string, GameMode> gameModes = new Dictionary<string, GameMode>();
        public static string currentGameMode = "default";

        /*
        public int CashAmount = 40;
        public int GoldAmount = 10;
        //public int ValuableAmount = 100;
        public int CrateAmount = 10;
        public int PowerupAmount = 30;

        public float TimeBetweenValuableSpawn = 10f;
        public float TimeBetweenCashSpawn = 10f;
        public float TimeBetweenGoldSpawn = 10f;
        public float TimeBetweenCrateSpawn = 30f;
        public float TimeBetweenPowerupSpawn = 30f;

        public float ProbOfDiamond = .5f;*/


        //reference

        public static void Start() {
            ReadGameModes();
        }



        // --------------------- CREATION METHODS ------------------
        GameMode(string name) {
            gameModeName = name;
            gameModes.Add(name, this);
        }

        void SetBaseParams(int rt, int maxcash, float pab, WinCondition wincon = WinCondition.Time) {
            roundTime = rt; maxCashToWin = maxcash; percentAtBeginning = pab; winCondition = wincon;
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

        void SetAmount(int money, int crate, int powerup) {
            ValuableAmount = money; CrateAmount = crate; PowerupAmount = powerup;
        }

        // --------------------- QUERIES ------------------
        public int StartValuableAmount { get { return (int)System.Math.Round(ValuableAmount * percentAtBeginning); } }
        public int StartCrateAmount    { get { return (int)System.Math.Round(CrateAmount    * percentAtBeginning); } }
        public int StartPowerupAmount  { get { return (int)System.Math.Round(PowerupAmount  * percentAtBeginning); } }

        public float TimeBetweenValuables { get { return ((float)roundTime) / (ValuableAmount - StartValuableAmount); } }
        public float TimeBetweenCrates    { get { return ((float)roundTime) / (CrateAmount - StartCrateAmount); } }
        public float TimeBetweenPowerups  { get { return ((float)roundTime) / (PowerupAmount - StartPowerupAmount); } }

        public string RandomValuable { get { return Utility.EvaluateDistribution(ValuableDistribution); } }
        public string RandomPowerup  { get { return Utility.EvaluateDistribution(PowerupDistribution); } }


        // --------------------- CREATION METHODS ----------------


        // commands
        public static void ReadGameModes() {
            CreateDefaultMode();
        }

        static void CreateDefaultMode() {
            GameMode def = new GameMode("default");
            def.SetBaseParams(150, 20000, .5f, WinCondition.Time);
            def.SetMoneyDistrib(.8f, .2f, .05f);
            def.SetPowerupDistribution(.1f, .1f, .1f, .1f, .1f, .1f, .1f);
            def.SetAmount(80, 30, 15);
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