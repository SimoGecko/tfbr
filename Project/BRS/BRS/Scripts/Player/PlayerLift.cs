// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerLift : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float liftDistance = 1f;

        //private
        bool lifting;
        Vector3 liftStartPos, liftEndPos;
        //Mesh forklift;
        //List<Mesh> separatedComponents;

        //reference
        PlayerInventory playerInventory;

        // --------------------- BASE METHODS ------------------
        public override void Start() {lifting = false; }
        public override void Update() { }


        // --------------------- CUSTOM METHODS ----------------
        public void Lift() {
            lifting = true;
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
        public bool IsLifting { get { return lifting; } }


        // other
        
    }
}