// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Trap : Powerup {
        ////////// powerup that can be dropped to slow down enemy //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            powerupName = "trap";

        }

        public override void Update() {
            base.Update();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            //transform.position = owner.transform.position + Vector3.Up;
            //instantiate oil trap
            GameObject.Instantiate("oilPrefab", owner.transform.position, MyRandom.YRotation());
            Debug.Log("instant");
        }



        // queries



        // other

    }

    class OilTrap : Component {
        ////////// oiltrap that slows down enemy when collided with //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float slowdownTime = 3f;
        const float timeToEffect = 1f;

        //private
        bool inUse = false;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            inUse = false;
            new Timer(timeToEffect, () => inUse = true);
        }

        public override void Update() {
        }

        public override void OnCollisionEnter(Collider c) {
            bool isplayer = c.gameObject.tag == ObjectTag.Player;
            if (isplayer && inUse) {
                PlayerMovement pM = c.gameObject.GetComponent<PlayerMovement>();
                pM.SetSlowdown(true);
                new Timer(slowdownTime, () => pM.SetSlowdown(false));
                GameObject.Destroy(gameObject);
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands


        // queries

        // other
    }
}