// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.Elements {
    class Crate : Component, IDamageable {
        ////////// represents a crate that can be cracked when attacked and reveals money and powerup inside //////////

        // --------------------- VARIABLES ---------------------

        //public


        //for rigged boxes
        private const float ExplosionRadius = 1f;
        private const float ExplosionDamage = 30;

        //private
        private const float CrackSpawnRadius = 1f;
        private const int MinNumCoins = 1;
        private const int MaxNumCoins = 8;
        private const float ProbOfPowerup = 1.2f;

        private bool _explosionRigged;
        private bool _cracked;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _explosionRigged = _cracked = false;
            new Timer(2.5f, CrackCrate);
        }

        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            if(c.GameObject.tag == ObjectTag.Player) {
                PlayerAttack pa = c.GameObject.GetComponent<PlayerAttack>();
                if (pa.IsAttacking)
                    CrackCrate();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CrackCrate() {
            _cracked = true;
            if (_explosionRigged) Explode();
            else SpawnValuables();

            ElementManager.Instance.Remove(this);
            GameObject.Destroy(GameObject);
        }

        void SpawnValuables() {
            int numCoins = MyRandom.Range(MinNumCoins, MaxNumCoins + 1);
            for (int i = 0; i < numCoins; i++) {
                Spawner.Instance.SpawnMoneyFromCenter(transform.position, CrackSpawnRadius);
            }

            if (MyRandom.Value <= ProbOfPowerup) {
                Spawner.Instance.SpawnPowerupFromCenter(transform.position, CrackSpawnRadius);
            }
        }

        void Explode() {
            //same code as in bomb
            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, ExplosionRadius);
            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IDamageable>()) {
                    c.GameObject.GetComponent<IDamageable>().TakeDamage(ExplosionDamage);
                }
            }
        }


        public void TakeDamage(float damage) { // TODO is it necessary to have both?
            if(!_cracked)CrackCrate();
        }

        public void SetExplosionRigged() { _explosionRigged = true; }


        // queries



        // other

    }

}