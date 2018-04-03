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
        List<Player> playerList = new List<Player>();
        List<Base> baseList = new List<Base>();

        List<Money> moneyList;
        List<Crate> crateList;
        List<Powerup> powerupList;

        //reference
        public static Elements instance;


        // --------------------- BASE METHODS ------------------
        public Elements() {
            instance = this;
        }

        public override void Start() {
            moneyList = new List<Money>(); moneyList.Clear();
            crateList = new List<Crate>(); crateList.Clear();
            powerupList = new List<Powerup>(); powerupList.Clear();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //player
        public void Add(Player p) { playerList.Add(p); } 
        public void Remove(Player p) { playerList.Remove(p); } 

        //base
        public void Add(Base b) { baseList.Add(b); }
        public void Remove(Base b) { baseList.Remove(b); }

        //money
        public void Add(Money m) {  moneyList.Add(m); }
        public void Remove(Money m) { moneyList.Remove(m); }

        //crate
        public void Add(Crate c) { crateList.Add(c); }
        public void Remove(Crate c) { crateList.Remove(c); }

        //powerup
        public void Add(Powerup p) { powerupList.Add(p); }
        public void Remove(Powerup p) { powerupList.Remove(p); }


        // queries
        public Vector3[] AllMoneyPosition() {
            List<Vector3> result = new List<Vector3>();
            foreach (Money m in moneyList) result.Add(m.gameObject.transform.position);
            return result.ToArray();
        }

        public Vector3[] AllCratePosition() {
            List<Vector3> result = new List<Vector3>();
            foreach (Crate c in crateList) result.Add(c.gameObject.transform.position);
            return result.ToArray();
        }

        public Vector3[] AllPowerupPosition() {
            List<Vector3> result = new List<Vector3>();
            foreach (Powerup p in powerupList) result.Add(p.gameObject.transform.position);
            return result.ToArray();
        }

        public Player Player(int i) {
            if (i < 0 || i >= playerList.Count) { Debug.LogError("Player index out of range"); return null; }
            return playerList[i];
        }
        public Player[] Team(int team) {
            Debug.Assert(team < 2, "Invalid team index");
            List<Player> result = new List<Scripts.Player>();
            foreach (Player p in playerList)
                if (p.teamIndex == team) result.Add(p);
            return result.ToArray();
        }
        public Player Enemy(int myteam) {
            //returns a random enemy in the other team
            Player[] enemyTeam = Team(1 - myteam);
            return enemyTeam[MyRandom.Range(0, enemyTeam.Length)];
        }

        public Base Base(int i) {
            if (i < 0 || i >= baseList.Count) { Debug.LogError("Base index out of range"); return null; }
            return baseList[i];
        }
        public Player[] Players() { return playerList.ToArray(); }
        public Base[] Bases()     { return baseList.ToArray(); }


        // other
        public void Restart() {
            foreach (var g in moneyList) GameObject.Destroy(g.gameObject);
            foreach (var g in crateList) GameObject.Destroy(g.gameObject);
            foreach (var g in powerupList) GameObject.Destroy(g.gameObject);
            Start();
        }

    }

}