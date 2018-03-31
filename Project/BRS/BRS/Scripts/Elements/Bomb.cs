﻿// (c) Simone Guggiari 2018
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
            powerupType = PowerupType.bomb;
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
            gameObject.active = true;
            rotate = false;

            //Powerup pu = Spawner.instance.SpawnOnePowerupAt(p.transform.position, "bombPrefab");
            //Powerup pu = p.gameObject.GetComponent<PlayerPowerup>().DropBomb(posBomb);

            new Timer(timeBeforeExplosion, () => Explode());
        }

        void Explode() {
            Collider[] overlapColliders = BRS.Physics.OverlapSphere(transform.position, explosionRadius);
            foreach(Collider c in overlapColliders) {
                if (c.gameObject.HasComponent<IDamageable>()) {
                    c.gameObject.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                }
            }
            GameObject.Destroy(gameObject);
        }

        // queries
        bool InExplosionRange(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= explosionRadius * explosionRadius;
        }



        // other

    }

}