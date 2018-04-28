// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Elements;
using BRS.Scripts.PowerUps;
using Microsoft.Xna.Framework;
using System;

namespace BRS.Scripts.Managers {
    class Spawner : Component {
        ////////// spawns elements randomly on the map //////////


        // --------------------- VARIABLES ---------------------

        //public

        const float ClusterRadius = .5f;
        const float TimeRandomizer = .2f;

        //private
        

        //reference
        public static Spawner Instance;
        GameMode _currentMode = new GameMode(); // to access actual parameters


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
            for (int i = 0; i < _currentMode.CashAmount; i++) {
                SpawnClusterAt(Heatmap.instance.GetCashPos().To3(), "cashPrefab", _currentMode.CashStackDistribution.Evaluate());
            }
        }
        void SpawnInitialGold() {
            for (int i = 0; i < _currentMode.GoldAmount; i++)
                SpawnMoneyAt(Heatmap.instance.GetGoldPos().To3(), "goldPrefab", Vector3.Zero);
        }

        public void SpawnMoneyAround(Vector3 p, float radius, string moneyType = "") { // cash, gold, diamond
            if (moneyType == "") moneyType = _currentMode.MoneyDistribution.Evaluate();
            moneyType = moneyType.ToLower();
            Vector3 pos = p + MyRandom.InsideUnitCircle().To3() * radius;
            SpawnMoneyAt(pos, moneyType + "Prefab", Vector3.Zero);
        }

        void SpawnClusterAt(Vector3 pos, string prefab, int number) {
            for (int i = 0; i < number; i++) {
                SpawnMoneyAt(pos + MyRandom.InsideUnitCircle().To3() * ClusterRadius, prefab, Vector3.Zero);
            }
        }

        public void SpawnMoneyFromCenter(Vector3 p, float radius, string moneyType = "") {
            if (moneyType == "") moneyType = _currentMode.MoneyDistribution.Evaluate();
            p.Y = MathHelper.Max(p.Y, 0.5f);
            SpawnMoneyAt(p, moneyType + "Prefab", MyRandom.UpsideLinearVelocity() * radius);
        }
        

        void SpawnOneDiamondRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            SpawnMoneyAt(position.To3(), "diamondPrefab", Vector3.Zero);
        }

        void SpawnMoneyAt(Vector3 pos, string prefab, Vector3 linearVelocity) {//cashPrefab, goldPrefab, diamondPrefab
            GameObject newMoney = GameObject.Instantiate(prefab, pos, MyRandom.YRotation(), linearVelocity);
            ElementManager.Instance.Add(newMoney.GetComponent<Money>());
        }



        // CRATE
        void SpawnInitialCrates() {
            for (int i = 0; i < _currentMode.CrateAmount; i++)
                SpawnOneCrateRandom();
        }

        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up * .25f, Quaternion.Identity);
            ElementManager.Instance.Add(newCrate.GetComponent<Crate>());
        }


        // POWERUP
        void SpawnInitialPowerup() {
            for (int i = 0; i < _currentMode.PowerupAmount; i++)
                SpawnOnePowerupRandom();
                //SpawnOnePowerupAt(new Vector3(i * 3, 0, -10));
        }

        void SpawnOnePowerupRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            SpawnOnePowerupAt(position.To3(), Vector3.Zero);
        }

        public void SpawnPowerupAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.InsideUnitCircle().To3() * radius;
            SpawnOnePowerupAt(pos, Vector3.Zero);
        }

        public void SpawnPowerupFromCenter(Vector3 p, float radius) {
            p.Y = MathHelper.Max(p.Y, 0.5f);
            SpawnOnePowerupAt(p, MyRandom.UpsideLinearVelocity() * radius);
        }

        void SpawnOnePowerupAt(Vector3 position, Vector3 linearVelocity) {
            position += new Vector3(0, 2, 0);
            GameObject newPowerup = GameObject.Instantiate(_currentMode.PowerupDistribution.Evaluate() + "Prefab", position + Vector3.Up * .45f, Quaternion.Identity, linearVelocity);
            ElementManager.Instance.Add(newPowerup.GetComponent<Powerup>());
        }

        //




        // queries


        // OTHER
        async void SpawnCashContinuous() {
            while (true) {
                SpawnClusterAt(Heatmap.instance.GetCashPos().To3(), "cashPrefab", _currentMode.CashStackDistribution.Evaluate());
                await Time.WaitForSeconds(_currentMode.TimeBetweenCashSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnGoldContinuous() {
            while (true) {
                SpawnMoneyAt(Heatmap.instance.GetGoldPos().To3(), "goldPrefab", Vector3.Zero);
                await Time.WaitForSeconds(_currentMode.TimeBetweenGoldSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnCrateContinuous() {
            while (true) {
                SpawnOneCrateRandom();
                await Time.WaitForSeconds(_currentMode.TimeBetweenCrateSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        async void SpawnPowerupContinuous() {
            while (true) {
                SpawnOnePowerupRandom();
                await Time.WaitForSeconds(_currentMode.TimeBetweenPowerupSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
            }
        }

        //
        async void SpawnDiamondCasual() {
            if(MyRandom.Value< _currentMode.ProbOfDiamond) {
                await Time.WaitForSeconds(MyRandom.Range(10, RoundManager.RoundTime));
                SpawnOneDiamondRandom();
            }
        }


    }

}