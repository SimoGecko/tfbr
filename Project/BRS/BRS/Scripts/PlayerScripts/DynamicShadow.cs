// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PlayerScripts {
    class DynamicShadow : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private

        // const

        //reference
        Transform _target;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _target = gameObject.transform;
        }

        public override void Update() { }


        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other

    }
}