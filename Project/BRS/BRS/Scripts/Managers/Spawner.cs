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
        GameMode _currentMode; // to access actual parameters


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _currentMode = GameMode.GetCurrentGameMode();

            SpawnInitialValuables();
            SpawnInitialCrates();
            SpawnInitialPowerup();

            SpawnValuableContinuous();
            SpawnCrateContinuous();
            SpawnPowerupContinuous();
            //SpawnDiamondCasual();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SpawnInitialValuables() {
            for(int i=0; i<_currentMode.StartValuableAmount; i++) {
                SpawnOneValuableRandom();
            }
        }

        void SpawnOneValuableRandom() {
            string valuable = _currentMode.RandomValuable;
            switch (valuable) {
                case "cash": SpawnRandomStack(); break;
                case "gold": SpawnRandomGold(); break;
                case "diamond": SpawnRandomDiamond(); break;
                default: Debug.LogError("weird case"); break;
            }
        }

        void SpawnRandomStack()   { SpawnClusterAt(Heatmap.instance.GetCashPos().To3(), "cashPrefab", EvaluateStackSizeDistribution()); }
        void SpawnRandomGold()    { SpawnMoneyAt(Heatmap.instance.GetGoldPos().To3(), "goldPrefab"); }
        void SpawnRandomDiamond() { SpawnMoneyAt(Heatmap.instance.GetUniformPos().To3(), "diamondPrefab"); }

        public void SpawnMoneyAround(Vector3 p, float radius, string moneyType = "") { // cash, gold, diamond
            if (moneyType == "") moneyType = _currentMode.RandomValuable;
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
            if (moneyType == "") moneyType = _currentMode.RandomValuable;
            p.Y = MathHelper.Max(p.Y, 0.5f);
            SpawnMoneyAt(p, moneyType + "Prefab", MyRandom.UpsideLinearVelocity() * radius);
        }
        

        void SpawnOneDiamondRandom() {
            Vector2 position = MyRandom.InsideRectangle(PlayArea.SpawnArea);
            SpawnMoneyAt(position.To3(), "diamondPrefab", Vector3.Zero);
        }

        void SpawnMoneyAt(Vector3 pos, string prefab, Vector3? linearVelocity = null) {//cashPrefab, goldPrefab, diamondPrefab
            pos.Y = Math.Max(pos.Y, 1.0f);

            GameObject newMoney = GameObject.Instantiate(prefab, pos, MyRandom.YRotation(), linearVelocity??Vector3.Zero);
            ElementManager.Instance.Add(newMoney.GetComponent<Money>());
        }



        // CRATE
        void SpawnInitialCrates() {
            for (int i = 0; i < _currentMode.StartCrateAmount; i++)
                SpawnOneCrateRandom();
        }

        void SpawnOneCrateRandom() {
            GameObject newCrate = GameObject.Instantiate("cratePrefab", RandomPos() + Vector3.Up * .5f, Quaternion.Identity);
            ElementManager.Instance.Add(newCrate.GetComponent<Crate>());
        }


        // POWERUP
        void SpawnInitialPowerup() {
            for (int i = 0; i < _currentMode.StartPowerupAmount; i++)
                SpawnOnePowerupRandom();
        }

        void SpawnOnePowerupRandom() {
            SpawnOnePowerupAt(RandomPos());
        }

        public void SpawnPowerupAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.InsideUnitCircle().To3() * radius;
            SpawnOnePowerupAt(pos);
        }

        public void SpawnPowerupFromCenter(Vector3 p, float radius) {
            p.Y = MathHelper.Max(p.Y, 0.5f);
            SpawnOnePowerupAt(p, MyRandom.UpsideLinearVelocity() * radius);
        }

        void SpawnOnePowerupAt(Vector3 position, Vector3? linearVelocity=null) {
            position.Y = Math.Max(position.Y, 1.5f);

            GameObject newPowerup = GameObject.Instantiate(_currentMode.RandomPowerup + "Prefab", position, Quaternion.Identity, linearVelocity??Vector3.Zero);
            ElementManager.Instance.Add(newPowerup.GetComponent<Powerup>());
        }



        // queries
        Vector3 RandomPos() {
            return Heatmap.instance.GetUniformPos().To3();
        }
        float TimeVariance { get { return MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer); } } // to spice things up

        int EvaluateStackSizeDistribution() {
            return MyRandom.Range(1, 5);
            //return Int32.Parse(Utility.EvaluateDistribution(_currentMode.CashStackDistribution));
        }


        // OTHER
        async void SpawnStuffCoroutine(Action action, float timeInterval) {
            const float coroutineWaitTime = 3f; // since time between rounds is 5, so we are sure to hit somewhere in there

            while (GameManager.GameEnded) await Time.WaitForSeconds(1); // intial countdown

            float remainingTime = timeInterval * TimeVariance;
            while (!GameManager.GameEnded) {
                if (remainingTime > coroutineWaitTime) {
                    await Time.WaitForSeconds(coroutineWaitTime);
                    remainingTime -= coroutineWaitTime;
                } else {
                    await Time.WaitForSeconds(remainingTime);
                    action();
                    remainingTime = timeInterval * TimeVariance;
                }
            }
        }

        void SpawnValuableContinuous() {
            SpawnStuffCoroutine(SpawnOneValuableRandom, _currentMode.TimeBetweenValuables);
        }
        void SpawnCrateContinuous() {
            SpawnStuffCoroutine(SpawnOneCrateRandom, _currentMode.TimeBetweenCrates);
        }
        void SpawnPowerupContinuous() {
            SpawnStuffCoroutine(SpawnOnePowerupRandom, _currentMode.TimeBetweenPowerups);
        }

        /*
        async void SpawnValuableContinuous() {
            float remainingTime = _currentMode.TimeBetweenValuables * TimeVariance;
            while (!GameManager.GameEnded) {
                if (remainingTime > coroutineWaitTime) {
                    await Time.WaitForSeconds(coroutineWaitTime);
                    remainingTime -= coroutineWaitTime;
                } else {
                    await Time.WaitForSeconds(remainingTime);
                    SpawnOneValuableRandom();
                    remainingTime = _currentMode.TimeBetweenValuables * TimeVariance;
                }
            }
        }

        async void SpawnCrateContinuous() {
            while (!GameManager.GameEnded) {
                await Time.WaitForSeconds(_currentMode.TimeBetweenCrates * TimeVariance);
                SpawnOneCrateRandom();
            }
        }

        async void SpawnPowerupContinuous() {
            while (!GameManager.GameEnded) {
                await Time.WaitForSeconds(_currentMode.TimeBetweenPowerups * TimeVariance);
                SpawnOnePowerupRandom();
            }
        }*/

        /*
        async void SpawnDiamondCasual() {
            if(MyRandom.Value< _currentMode.ProbOfDiamond) {
                await Time.WaitForSeconds(MyRandom.Range(10, RoundManager.RoundTime));
                SpawnOneDiamondRandom();
            }
        }*/


    }

}