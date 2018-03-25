// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Arrow : Component {
        ////////// shows an arrow that always points in the direction of the interest point //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float margin = .4f;
        public int playerIndex;

        //private
        Vector3 point; // in case the point is fixed

        //reference
        Transform target;
        Transform follow;


        // --------------------- BASE METHODS ------------------
        public Arrow(Transform _follow, Transform _target, int _index) {
            target = _target; follow = _follow; playerIndex = _index;
        }

        public override void Start() {
            point = Vector3.Zero;
            target = GameObject.FindGameObjectWithName("player_"+(1-playerIndex)).transform;
        }

        public override void Update() {
            LookAtPoi();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void LookAtPoi() {
            transform.position = follow.position;
            Vector3 direction = poi - transform.position;
            float angle = MathHelper.ToDegrees((float) System.Math.Atan2(direction.Z, direction.X));
            transform.eulerAngles = new Vector3(0, -angle, 0);
            transform.Translate(Vector3.Right * margin);
        }



        // queries
        Vector3 poi { get { return target != null ? target.position : point; } }


        // other

    }
}