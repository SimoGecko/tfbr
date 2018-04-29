// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Engine.Utilities;

namespace BRS.Scripts {
    class GameMode  {
        ////////// class that represents a game mode with all parameters //////////
        public enum WinCondition { Time, Cash} // if you win after a certain time or by accumulating cash

        // --------------------- VARIABLES ---------------------

        //public
        //public readonly Dictionary<string, float> MoneyDistribution = new Dictionary<string, float> {
        //    { "cash", 0.0f }, { "gold", 0.0f }, { "diamond", 1.0f} };
        //public readonly Dictionary<string, float> MoneyDistribution = new Dictionary<string, float> {
        //    { "cash", .8f }, { "gold", .2f }, { "diamond", .01f} };
        public readonly Distribution<string> MoneyDistribution = new Distribution<string>(new Dictionary<string, float> {
            { "cash", .8f }, { "gold", .2f }, { "diamond", .01f} });

        //public readonly Dictionary<string, float> CashStackDistribution = new Dictionary<string, float> {
        //    { "1", .6f }, { "2", .3f }, { "4", .15f }};
        public readonly Distribution<int> CashStackDistribution = new Distribution<int>(new Dictionary<int, float> {
            { 1, .6f }, { 2, .3f }, { 4, .15f } });

        //public readonly Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> {
        //    { "bomb", .3f }, { "stamina", .1f }, { "capacity", .0f }, { "key", .1f }, { "health", .0f }, { "shield", .0f },
        //    { "speed", .0f }, { "trap", .3f }, { "explodingbox", .2f }, { "weight", .3f }, { "magnet", .3f } };
        public readonly Distribution<string> PowerupDistribution = new Distribution<string>(new Dictionary<string, float> {
            { "bomb", .3f }, { "stamina", .1f }, { "capacity", .0f }, { "key", .1f }, { "health", .0f }, { "shield", .0f },
            { "speed", .0f }, { "trap", .3f }, { "explodingbox", .2f }, { "weight", .3f }, { "magnet", .3f } });


        //private
        public int CashAmount = 80;
        public int GoldAmount = 20;
        public int ValuableAmount = 100;
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