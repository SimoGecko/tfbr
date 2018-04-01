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
        const float smoothTime = .1f;


        //private
        int playerIndex;
        float smoothAngle, smoothRefAngle;
        public Vector3 teamBase;
        Vector3 point; // in case the point is fixed
        Color arrowColor;

        //reference
        GameObject follow; // attached player
        PlayerInventory pI;
        Transform target; // enemy player


        // --------------------- BASE METHODS ------------------
        public Arrow(GameObject _follow, Transform _target, int _index) {
            target = _target; follow = _follow; playerIndex = _index;
        }

        public override void Start() {
            point = Vector3.Zero;
            pI = follow.GetComponent<PlayerInventory>();
            target = Elements.instance.Player(1 - playerIndex).transform;
            teamBase = Elements.instance.Base(playerIndex).transform.position;
        }

        public override void Update() {
            LookAtPoi();
            //TODO apply arrow color
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void LookAtPoi() {
            transform.position = follow.transform.position;
            Vector3 direction = Poi() - transform.position;
            float angle = MathHelper.ToDegrees((float) System.Math.Atan2(direction.Z, direction.X));
            smoothAngle = Utility.SmoothDampAngle(smoothAngle, angle, ref smoothRefAngle, smoothTime);
            transform.eulerAngles = new Vector3(0, -smoothAngle, 0);
            transform.Translate(Vector3.Right * margin);
        }



        // queries
        Vector3 Poi() {
            if (pI.IsFull()) {
                arrowColor = Color.Green;
                return teamBase;
            }
            arrowColor = Color.Red;
            return target != null ? target.position : point;
        }


        // other

    }
}