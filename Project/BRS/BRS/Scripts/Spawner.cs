// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class Spawner : Component {
        ////////// spawns money randomly on the map //////////

        // --------------------- VARIABLES ---------------------

        //public
        public static Spawner instance;
        Rectangle spawnArea = new Rectangle(-10, 0, 20, -30);

        //private
        int moneyamount = 30;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;
            SpawnInitialMoney();
            SpawnContinuous();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SpawnInitialMoney() {
            for (int i = 0; i < moneyamount; i++)
                SpawnOneMoneyRandom();
        }

        void SpawnOneMoneyRandom() {
            Vector2 position = MyRandom.InsideRectangle(spawnArea);
            SpawnOneMoney(position.To3());
        }

        void SpawnOneMoney(Vector3 pos) {
            GameObject newmoney = GameObject.Instantiate("moneyprefab", pos, Quaternion.Identity);
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



        // other

    }

}