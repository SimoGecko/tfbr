// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class SpeedPad : Component {
        ////////// represents a speed pad that once passed over gives a quick speed boost //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float effectTime = 2f;


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            bool isplayer = c.gameObject.tag == ObjectTag.Player;
            if (isplayer) {
                PlayerMovement pM = c.gameObject.GetComponent<PlayerMovement>();
                pM.SetSpeedPad(true);
                new Timer(effectTime, () => pM.SetSpeedPad(false));
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}