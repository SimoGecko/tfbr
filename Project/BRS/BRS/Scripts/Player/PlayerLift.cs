// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerLift : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private bool _lifting;
        private Vector3 _liftStartPos, _liftEndPos;

        // const
        const float LiftDistance = 1f;

        //Mesh forklift;
        //List<Mesh> separatedComponents;

        //reference
        PlayerInventory playerInventory;

        // --------------------- BASE METHODS ------------------
        public override void Start() { _lifting = false; }
        public override void Update() { }


        // --------------------- CUSTOM METHODS ----------------
        public void Lift() {
            _lifting = true;
            //liftStartPos = separatedComponents[0].transform;
            //liftEndPos = liftStartPos.y + liftDistance;

            //separatedComponents[0].transform = liftEndPos;
        }

        void separateMesh() {
            // Update separated components
        }

        void showCash() {

        }
        // queries
        public bool IsLifting { get { return _lifting; } }


        // other

    }
}