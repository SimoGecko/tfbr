// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Elements {

    // Todo: Probably belongs in the engine? -> no as it's something particular
    public interface IOpenable : IComponent {
        void Open();
    }

    /// <summary>
    /// vault that can be exploded or opened and allows to enter in it
    /// </summary>
    class Vault : LivingEntity, IOpenable {
        // --------------------- VARIABLES ---------------------

        //public

        //public static Rectangle VaultArea = new Rectangle(-9, -72, 19, 8);


        //private
        private const float OpeningDuration = 2f;
        private const float OpeningAnge = -90f;
        private const float ClosenessForMessage = 10f;
        private const float TimeBetweenMessages = 20f;

        private bool _open; // at end of animation
        private bool _opening; // during animation
        private float _openRefTime;

        public System.Action OnVaultOpen;

        public System.Action OnVaultClosedCloseEnough;
        public static int OnClosedCloseIndex;
        float[] timeForNextClosenessCall;


        //reference
        public static Vault instance;


        // --------------------- BASE METHODS ------------------
        public override void Awake() {
            instance = this;
        }

        public override void Start() {
            base.Start();
            Health = 10;

            _open = _opening = false;
            timeForNextClosenessCall = new float[GameManager.NumPlayers];
        }

        public override void Update() {
            if (_opening) OpenCoroutine();
            if(!_open)CheckForClosePlayer();
        }

        public override void Reset() {
            Start();
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CheckForClosePlayer() {
            for(int i=0; i<GameManager.NumPlayers; i++) {
                if (Time.CurrentTime > timeForNextClosenessCall[i]) {
                    if ((ElementManager.Instance.Player(i).transform.position-transform.position).LengthSquared()<= ClosenessForMessage* ClosenessForMessage) {
                        OnClosedCloseIndex = i;
                        OnVaultClosedCloseEnough?.Invoke();
                        timeForNextClosenessCall[i] = Time.CurrentTime + TimeBetweenMessages;
                    }
                }
            }
        }

        public void Open() {
            if (_open) {
                return;
            }
            Audio.Play("vault_opening", transform.position);
            OnVaultOpen?.Invoke();
            _opening = true;
            _openRefTime = 0;

            for(int i=0; i<GameManager.NumPlayers; i++) {
                Input.Vibrate(.03f, OpeningDuration, i);
            }
        }

        protected override void Die() {
            Open();
            base.Die();
        }

        // queries
        Vector3 pivotPoint { get { return transform.position; } }
        public bool IsOpen() { return _open; }

        // other
        void OpenCoroutine() {
            if (_openRefTime <= 1) {
                float amount = Time.DeltaTime / OpeningDuration;
                _openRefTime += amount;
                //float t = Curve.EvaluateSqrt(openRefTime);
                transform.RotateAround(pivotPoint, Vector3.Up, amount * OpeningAnge);
            } else {
                _opening = false;
                _open = true;
            }
        }



    }

}