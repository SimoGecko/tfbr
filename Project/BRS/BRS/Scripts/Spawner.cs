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

        // --------------------- VARIABLES ---------------------

        //public
        public static Spawner instance;
        Rectangle spawnArea = new Rectangle(-10, 5, 20, -25);

        //private
        int moneyAmount = 30;
        int crateAmount = 10;
        int powerUpAmount = 10;

        const float probOfCash = .6f;
        const float probOfDiamond = .3f;
        const float probOfGold = .1f;
        const float probOfPowerUp = .1f;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            SpawnInitialMoney();
            SpawnInitialCrates();
            SpawnInitialPowerUp();
            //SpawnContinuous();
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
            //Vector2 position = MyRandom.InsideRectangle(spawnArea);
            SpawnOneMoney(position.To3());
        }

        void SpawnOneMoney(Vector3 pos) {
            GameObject newmoney = GameObject.Instantiate(RandomValuable(), pos, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(MyRandom.Value*360)));
        }

        //crate
        void SpawnOneCrateRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            GameObject newCrate = GameObject.Instantiate("cratePrefab", position.To3() + Vector3.Up*.25f, Quaternion.Identity);
        }

        //power up
        void SpawnOnePowerUpRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            GameObject newCrate = GameObject.Instantiate("powerUpPrefab", position.To3() + Vector3.Up * .25f, Quaternion.Identity);
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



        // other

    }

}