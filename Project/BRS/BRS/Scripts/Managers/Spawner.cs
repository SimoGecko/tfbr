// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BRS.Scripts {
    class Spawner : Component {
        ////////// spawns money randomly on the map //////////

        //TODO move storing objects in scene in some other class

        // --------------------- VARIABLES ---------------------

        //public


        //prob distributions (no need to sum up to 1)
        static readonly Dictionary<string, float> MoneyDistribution   = new Dictionary<string, float> {
            { "money1", .6f }, { "money3", .4f }, { "money10", .2f }, { "gold", .1f } };

        private static readonly Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> {
            { "bomb", .1f }, { "stamina", .1f }, { "capacity", .1f }, { "key", .1f }, { "health", .1f }, { "shield", .1f },
            { "speed", .1f }, { "trap", .1f }, { "explodingbox", .1f }, { "weight", .1f }, { "magnet", 3.0f } };



        //private
        private const int MoneyAmount = 50;
        private const int VaultGold = 20;
        private const int CrateAmount = 10;
        private const int PowerupAmount = 15;

        private const float TimeBetweenCashSpawn = 10f;
        private const float TimeBetweenCrateSpawn = 30f;
        private const float TimeBetweenPowerupSpawn = 30;

        private const float TimeRandomizer = .2f;

        private static Rectangle _spawnArea = new Rectangle(-25, 5, 50, -80);

        //reference
        public static Spawner Instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;

            for(int k=1; k<=10; k++) {
                SpawnKCashAt(new Vector3(-10+ k*2, 0, -5), k);
            }
            
            SpawnInitialMoney();
            SpawnInitialVaultGold();
            SpawnInitialCrates();
            SpawnInitialPowerup();

            
            SpawnCashContinuous();
            SpawnCrateContinuous();
            SpawnPowerupContinuous();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands

        // MONEY
        void SpawnInitialMoney() {
            for (int i = 0; i < MoneyAmount; i++)
                SpawnOneMoneyRandom();
        }


        void SpawnOneMoneyRandom() {
            Vector2 sample = new Vector2(MyRandom.Value, (float)Math.Sqrt(MyRandom.Value)); // distribution more dense above
            //Vector2 sample = new Vector2(MyRandom.Value, Utility.InverseCDF(MyRandom.Value, .5f)); // todo fix (doesn't work)
            Vector2 position = _spawnArea.Evaluate(sample);
            SpawnOneMoneyAt(position.To3());
        }

        public void SpawnMoneyAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.InsideUnitCircle().To3() * radius;
            SpawnOneMoneyAt(pos);
        }

        void SpawnOneMoneyAt(Vector3 pos) {
            pos += new Vector3(0, 5, 0);
            string prefabName = Utility.EvaluateDistribution(MoneyDistribution) + "Prefab";
            GameObject newmoney = GameObject.Instantiate(prefabName, pos, MyRandom.YRotation());
            Elements.Instance.Add(newmoney.GetComponent<Money>());
        }

        void SpawnKCashAt(Vector3 position, int k) {
            //spqwns a stack of cash
            float radius = .5f;
            float thickness = .1f;
            for(int i=0; i<k; i++) {
                int lvl = (int)Math.Log(i+1);
                Vector3 pos = MyRandom.InsideUnitCircle().To3() * radius * (float)Math.Pow(.8f, lvl) + Vector3.Up * thickness * lvl + position;
                GameObject newmoney = GameObject.Instantiate("money1Prefab", pos, MyRandom.YRotation());
                Elements.Instance.Add(newmoney.GetComponent<Money>());
            }
        }

        void SpawnInitialVaultGold() {
            for (int i = 0; i < VaultGold; i++) {
                Vector2 position = MyRandom.InsideRectangle(Vault.VaultArea);
                GameObject newGold = GameObject.Instantiate("goldPrefab", position.To3(), MyRandom.YRotation());
                Elements.Instance.Add(newGold.GetComponent<Money>());
            }
        }

        // CRATE
        void SpawnInitialCrates() {
            for (int i = 0; i < CrateAmount; i++)
                SpawnOneCrateRandom();
        }

        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(_spawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up * .25f, Quaternion.Identity);
            Elements.Instance.Add(newCrate.GetComponent<Crate>());
        }


        // POWERUP
        void SpawnInitialPowerup() {
            for (int i = 0; i < PowerupAmount; i++)
                SpawnOnePowerupRandom();
                //SpawnOnePowerupAt(new Vector3(i * 3, 0, -10));
        }

        void SpawnOnePowerupRandom() {
            Vector2 position = MyRandom.InsideRectangle(_spawnArea);
            SpawnOnePowerupAt(position.To3());
        }
        public void SpawnPowerupAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.InsideUnitCircle().To3() * radius;
            SpawnOnePowerupAt(pos);
        }

        void SpawnOnePowerupAt(Vector3 position) {
            position += new Vector3(0, 2, 0);
            GameObject newPowerup = GameObject.Instantiate(Utility.EvaluateDistribution(PowerupDistribution) + "Prefab", position + Vector3.Up * .45f, Quaternion.Identity);
            Elements.Instance.Add(newPowerup.GetComponent<Powerup>());
        }


        // queries


        // OTHER
        async void SpawnCashContinuous() {
            while (true) {
                SpawnOneMoneyRandom();
                await Time.WaitForSeconds(TimeBetweenCashSpawn * MyRandom.Range(1 - TimeRandomizer, 1 + TimeRandomizer));
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


    }

}