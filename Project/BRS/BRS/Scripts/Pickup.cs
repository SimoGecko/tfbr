// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class Pickup : Component {
        ////////// generic class for object that can be picked up //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        //string interactabletag = "player";
        float pickupthreshold = .5f;

        //reference
        //Transform target;
        protected GameObject[] players;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            players = GameObject.FindGameObjectsWithTag("player");//.GetComponent<Player>();
            if (players == null) Debug.LogError("player not found");
            //target = player.transform;
        }

        public override void Update() {
            CheckPickup();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CheckPickup() {
            foreach(GameObject o in players) {
                float dist = Vector3.DistanceSquared(transform.position, o.transform.position);
                if (dist < pickupthreshold) {
                    OnPickup(o);
                }
            }
            
        }

        protected virtual void OnPickup(GameObject o) {
            //fill
        }


        // queries



        // other

    }

}