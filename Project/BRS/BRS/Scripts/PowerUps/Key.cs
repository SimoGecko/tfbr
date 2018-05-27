// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Rendering;
using BRS.Scripts.PowerUps;

namespace BRS.Scripts.Elements {
    class Key : Powerup {
        ////////// key that can be used to open the vault or safe boxes //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private const float TimeToUseDelay = .3f;
        private const float OpenRadius = 10f;

        //reference
        public Key() {
            powerupType = PowerupType.Key;
            ParticleRay = ParticleType3D.PowerUpKeyRay;
            ParticleStar = ParticleType3D.PowerUpKeyStar;
        }


        // --------------------- BASE METHODS ------------------

        // --------------------- CUSTOM METHODS ----------------

        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            new Timer(TimeToUseDelay, TryOpen);
        }

        void TryOpen() {
            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, OpenRadius);
            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IOpenable>()) {
                    c.GameObject.GetComponent<IOpenable>().Open();
                }
            }
        }

        // queries
        /*
        bool InOpeningRange(GameObject o) {
            transform.position = Owner.transform.position;
            return (o.transform.position - transform.position).LengthSquared() <= OpenRadius * OpenRadius;
        }

        bool ThereIsOneOpenableInRange() {
            transform.position = Owner.transform.position;
            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, OpenRadius);

            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IOpenable>()) return true;
            }

            return false;
        }

        public override bool CanUse() {
            return ThereIsOneOpenableInRange();
        }
        */



        // other

    }

}