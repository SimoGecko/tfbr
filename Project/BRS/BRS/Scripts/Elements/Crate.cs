// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.PostProcessing;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private const float ProbOfPowerup = .2f;

        private bool _explosionRigged;
        private bool _cracked;
        private int _teamWhoRigged;

        //reference
        static Texture2D dangerIcon;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _explosionRigged = _cracked = false;
            if (dangerIcon == null) dangerIcon = File.Load<Texture2D>("Images/UI/explodingboxicon");
        }

        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            if(c.GameObject.tag == ObjectTag.Player) {
                PlayerAttack pa = c.GameObject.GetComponent<PlayerAttack>();
                if (pa.IsAttacking && !_cracked)
                    CrackCrate();
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CrackCrate() {
            _cracked = true;
            Audio.Play("crate_cracking", transform.position);
            ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Drill, 1.5f);
            if (_explosionRigged) Explode();
            else SpawnValuables();

            ElementManager.Instance.Remove(this);
            GameObject.Destroy(gameObject);
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
            Audio.Play("bomb_explosion", transform.position);
            ParticleUI.Instance.GiveOrder(transform.position, ParticleType.FireExplosion, 1.8f);

            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, ExplosionRadius);
            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IDamageable>()) {
                    c.GameObject.GetComponent<IDamageable>().TakeDamage(ExplosionDamage);
                }
            }

            PostProcessingManager.Instance.ActivateShockWave(transform.position);
        }


        public void TakeDamage(float damage) { // TODO is it necessary to have both -> yes as bombs could crack it
            if(!_cracked)
                CrackCrate();
        }

        public void SetExplosionRigged(int teamWhoRigged) { _explosionRigged = true; _teamWhoRigged = teamWhoRigged; }


        // queries

        public override void Draw2D(int i) {
            if (i == -1) return;
            if (!_explosionRigged) return;
            if(GameManager.TeamIndex(i) == _teamWhoRigged) {
                //draw danger icon
                float distScale = Camera.FocusDistance/(Camera.GetCamera(i).transform.position - transform.position).Length();
                Vector2 screenPos = Camera.GetCamera(i).WorldToScreenPoint(transform.position);
                UserInterface.DrawPicture(dangerIcon, screenPos, pivot:Align.Center, scale: .3f*distScale);
            }
        }



        // other

    }

}