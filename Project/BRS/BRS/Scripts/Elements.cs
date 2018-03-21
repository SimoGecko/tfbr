// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Elements : Component {
        ////////// stores which elements (money/crates/powerups/...) are present on the map and where //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        List<Money> moneyList;
        List<Crate> crateList;
        List<Powerup> powerupList;

        //reference
        public static Elements instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            instance = this;

            moneyList = new List<Money>(); moneyList.Clear();
            crateList = new List<Crate>(); crateList.Clear();
            powerupList = new List<Powerup>(); powerupList.Clear();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //money
        public void Add(Money m) {
            moneyList.Add(m);
        }
        public void Remove(Money m) {
            moneyList.Remove(m);
        }

        //crate
        public void Add(Crate c) {
            crateList.Add(c);
        }
        public void Remove(Crate c) {
            crateList.Remove(c);
        }

        //powerup
        public void Add(Powerup p) {
            powerupList.Add(p);
        }
        public void Remove(Powerup p) {
            powerupList.Remove(p);
        }


        // queries
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


        // other

    }

}