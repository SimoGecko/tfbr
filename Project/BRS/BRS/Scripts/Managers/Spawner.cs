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


        // --------------------- VARIABLES ---------------------

        //public

        const float clusterRadius = .5f;
        const float TimeRandomizer = .2f;

        //private
        



        //reference
        public static Spawner Instance;
        GameMode currentMode = new GameMode(); // to access actual parameters



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;

            //SpawnInitialValuables();
            SpawnInitialCash();
            SpawnInitialGold();
            SpawnInitialCrates();
            SpawnInitialPowerup();

            //SpawnValuablesContinuous();
            SpawnCashContinuous();
            SpawnGoldContinuous();
            SpawnCrateContinuous();
            SpawnPowerupContinuous();
            //SpawnDiamondCasual();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SpawnInitialCash() {
            for (int i = 0; i < currentMode.CashAmount; i++) {
                SpawnClusterAt(Heatmap.instance.GetCashPos().To3(), "cashPrefab", EvaluateStackSizeDistribution());
            }
        }
        void SpawnInitialGold() {
            for (int i = 0; i < currentMode.GoldAmount; i++)
                SpawnMoneyAt(Heatmap.instance.GetGoldPos().To3(), "goldPrefab");
        }

        public void SpawnMoneyAround(Vector3 p, float radius, string moneyType = "") { // cash, gold, diamond
            if (moneyType == "") moneyType = Utility.EvaluateDistribution(currentMode.MoneyDistribution);
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



        // CRATE
        void SpawnInitialCrates() {
            for (int i = 0; i < currentMode.CrateAmount; i++)
                SpawnOneCrateRandom();
        }

        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up * .25f, Quaternion.Identity);
            ElementManager.Instance.Add(newCrate.GetComponent<Crate>());
        }


        // POWERUP
        void SpawnInitialPowerup() {
            for (int i = 0; i < currentMode.PowerupAmount; i++)
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
            GameObject newPowerup = GameObject.Instantiate(Utility.EvaluateDistribution(currentMode.PowerupDistribution) + "Prefab", position + Vector3.Up * .45f, Quaternion.Identity);
            ElementManager.Instance.Add(newPowerup.GetComponent<Powerup>());
        }

        //
        
        


        // queries
        int EvaluateStackSizeDistribution() {
            return Int32.Parse(Utility.EvaluateDistribution(currentMode.CashStackDistribution));
        }


        // OTHER
        async void SpawnCashContinuous() {
            while (true) {
                SpawnClusterAt(Heatmap.instance.GetCashPos().To3(), "cashPrefab", EvaluateStackSizeDistribution());
                await Time.WaitForSeconds(currentMode.TimeBetweenCashSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnGoldContinuous() {
            while (true) {
                SpawnMoneyAt(Heatmap.instance.GetGoldPos().To3(), "goldPrefab");
                await Time.WaitForSeconds(currentMode.TimeBetweenGoldSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnCrateContinuous() {
            while (true) {
                SpawnOneCrateRandom();
                await Time.WaitForSeconds(currentMode.TimeBetweenCrateSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnPowerupContinuous() {
            while (true) {
                SpawnOnePowerupRandom();
                await Time.WaitForSeconds(currentMode.TimeBetweenPowerupSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }
        //
        async void SpawnDiamondCasual() {
            if(MyRandom.Value< currentMode.ProbOfDiamond) {
                await Time.WaitForSeconds(MyRandom.Range(10, RoundManager.RoundTime));
                SpawnOneDiamondRandom();
            }
        }


    }

}