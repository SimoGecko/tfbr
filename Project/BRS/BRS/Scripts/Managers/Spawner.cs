// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
using BRS.Scripts.PowerUps;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Managers {
    class Spawner : Component {
        ////////// spawns elements randomly on the map //////////

        //TODO organize class

        // --------------------- VARIABLES ---------------------

        //public


        //prob distributions (no need to sum up to 1)
        //static readonly Dictionary<string, float> MoneyDistribution   = new Dictionary<string, float> {
        //    { "money1", .6f }, { "money2", .4f }, { "money3", .2f }, { "gold", .1f } };
        static readonly Dictionary<string, float> CashStackDistribution   = new Dictionary<string, float> {
            { "1", .6f }, { "2", .3f }, { "4", .15f }};
        static readonly Dictionary<string, float> MoneyDistribution   = new Dictionary<string, float> {
            { "cash", .8f }, { "gold", .2f } };


        private static readonly Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> {
            { "bomb", .3f }, { "stamina", .1f }, { "capacity", .1f }, { "key", .1f }, { "health", .1f }, { "shield", .1f },
            { "speed", .1f }, { "trap", .3f }, { "explodingbox", .2f }, { "weight", .3f }, { "magnet", .2f } };


        const float clusterRadius = .5f;
        const float TimeRandomizer = .2f;

        //private
        private const int CashAmount = 60;
        private const int GoldAmount = 20;
        private const int CrateAmount = 10;
        private const int PowerupAmount = 15;

        private const float TimeBetweenCashSpawn = 10f;
        private const float TimeBetweenGoldSpawn = 30f;
        private const float TimeBetweenCrateSpawn = 30f;
        private const float TimeBetweenPowerupSpawn = 30;
        //private const float TimeBetweenDiamondSpawn = 100;
        float ProbOfDiamond = .5f;



        //reference
        public static Spawner Instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;

            SpawnInitialCash();
            SpawnInitialGold();
            SpawnInitialCrates();
            SpawnInitialPowerup();

            
            SpawnCashContinuous();
            SpawnGoldContinuous();
            SpawnCrateContinuous();
            SpawnPowerupContinuous();
            SpawnDiamondCasual();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SpawnInitialCash() {
            for (int i = 0; i < CashAmount; i++) {
                SpawnClusterAt(Heatmap.instance.GetCashPos().To3(), "cashPrefab", EvaluateStackSizeDistribution());
            }
        }
        void SpawnInitialGold() {
            for (int i = 0; i < GoldAmount; i++)
                SpawnMoneyAt(Heatmap.instance.GetGoldPos().To3(), "goldPrefab");
        }

        public void SpawnMoneyAround(Vector3 p, float radius, string moneyType = "") { // cash, gold, diamond
            if (moneyType == "") moneyType = Utility.EvaluateDistribution(MoneyDistribution);
            moneyType = moneyType.ToLower();
            Vector3 pos = p + MyRandom.InsideUnitCircle().To3() * radius;
            SpawnMoneyAt(pos, moneyType + "Prefab");
        }

        void SpawnClusterAt(Vector3 pos, string prefab, int number) {
            for (int i = 0; i < number; i++) {
                SpawnMoneyAt(pos + MyRandom.InsideUnitCircle().To3() * clusterRadius, prefab);
            }
        }

        void SpawnMoneyAt(Vector3 pos, string prefab) {//cashPrefab, goldPrefab, diamondPrefab
            GameObject newMoney = GameObject.Instantiate(prefab, pos, MyRandom.YRotation());
            ElementManager.Instance.Add(newMoney.GetComponent<Money>());
        }

        void SpawnOneDiamondRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            SpawnMoneyAt(position.To3(), "diamondPrefab");
        }

        // MONEY CASH
        /*

        void SpawnOneMoneyRandom() {

            //Vector2 sample = new Vector2(MyRandom.Value, Utility.InverseCDF(MyRandom.Value, .5f)); // todo fix (doesn't work)

            //Vector2 sample = new Vector2(MyRandom.Value, (float)Math.Sqrt(MyRandom.Value)); // distribution more dense above
            //Vector2 position = _spawnArea.Evaluate(sample);
            //SpawnOneMoneyAt(position.To3());
            
            SpawnOneMoneyAt(Heatmap.instance.GetMoneyPos().To3());
        }

        

        void SpawnOneMoneyAt(Vector3 pos) {
            pos += new Vector3(0, 5, 0);
            string prefabName = Utility.EvaluateDistribution(MoneyDistribution) + "Prefab";
            GameObject newmoney = GameObject.Instantiate(prefabName, pos, MyRandom.YRotation());
            ElementManager.Instance.Add(newmoney.GetComponent<Money>());
        }

        void SpawnKCashFlatAt(Vector3 position, int k) {

        }

        void SpawnKCashAt(Vector3 position, int k) {
            //spqwns a stack of cash
            float radius = .5f;
            float thickness = .1f;
            for(int i=0; i<k; i++) {
                int lvl = (int)Math.Log(i+1);
                Vector3 pos = MyRandom.InsideUnitCircle().To3() * radius * (float)Math.Pow(.8f, lvl) + Vector3.Up * thickness * lvl + position;
                GameObject newmoney = GameObject.Instantiate("money1Prefab", pos, MyRandom.YRotation());
                ElementManager.Instance.Add(newmoney.GetComponent<Money>());
            }
        }

        void SpawnInitialVaultGold() {
            for (int i = 0; i < GoldAmount; i++) {
                Vector2 position = MyRandom.InsideRectangle(Vault.VaultArea);
                GameObject newGold = GameObject.Instantiate("goldPrefab", position.To3(), MyRandom.YRotation());
                ElementManager.Instance.Add(newGold.GetComponent<Money>());
            }
        }*/





        // CRATE
        void SpawnInitialCrates() {
            for (int i = 0; i < CrateAmount; i++)
                SpawnOneCrateRandom();
        }

        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up * .25f, Quaternion.Identity);
            ElementManager.Instance.Add(newCrate.GetComponent<Crate>());
        }


        // POWERUP
        void SpawnInitialPowerup() {
            for (int i = 0; i < PowerupAmount; i++)
                SpawnOnePowerupRandom();
                //SpawnOnePowerupAt(new Vector3(i * 3, 0, -10));
        }

        void SpawnOnePowerupRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            SpawnOnePowerupAt(position.To3());
        }
        public void SpawnPowerupAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.InsideUnitCircle().To3() * radius;
            SpawnOnePowerupAt(pos);
        }

        void SpawnOnePowerupAt(Vector3 position) {
            position += new Vector3(0, 2, 0);
            GameObject newPowerup = GameObject.Instantiate(Utility.EvaluateDistribution(PowerupDistribution) + "Prefab", position + Vector3.Up * .45f, Quaternion.Identity);
            ElementManager.Instance.Add(newPowerup.GetComponent<Powerup>());
        }

        //
        
        


        // queries
        int EvaluateStackSizeDistribution() {
            return Int32.Parse(Utility.EvaluateDistribution(CashStackDistribution));
        }


        // OTHER
        async void SpawnCashContinuous() {
            while (true) {
                SpawnClusterAt(Heatmap.instance.GetCashPos().To3(), "cashPrefab", EvaluateStackSizeDistribution());
                await Time.WaitForSeconds(TimeBetweenCashSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnGoldContinuous() {
            while (true) {
                SpawnMoneyAt(Heatmap.instance.GetGoldPos().To3(), "goldPrefab");
                await Time.WaitForSeconds(TimeBetweenGoldSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnCrateContinuous() {
            while (true) {
                SpawnOneCrateRandom();
                await Time.WaitForSeconds(TimeBetweenCrateSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnPowerupContinuous() {
            while (true) {
                SpawnOnePowerupRandom();
                await Time.WaitForSeconds(TimeBetweenPowerupSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }
        //
        async void SpawnDiamondCasual() {
            if(MyRandom.Value<ProbOfDiamond) {
                await Time.WaitForSeconds(MyRandom.Range(10, RoundManager.RoundTime));
                SpawnOneDiamondRandom();
            }
        }


    }

}