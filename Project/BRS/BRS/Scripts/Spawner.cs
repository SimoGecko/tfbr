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

        const float probOfCash = .6f;
        const float probOfDiamond = .3f;
        const float probOfGold = .1f;

        const float probOfPowerUp = .1f;

        const float timeBetweenCashSpawn = 1f;


        //private
        List<Money> moneyList;
        List<Crate> crateList;
        List<Powerup> powerupList;


        //reference
        public static Spawner instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;

            moneyList = new List<Money>(); moneyList.Clear();
            crateList = new List<Crate>(); crateList.Clear();
            powerupList = new List<Powerup>(); powerupList.Clear();

            SpawnInitialMoney();
            SpawnInitialCrates();
            SpawnInitialPowerUp();

            SpawnCashContinuous();
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
            string[] namesPowerupsPrefab = { "bombPrefab", "capacityPrefab", "healthPrefab", "shieldPrefab", "speedPrefab" }; // TODO move out of here, spawner should not know names of powerup

            for (int i = 0; i < namesPowerupsPrefab.Length; i++)
                for (int j = 0; j < powerUpAmount; j++)
                    SpawnOnePowerUpRandom(namesPowerupsPrefab[i]);
        }

        // MONEY
        void SpawnOneMoneyRandom() {
            Vector2 sample = new Vector2(MyRandom.Value, (float)Math.Sqrt(MyRandom.Value));
            //Vector2 sample = new Vector2(MyRandom.Value, Utility.InverseCDF(MyRandom.Value, .5f)); // todo fix (doesn't work)
            Vector2 position = spawnArea.Evaluate(sample);
            SpawnOneMoney(position.To3());
        }

        public void SpawnMoneyAround(Vector3 p, float radius) {
            Vector3 pos = p + MyRandom.insideUnitCircle().To3() * radius;
            SpawnOneMoney(pos);
        }

        void SpawnOneMoney(Vector3 pos) {
            GameObject newmoney = GameObject.Instantiate(RandomValuable(), pos, MyRandom.YRotation());
            Money moneyComponent = newmoney.GetComponent<Money>();
            moneyList.Add(moneyComponent);
        }
        

        public void RemoveMoney(Money money) {
            moneyList.Remove(money);
        }

        // CRATE
        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up*.25f, Quaternion.Identity);
            crateList.Add(newCrate.GetComponent<Crate>());
        }
        public void RemoveCrate(Crate crate) {
            crateList.Remove(crate);
        }

        // POWERUP
        void SpawnOnePowerUpRandom(string prefabName) {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            SpawnOnePowerUpAt(position.To3(), prefabName);
        }
        void SpawnOnePowerUpAt(Vector3 position, string prefabName) {
            GameObject newPowerup = GameObject.Instantiate(prefabName, position + Vector3.Up*.45f, Quaternion.Identity);
            powerupList.Add(newPowerup.GetComponent<Powerup>());
            //return newPowerup.GetComponent<Powerup>(); // WHY would you want to return it
        }
        public void RemovePowerup(Powerup powerup) {
            powerupList.Remove(powerup);
        }


        // queries
        string RandomValuable() {
            float val = MyRandom.Value;
            if (val <= probOfCash) return "moneyPrefab";
            val -= probOfCash;
            if (val <= probOfDiamond) return "diamondPrefab";
            return "goldPrefab";
        }


        //TODO move in their own class
        public Vector3[] AllMoneyPosition() {
            List<Vector3> result = new List<Vector3>();
            foreach (Money m in moneyList) result.Add(m.gameObject.Transform.position);
            return result.ToArray();
        }

        public Vector3[] AllCratePosition() {
            List<Vector3> result = new List<Vector3>();
            foreach (Crate c in crateList) result.Add(c.gameObject.Transform.position);
            return result.ToArray();
        }

        public Vector3[] AllPowerupPosition() {
            List<Vector3> result = new List<Vector3>();
            foreach (Powerup p in powerupList) result.Add(p.gameObject.Transform.position);
            return result.ToArray();
        }


        // OTHER
        async void SpawnCashContinuous() {
            while (true) {
                SpawnOneMoneyRandom();
                await Time.WaitForSeconds(timeBetweenCashSpawn);
            }
        }
    }

}