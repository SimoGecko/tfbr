// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.PowerUps;

namespace BRS.Scripts.Elements {
    class Key : Powerup {
        ////////// key that can be used to open the vault or safe boxes //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private const float TimeToUse = .3f;
        private const float OpenRadius = 10f;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Key;
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            transform.position = Owner.transform.position;
            new Timer(TimeToUse, OpenVault);
        }

        void OpenVault() {
            JRigidBody[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, OpenRadius);

            foreach (JRigidBody c in overlapColliders) {
                if (c.GameObject.HasComponent<IOpenable>()) {
                    c.GameObject.GetComponent<IOpenable>().Open();
                }
            }
        }

        // queries
        bool InOpeningRange(GameObject o) {
            transform.position = Owner.transform.position;
            return (o.transform.position - transform.position).LengthSquared() <= OpenRadius * OpenRadius;
        }

        bool ThereIsOneOpenableInRange() {
            transform.position = Owner.transform.position;
            JRigidBody[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, OpenRadius);

            foreach (JRigidBody c in overlapColliders) {
                if (c.GameObject.HasComponent<IOpenable>()) return true;
            }

            return false;
        }

        public override bool CanUse() {
            return ThereIsOneOpenableInRange();
        }



        // other

    }

}