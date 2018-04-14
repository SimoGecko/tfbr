using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.Elements {
    /// <summary>
    /// Bomb that can be planted and explodes after some time damaging what's around
    /// </summary>
    class PlantedBomb : Component {

        private const float TimeBeforeExplosion = 4f;
        private const float ExplosionRadius = 4f;
        private const float ExplosionDamage = 60;

        public override void Start() {
            
        }

        public void Plant() {
            Audio.Play("bomb_timer", transform.position);
            new Timer(TimeBeforeExplosion, Explode);
        }

        void Explode() {
            Audio.Play("explosion", transform.position);
            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, ExplosionRadius);
            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IDamageable>()) {
                    c.GameObject.GetComponent<IDamageable>().TakeDamage(ExplosionDamage);
                }
            }
            ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Explosion);
            GameObject.Destroy(gameObject);
        }

        // queries
        bool InExplosionRange(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= ExplosionRadius * ExplosionRadius;
        }
    }
}