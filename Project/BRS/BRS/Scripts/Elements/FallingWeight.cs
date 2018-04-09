using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Elements {
    /// <summary>
    /// weight that falls on top of the enemy player 
    /// </summary>
    class FallingWeight : Component {
        // --------------------- VARIABLES ---------------------

        //public

        //private
        private const float FallSpeed = 15f;
        private const int FallDamage = 30;
        private const float Lifetime = 3f;

        private bool _invokedDelete;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }
        public override void Update() {
            if (transform.position.Y > 0) {
                transform.position += Vector3.Down * FallSpeed * Time.DeltaTime;
            } else if (!_invokedDelete) {
                _invokedDelete = true;
                GameObject.Destroy(gameObject, Lifetime);
            }
        }

        // --------------------- CUSTOM METHODS ----------------
        public override void OnCollisionEnter(Collider c) {
            if (_invokedDelete) return;
            if (c.GameObject.HasComponent<IDamageable>()) {
                c.GameObject.GetComponent<IDamageable>().TakeDamage(FallDamage);
            }
        }

        // commands



        // queries



        // other
    }
}