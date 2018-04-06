// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;

namespace BRS.Scripts {

    // Todo: Probably belongs in the engine?
    public interface IOpenable : IComponent {
        void Open();
    }

    /// <summary>
    /// vault that can be exploded or opened and allows to enter in it
    /// </summary>
    class Vault : LivingEntity, IOpenable {
        // --------------------- VARIABLES ---------------------

        //public

        public static Rectangle VaultArea = new Rectangle(-9, -72, 19, 8);


        //private
        private const float OpeningDuration = 2f;
        private const float OpeningAnge = -90f;
        private const float PivotOffset = -1.5f;

        private bool _open; // at end of animation
        private bool _opening; // for animation
        private float _openRefTime;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Health = 10;
        }

        public override void Update() {
            if (_opening) OpenCoroutine();

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Open() {
            _opening = true;
            _openRefTime = 0;
        }

        protected override void Die() {
            Open();
            base.Die();
        }

        // queries
        Vector3 pivotPoint { get { return transform.position + transform.Right*PivotOffset; } }


        // other
        void OpenCoroutine() {
            if (_openRefTime <= 1) {
                float amount = Time.DeltaTime / OpeningDuration;
                _openRefTime += amount;
                //float t = Curve.EvaluateSqrt(openRefTime);
                transform.RotateAround(pivotPoint, Vector3.Up, amount*OpeningAnge);
            } else {
                _opening = false;
                _open = true;
            }
        }

    }

}