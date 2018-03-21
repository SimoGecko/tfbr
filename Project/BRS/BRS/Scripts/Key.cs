// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
            powerupName = "key";
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
            Collider[] overlapColliders = Physics.OverlapSphere(transform.position, openRadius);
            foreach (Collider c in overlapColliders) {
                if (c.gameObject.HasComponent<IOpenable>()) {
                    c.gameObject.GetComponent<IOpenable>().Open();
                }
            }
        }

        // queries
        bool InOpeningRange(GameObject o) {
            transform.position = owner.transform.position;
            return (o.Transform.position - transform.position).LengthSquared() <= openRadius * openRadius;
        }

        bool ThereIsOneOpenableInRange() {
            transform.position = owner.transform.position;
            Collider[] overlapColliders = Physics.OverlapSphere(transform.position, openRadius);
            foreach (Collider c in overlapColliders) {
                if (c.gameObject.HasComponent<IOpenable>()) return true;
            }
            return false;
        }

        public override bool CanUse() {
            return ThereIsOneOpenableInRange();
        }



        // other

    }

}