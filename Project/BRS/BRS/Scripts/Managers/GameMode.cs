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

        //public
        public readonly Dictionary<string, float> MoneyDistribution = new Dictionary<string, float> {
            { "cash", .8f }, { "gold", .2f }, { "diamond", .01f} };

        public readonly Dictionary<string, float> CashStackDistribution = new Dictionary<string, float> {
            { "1", .6f }, { "2", .3f }, { "4", .15f }};

        public readonly Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> {
            { "bomb", .3f }, { "stamina", .1f }, { "capacity", .0f }, { "key", .1f }, { "health", .0f }, { "shield", .0f },
            { "speed", .0f }, { "trap", .3f }, { "explodingbox", .2f }, { "weight", .3f }, { "magnet", .3f } };


        //private
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

        public float ProbOfDiamond = .5f;


        //reference


        // --------------------- BASE METHODS ------------------



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}