// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BRS.Scripts {
    class Spawner : Component {
        ////////// spawns money randomly on the map //////////

        //TODO move storing objects in scene in some other class

        // --------------------- VARIABLES ---------------------

        //public
        static Rectangle spawnArea = new Rectangle(-25, 5, 50, -80);

        const int moneyAmount = 50;
        const int vaultGold = 20;
        const int crateAmount = 10;
        const int powerupAmount = 30;

        const float timeBetweenCashSpawn = 1f;
        const float timeBetweenCrateSpawn = 5f;
        const float timeBetweenPowerupSpawn = 10f;

        const float timeRandomizer = .2f;

        //prob distributions
        static Dictionary<string, float> MoneyDistribution = new Dictionary<string, float> { { "money", .6f }, { "diamond", .3f }, { "gold", .1f } };
        static Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> { { "bomb", .2f }, { "capacity", .1f }, { "key", .2f }, { "health", .1f }, { "shield", .1f }, { "speed", .1f }, { "trap", .2f } };



        //private



        //reference
        public static Spawner instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;

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
            for (int i = 0; i < moneyAmount; i++)
                SpawnOneMoneyRandom();
        }


        void SpawnOneMoneyRandom() {
            Vector2 sample = new Vector2(MyRandom.Value, (float)Math.Sqrt(MyRandom.Value)); // distribution more dense above
            //Vector2 sample = new Vector2(MyRandom.Value, Utility.InverseCDF(MyRandom.Value, .5f)); // todo fix (doesn't work)
            Vector2 position = spawnArea.Evaluate(sample);
            SpawnOneMoneyAt(position.To3());
        }

        public void SpawnMoneyAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.insideUnitCircle().To3() * radius;
            SpawnOneMoneyAt(pos);
        }

        void SpawnOneMoneyAt(Vector3 pos) {
            pos += new Vector3(0, 5, 0);
            string prefabName = Utility.EvaluateDistribution(MoneyDistribution) + "Prefab";
            GameObject newmoney = GameObject.Instantiate(prefabName, pos, MyRandom.YRotation());
            Elements.instance.Add(newmoney.GetComponent<Money>());
        }

        void SpawnInitialVaultGold() {
            for (int i = 0; i < vaultGold; i++) {
                Vector2 position = MyRandom.InsideRectangle(Vault.vaultArea);
                GameObject newGold = GameObject.Instantiate("goldPrefab", position.To3(), MyRandom.YRotation());
                Elements.instance.Add(newGold.GetComponent<Money>());
            }
        }

        // CRATE
        void SpawnInitialCrates() {
            for (int i = 0; i < crateAmount; i++)
                SpawnOneCrateRandom();
        }

        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up * .25f, Quaternion.Identity);
            Elements.instance.Add(newCrate.GetComponent<Crate>());
        }


        // POWERUP
        void SpawnInitialPowerup() {
            for (int i = 0; i < powerupAmount; i++)
                SpawnOnePowerupRandom();
        }

        void SpawnOnePowerupRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            SpawnOnePowerupAt(position.To3());
        }
        public void SpawnPowerupAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.insideUnitCircle().To3() * radius;
            SpawnOnePowerupAt(pos);
        }

        void SpawnOnePowerupAt(Vector3 position) {
            position += new Vector3(0, 2, 0);
            GameObject newPowerup = GameObject.Instantiate(Utility.EvaluateDistribution(PowerupDistribution) + "Prefab", position + Vector3.Up * .45f, Quaternion.Identity);
            Elements.instance.Add(newPowerup.GetComponent<Powerup>());
        }


        // queries


        // OTHER
        async void SpawnCashContinuous() {
            while (true) {
                SpawnOneMoneyRandom();
                await Time.WaitForSeconds(timeBetweenCashSpawn * MyRandom.Range(1 - timeRandomizer, 1 + timeRandomizer));
            }
        }

        async void SpawnCrateContinuous() {
            while (true) {
                SpawnOneCrateRandom();
                await Time.WaitForSeconds(timeBetweenCrateSpawn * MyRandom.Range(1 - timeRandomizer, 1 + timeRandomizer));
            }
        }

        async void SpawnPowerupContinuous() {
            while (true) {
                SpawnOnePowerupRandom();
                await Time.WaitForSeconds(timeBetweenPowerupSpawn * MyRandom.Range(1 - timeRandomizer, 1 + timeRandomizer));
            }
        }


    }

}