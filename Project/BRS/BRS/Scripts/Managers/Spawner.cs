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
        const int powerupAmount = 15;

        const float timeBetweenCashSpawn = 10f;
        const float timeBetweenCrateSpawn = 30f;
        const float timeBetweenPowerupSpawn = 30;

        const float timeRandomizer = .2f;

        //prob distributions (no need to sum up to 1)
        static Dictionary<string, float> MoneyDistribution   = new Dictionary<string, float> {
            { "money1", .6f }, { "money3", .4f }, { "money10", .2f }, { "gold", .1f } };
        static Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> {
            { "bomb", .1f }, { "stamina", .1f }, { "capacity", .1f }, { "key", .1f }, { "health", .1f }, { "shield", .1f },
            { "speed", .1f }, { "trap", .1f }, { "explodingbox", .1f }, { "weight", .1f }, { "magnet", 3.0f } };
        


        //private



        //reference
        public static Spawner instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;

            for(int k=1; k<=10; k++) {
                SpawnKCashAt(new Vector3(k*4, 0, 0), k);
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
            string prefabName = Utility.EvaluateDistribution(MoneyDistribution) + "Prefab";
            GameObject newmoney = GameObject.Instantiate(prefabName, pos, MyRandom.YRotation());
            Elements.instance.Add(newmoney.GetComponent<Money>());
        }

        void SpawnKCashAt(Vector3 position, int k) {
            //spqwns a stack of cash
            float radius = .5f;
            float thickness = .1f;
            for(int i=0; i<k; i++) {
                int lvl = (int)Math.Log(i);
                Vector3 pos = MyRandom.insideUnitCircle().To3() * radius * (float)Math.Pow(.8f, lvl) + Vector3.Up * thickness * lvl + position;
                GameObject newmoney = GameObject.Instantiate("money1Prefab", pos, MyRandom.YRotation());
                Elements.instance.Add(newmoney.GetComponent<Money>());
            }
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
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up*.25f, Quaternion.Identity);
            Elements.instance.Add(newCrate.GetComponent<Crate>());
        }


        // POWERUP
        void SpawnInitialPowerup() {
            for (int i = 0; i < powerupAmount; i++)
                SpawnOnePowerupRandom();
                //SpawnOnePowerupAt(new Vector3(i * 3, 0, -10));
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
            GameObject newPowerup = GameObject.Instantiate(Utility.EvaluateDistribution(PowerupDistribution) + "Prefab", position + Vector3.Up*.45f, Quaternion.Identity);
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
                await Time.WaitForSeconds(timeBetweenCrateSpawn * MyRandom.Range(1-timeRandomizer, 1+timeRandomizer));
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