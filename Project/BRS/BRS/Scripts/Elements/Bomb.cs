// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Bomb : Powerup {
        ////////// bomb that can be planted and explodes after some time damaging what's around //////////

        // --------------------- VARIABLES ---------------------

        //public
        

        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            powerupType = PowerupType.bomb;
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            transform.position = owner.transform.position + Vector3.Up;
            GameObject plantedBomb = GameObject.Instantiate("plantedBombPrefab", transform);
        }
    }

    class PlantedBomb : Component {
        ////////// bomb that can be planted and explodes after some time damaging what's around //////////

        const float timeBeforeExplosion = 3f;
        const float explosionRadius = 4f;
        const float explosionDamage = 60;

        public override void Start() {
            new Timer(timeBeforeExplosion, () => Explode());
        }

        void Explode() {
            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, explosionRadius);
            foreach(Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IDamageable>()) {
                    c.GameObject.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                }
            }
            GameObject.Destroy(gameObject);
        }

        // queries
        bool InExplosionRange(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= explosionRadius * explosionRadius;
        }
    }

    

}