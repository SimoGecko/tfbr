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
        private const float TimeBeforeProximityCheck = 1;
        private const float ExplosionRadius = 7f; // also proximity explosion
        private const float ExplosionDamage = 60;
        const bool doProximityCheck = false;

        int teamIndex;

        bool exploded;
        bool checkProximity;

        public override void Start() {
            exploded = false;
            checkProximity = false;
        }

        public override void Update() {
            base.Update();
            if(checkProximity)
                CheckProximity();
        }

        public void Plant(int teamIndex) {
            exploded = false;
            Audio.Play("bomb_timer", transform.position);
            for (float i = 0; i < TimeBeforeExplosion; i += .5f) {
                new Timer(i, () => ParticleUI.Instance.GiveOrder(FusePosition(), ParticleType.Sparks));
            }
            if(doProximityCheck)
                new Timer(TimeBeforeProximityCheck, () => checkProximity = true);
            new Timer(TimeBeforeExplosion, Explode);
        }

        void CheckProximity() {
            foreach(var p in ElementManager.Instance.Players()) {
                if (p.TeamIndex != teamIndex && InExplosionRange(p.gameObject))
                    Explode();
            }
        }

        void Explode() {
            if (exploded) return;
            exploded = true;
            Audio.Play("bomb_explosion", transform.position);
            ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Explosion, 1.5f);

            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, ExplosionRadius);
            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IDamageable>()) {
                    c.GameObject.GetComponent<IDamageable>().TakeDamage(ExplosionDamage);
                }
            }

            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                PostProcessingManager.Instance.ActivateShockWave(i, transform.position);
            }
            ElementManager.Instance.Remove(this.gameObject);
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