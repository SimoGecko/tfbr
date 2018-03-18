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
        public static Spawner instance;
        static Rectangle spawnArea = new Rectangle(-25, 5, 50, -80);

        //private
        const int moneyAmount = 50;
        const int crateAmount = 10;
        const int powerUpAmount = 10;

        const float probOfCash = .6f;
        const float probOfDiamond = .3f;
        const float probOfGold = .1f;
        const float probOfPowerUp = .1f;


        //reference
        List<Money> moneyList;
        List<Crate> crateList;
        List<Powerup> powerupList;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            moneyList = new List<Money>();
            crateList = new List<Crate>();
            powerupList = new List<Powerup>();

            instance = this;
            SpawnInitialMoney();
            SpawnInitialCrates();
            SpawnInitialPowerUp();
            SpawnContinuous();
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
            for (int i = 0; i < powerUpAmount; i++)
                SpawnOnePowerUpRandom();
        }

        //money
        void SpawnOneMoneyRandom() {
            Vector2 sample = new Vector2(MyRandom.Value, (float)Math.Sqrt(MyRandom.Value));
            //Vector2 sample = new Vector2(MyRandom.Value, Utility.InverseCDF(MyRandom.Value, .5f)); // todo fix (doesn't work)
            Vector2 position = spawnArea.Evaluate(sample);
            SpawnOneMoney(position.To3());
        }

        void SpawnOneMoney(Vector3 pos) {
            GameObject newmoney = GameObject.Instantiate(RandomValuable(), pos, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(MyRandom.Value*360)));
            Money moneyComponent = newmoney.GetComponent<Money>();
            moneyList.Add(moneyComponent);
        }

        public void RemoveMoney(Money money) {
            moneyList.Remove(money);
        }

        //crate
        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up*.25f, Quaternion.Identity);
            crateList.Add(newCrate.GetComponent<Crate>());
        }
        public void RemoveCrate(Crate crate) {
            crateList.Remove(crate);
        }

        //power up
        void SpawnOnePowerUpRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            GameObject newPowerup = GameObject.Instantiate("powerUpPrefab", position.To3() + Vector3.Up * .25f, Quaternion.Identity);
            powerupList.Add(newPowerup.GetComponent<Powerup>());
        }
        public Powerup SpawnOnePowerUpAt(Vector3 position) {
            GameObject newPowerup = GameObject.Instantiate("powerUpPrefab", position + Vector3.Up * .25f, Quaternion.Identity);
            powerupList.Add(newPowerup.GetComponent<Powerup>());
            return newPowerup.GetComponent<Powerup>();
        }
        public void RemovePowerup(Powerup powerup) {
            powerupList.Remove(powerup);
        }

        async void SpawnContinuous() {
            float timeBetweenSpawn = 1f;
            while (true) {
                SpawnOneMoneyRandom();
                await Time.WaitForSeconds(timeBetweenSpawn);
            }
        }

        public void SpawnMoneyAround(Vector3 p) {
            float radius = 1f;
            Vector3 pos = p + MyRandom.insideUnitCircle().To3() * radius;
            SpawnOneMoney(pos);
        }

        // queries
        string RandomValuable() {
            float val = MyRandom.Value;
            if (val <= probOfCash) return "moneyPrefab";
            val -= probOfCash;
            if (val <= probOfDiamond) return "diamondPrefab";
            return "goldPrefab";
        }

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



        // other

    }

}