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
        const int crateAmount = 10;
        const int powerUpAmount = 5;

        Dictionary<string, float> MoneyDistribution   = new Dictionary<string, float> { { "money", .6f }, { "diamond", .3f }, { "gold", .1f } };
        Dictionary<string, float> PowerupDistribution = new Dictionary<string, float> { { "bomb", .3f }, { "capacity", .2f }, { "key", .2f }, { "health", .1f }, { "shield", .1f }, { "speed", .1f } };


        /*
        const float probOfCash = .6f;
        const float probOfDiamond = .3f;
        const float probOfGold = .1f;

        const float probOfPowerUp = .1f;
        */

        const float timeBetweenCashSpawn = 1f;


        //private
        


        //reference
        public static Spawner instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            
            SpawnInitialMoney();
            SpawnInitialCrates();
            SpawnInitialPowerUp();

            SpawnCashContinuous();

            GameObject manager = GameObject.FindGameObjectWithName("manager");
            if (!manager.HasComponent<Elements>()) Debug.LogError("ont attackedd");
            else Debug.Log("has instance");
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SpawnInitialMoney() {
            for (int i = 0; i < moneyAmount; i++)
                SpawnOneMoneyRandom();
        }


        void SpawnInitialCrates() {
            for (int i = 0; i < crateAmount; i++)
                SpawnOneCrateRandom();
        }

        void SpawnInitialPowerUp() {
            //for (int i = 0; i < namesPowerupsPrefab.Length; i++)
            for (int i = 0; i < powerUpAmount; i++)
                SpawnOnePowerUpRandom();
        }

        // MONEY
        void SpawnOneMoneyRandom() {
            Vector2 sample = new Vector2(MyRandom.Value, (float)Math.Sqrt(MyRandom.Value)); // distribution more dense above
            //Vector2 sample = new Vector2(MyRandom.Value, Utility.InverseCDF(MyRandom.Value, .5f)); // todo fix (doesn't work)
            Vector2 position = spawnArea.Evaluate(sample);
            SpawnOneMoney(position.To3());
        }

        public void SpawnMoneyAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.insideUnitCircle().To3() * radius;
            SpawnOneMoney(pos);
        }

        void SpawnOneMoney(Vector3 pos) {
            string prefabName = Utility.EvaluateDistribution(MoneyDistribution) + "Prefab";
            GameObject newmoney = GameObject.Instantiate(prefabName, pos, MyRandom.YRotation());
            Elements.instance.Add(newmoney.GetComponent<Money>());
        }

        // CRATE
        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up*.25f, Quaternion.Identity);
            Elements.instance.Add(newCrate.GetComponent<Crate>());
        }
        

        // POWERUP
        void SpawnOnePowerUpRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            SpawnOnePowerUpAt(position.To3());
        }
        public void SpawnPowerupAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.insideUnitCircle().To3() * radius;
            SpawnOnePowerUpAt(pos);
        }

        void SpawnOnePowerUpAt(Vector3 position) {
            GameObject newPowerup = GameObject.Instantiate(Utility.EvaluateDistribution(PowerupDistribution) + "Prefab", position + Vector3.Up*.45f, Quaternion.Identity);
            Elements.instance.Add(newPowerup.GetComponent<Powerup>());
            //return newPowerup.GetComponent<Powerup>(); // WHY would you want to return it
        }


        // queries


        // OTHER
        async void SpawnCashContinuous() {
            while (true) {
                SpawnOneMoneyRandom();
                await Time.WaitForSeconds(timeBetweenCashSpawn);
            }
        }
    }

}