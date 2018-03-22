// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {

    public interface IOpenable : IComponent {
        void Open();
    }

    class Vault : LivingEntity, IOpenable {
        ////////// vault that can be exploded or opened and allows to enter in it //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float openingDuration = 2f;
        const float openingAnge = -90f;
        //static Vector3 pivotOffset = new Vector3(-1.5f, 0, 0);
        const float pivotOffset = -1.5f;

        //private
        bool open = false; // at end of animation
        bool opening = false; // for animation
        float openRefTime;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            health = 10;
        }

        public override void Update() {
            if (opening) OpenCoroutine();

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Open() {
            opening = true;
            openRefTime = 0;
        }

        protected override void Die() {
            Open();
            base.Die();
        }

        // queries
        Vector3 pivotPoint { get { return transform.position + transform.Right*pivotOffset; } }


        // other
        void OpenCoroutine() {
            if (openRefTime <= 1) {
                float amount = Time.deltatime / openingDuration;
                openRefTime += amount;
                //float t = Curve.EvaluateSqrt(openRefTime);
                transform.RotateAround(pivotPoint, Vector3.Up, amount*openingAnge);
            } else {
                opening = false;
                open = true;
            }
        }

    }

}