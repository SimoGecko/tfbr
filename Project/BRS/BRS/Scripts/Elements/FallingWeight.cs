using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.Elements {
    /// <summary>
    /// weight that falls on top of the enemy player 
    /// </summary>
    class FallingWeight : Component {
        // --------------------- VARIABLES ---------------------

        //public

        //private
        private const int FallDamage = 30;
        private const float Lifetime = 3f;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Audio.Play("falling_weight", transform.position);

        }
        public override void Update() {
        }

        // --------------------- CUSTOM METHODS ----------------
        public override void OnCollisionEnter(Collider c) {
            // As soon as the weight collided with any static-element (most likely the ground) it's going to be destroyed.
            if (c.IsStatic) {
                new Timer(Lifetime, () => RemoveWeight());
            }

            // If the weight collided with something which can be damaged => damage it
            if (c.GameObject.HasComponent<IDamageable>()) {
                c.GameObject.GetComponent<IDamageable>().TakeDamage(FallDamage);
            }
        }

        // commands
        void RemoveWeight() {
            ElementManager.Instance.Remove(this.gameObject);
            GameObject.Destroy(gameObject, Lifetime);

        }


        // queries



        // other
    }
}