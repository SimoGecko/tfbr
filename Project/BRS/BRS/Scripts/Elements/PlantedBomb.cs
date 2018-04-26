using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.PostProcessing;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

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
            for (float i = 0; i < TimeBeforeExplosion; i += .5f) {
                new Timer(i, () => ParticleUI.Instance.GiveOrder(FusePosition(), ParticleType.Sparks));
            }
            new Timer(TimeBeforeExplosion, Explode);
        }

        void Explode() {
            Audio.Play("bomb_explosion", transform.position);
            ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Explosion);

            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, ExplosionRadius);
            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IDamageable>()) {
                    c.GameObject.GetComponent<IDamageable>().TakeDamage(ExplosionDamage);
                }
            }

            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                PostProcessingManager.Instance.ActivateShockWave(i, transform.position);
            }

            GameObject.Destroy(gameObject);
        }

        // queries
        bool InExplosionRange(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= ExplosionRadius * ExplosionRadius;
        }

        Vector3 FusePosition() {
            return transform.position + Vector3.Up * .5f;
        }

    }
}