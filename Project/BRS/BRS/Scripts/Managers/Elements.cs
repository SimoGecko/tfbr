// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BRS.Scripts {
    class Elements : Component {
        ////////// stores which elements (money/crates/powerups/...) are present on the map and where //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private readonly List<Player> _playerList = new List<Player>();
        private readonly List<Base> _baseList = new List<Base>();

        private List<Money> _moneyList;
        private List<Crate> _crateList;
        private List<Powerup> _powerupList;

        //reference
        public static Elements Instance;


        // --------------------- BASE METHODS ------------------
        public Elements() {
            Instance = this;
        }

        public override void Start() {
            _moneyList = new List<Money>();
            _moneyList.Clear();

            _crateList = new List<Crate>();
            _crateList.Clear();

            _powerupList = new List<Powerup>();
            _powerupList.Clear();
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //player
        public void Add(Player p) { _playerList.Add(p); }
        public void Remove(Player p) { _playerList.Remove(p); }

        //base
        public void Add(Base b) { _baseList.Add(b); }
        public void Remove(Base b) { _baseList.Remove(b); }

        //money
        public void Add(Money m) { _moneyList.Add(m); }
        public void Remove(Money m) { _moneyList.Remove(m); }

        //crate
        public void Add(Crate c) { _crateList.Add(c); }
        public void Remove(Crate c) { _crateList.Remove(c); }

        //powerup
        public void Add(Powerup p) { _powerupList.Add(p); }
        public void Remove(Powerup p) { _powerupList.Remove(p); }


        // queries
        public Vector3[] AllMoneyPosition() {
            List<Vector3> result = new List<Vector3>();

            foreach (Money m in _moneyList) {
                result.Add(m.GameObject.transform.position);
            }

            return result.ToArray();
        }

        public Vector3[] AllCratePosition() {
            List<Vector3> result = new List<Vector3>();

            foreach (Crate c in _crateList) {
                result.Add(c.GameObject.transform.position);
            }

            return result.ToArray();
        }

        public Vector3[] AllPowerupPosition() {
            List<Vector3> result = new List<Vector3>();

            foreach (Powerup p in _powerupList) {
                result.Add(p.GameObject.transform.position);
            }

            return result.ToArray();
        }

        public Player Player(int i) {
            if (i < 0 || i >= _playerList.Count) {
                Debug.LogError("Player index out of range"); return null;
            }

            return _playerList[i];
        }

        public Player[] Team(int team) {
            Debug.Assert(team < 2, "Invalid team index");
            List<Player> result = new List<Player>();

            foreach (Player p in _playerList) {
                if (p.TeamIndex == team) {
                    result.Add(p);
                }
            }

            return result.ToArray();
        }
        public Player Enemy(int myteam) {
            //returns a random enemy in the other team
            Player[] enemyTeam = Team(1 - myteam);
            return enemyTeam[MyRandom.Range(0, enemyTeam.Length)];
        }

        public Base Base(int i) {
            if (i < 0 || i >= _baseList.Count) {
                Debug.LogError("Base index out of range"); return null;
            }

            return _baseList[i];
        }
        public Player[] Players() { return _playerList.ToArray(); }
        public Base[] Bases() { return _baseList.ToArray(); }


        // other
        public void Restart() {
            foreach (var g in _moneyList) GameObject.Destroy(g.GameObject);
            foreach (var g in _crateList) GameObject.Destroy(g.GameObject);
            foreach (var g in _powerupList) GameObject.Destroy(g.GameObject);
            Start();
        }

    }

}