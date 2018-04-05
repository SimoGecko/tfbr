// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BRS.Scripts {
    class Magnet : Powerup {
        ////////// powerup that spawns a magnet //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            powerupType = PowerupType.magnet;
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void UsePowerup() {
            base.UsePowerup();
            transform.position = owner.transform.position + Vector3.Up;
            //instantiate magnet slowing down stuff
            GameObject plantedMagnet = GameObject.Instantiate("plantedMagnetPrefab", transform);
        }

        // queries


        // other

    }

    class PlantedMagnet : Component {
        ////////// magnet that slows down everything within its radius //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float actionRadius = 3f;
        const float startDelay = 2f;
        const float duration = 60f;



        //private
        List<PlayerMovement> pMAffected;
        bool active = false;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            active = false;
            pMAffected = new List<PlayerMovement>();
            Invoke(startDelay, () => active = true);
            GameObject.Destroy(gameObject, duration);
        }

        public override void Update() {
            if(active)
                CheckSlowdownRadius();
        }




        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CheckSlowdownRadius() {
            foreach (var pm in pMAffected) pm.SetSlowdown(false);
            pMAffected.Clear();

            foreach(Player p in Elements.instance.Players()) {
                if (InActionRadius(p.gameObject)){
                    PlayerMovement pM = p.gameObject.GetComponent<PlayerMovement>();
                    pM.SetSlowdown(true);
                    pMAffected.Add(pM);
                }
            }
        }


        // queries
        bool InActionRadius(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= actionRadius * actionRadius;
        }


        // other

    }

}