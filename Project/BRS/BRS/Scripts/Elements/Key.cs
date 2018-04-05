// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;

namespace BRS.Scripts {
    class Key : Powerup {
        ////////// key that can be used to open the vault or safe boxes //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float timeToUse = .3f;
        const float openRadius = 10f;

        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            powerupType = PowerupType.key;
        }

        public override void Update() {
            base.Update();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            transform.position = owner.transform.position;
            new Timer(timeToUse, () => OpenVault());
        }

        void OpenVault() {
            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, openRadius);
            foreach (Collider c in overlapColliders) {
                if (c.GameObject.HasComponent<IOpenable>()) {
                    c.GameObject.GetComponent<IOpenable>().Open();
                }
            }
        }

        // queries
        bool InOpeningRange(GameObject o) {
            transform.position = owner.transform.position;
            return (o.transform.position - transform.position).LengthSquared() <= openRadius * openRadius;
        }

        bool ThereIsOneOpenableInRange() {
            transform.position = owner.transform.position;
            Collider[] overlapColliders = PhysicsManager.OverlapSphere(transform.position, openRadius);
            foreach (Collider c in overlapColliders) {
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