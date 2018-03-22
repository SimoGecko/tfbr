// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Bomb : Powerup {
        ////////// bomb that can be planted and explodes after some time damaging what's around //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float timeBeforeExplosion = 3f;
        const float explosionRadius = 2f;
        const float explosionDamage = 60;

        //private
        bool planted = false; // use to show sparks


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            destroyOnUse = false;
            powerupName = "bomb";
        }

        public override void Update() {
            base.Update();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            transform.position = owner.transform.position + Vector3.Up;
            canPickup = false;
            planted = true; 
            gameObject.Active = true;
            rotate = false;

            //Powerup pu = Spawner.instance.SpawnOnePowerupAt(p.transform.position, "bombPrefab");
            //Powerup pu = p.gameObject.GetComponent<PlayerPowerup>().DropBomb(posBomb);

            new Timer(timeBeforeExplosion, () => Explode());
        }

        void Explode() {
            //THIS CODE SHOULD NOT KNOW ANYTHING ABOUT THE SCENE -> THAT'S WHAT THE Idamageable interface is for
            /*
            GameObject[] bases = GameObject.FindGameObjectsWithTag("base");
            GameObject[] players = GameObject.FindGameObjectsWithTag("player");
            GameObject[] vautlDoor = GameObject.FindGameObjectsWithTag("VaultDoor");

            foreach (GameObject go in bases) {
                if (InExplosionRange(go)) go.GetComponent<Base>().TakeHit(damageExplosion);
            }

            foreach (GameObject go in players) {
                if ((go.transform.position - posBomb).LengthSquared() < radiusExplosion * radiusExplosion)
                    go.GetComponent<Player>().TakeHit(damageExplosion);
            }

            if (vautlDoor != null) {
                foreach (GameObject go in vautlDoor) {
                    if ((go.transform.position - posBomb).LengthSquared() < radiusExplosion * radiusExplosion) {
                        GameObject.Destroy(go);
                    }

                }
            }*/

            Collider[] overlapColliders = BRS.Physics.OverlapSphere(transform.position, explosionRadius);
            foreach(Collider c in overlapColliders) {
                if (c.gameObject.HasComponent<IDamageable>()) {
                    c.gameObject.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                }
            }

            //GameObject.Destroy(gameObject);
            //Spawner.instance.RemovePowerup(pu);
            GameObject.Destroy(gameObject);
        }

        // queries
        bool InExplosionRange(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= explosionRadius * explosionRadius;
        }



        // other

    }

}