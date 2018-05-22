// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using System;

namespace BRS.Scripts.Elements {
    delegate bool Predicate();

    class Arrow : Component {
        ////////// shows an arrow that always points in the direction of the interest point //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float Margin = .6f;
        const float SmoothTime = .1f;

        //private
        private float _smoothAngle, _smoothRefAngle;
        private float _originalScale;
        bool _pointEnemy;
        int _playerIndex;

        //reference
        private readonly GameObject _follow; // attached player
        private Transform _target; // enemy player
        Predicate _condition;


        // --------------------- BASE METHODS ------------------
        public Arrow(GameObject follow, bool pointEnemy, int playerIndex, Predicate condition) {
            _follow = follow;
            _pointEnemy = pointEnemy;
            _playerIndex = playerIndex;
            _condition = condition;
        }

        public override void Start() {
            _originalScale = transform.scale.X;
            int teamIndex = _playerIndex % 2;
            if (_pointEnemy) {
                if (ElementManager.Instance.Enemy(teamIndex) != null)
                    _target = ElementManager.Instance.Enemy(teamIndex).transform;
                else
                    GameObject.Destroy(gameObject); // no enemy to point at -> destroy self
            } else {
                _target = ElementManager.Instance.Base(teamIndex).transform;
            }
        }

        public override void Update() {
            CheckIfDisplay();
            LookAtPoi();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CheckIfDisplay() {
            if (_condition()) {
                transform.scale = Vector3.One * _originalScale;
            } else {
                transform.scale = Vector3.Zero;
            }
        }

        void LookAtPoi() {
            transform.position = _follow.transform.position+Vector3.Up*.1f;
            Vector3 direction = _target.position - transform.position;
            float angle = MathHelper.ToDegrees((float) Math.Atan2(direction.Z, direction.X));
            _smoothAngle = Utility.SmoothDampAngle(_smoothAngle, angle, ref _smoothRefAngle, SmoothTime);
            transform.eulerAngles = new Vector3(0, -_smoothAngle, 0);
            transform.Translate(Vector3.Right * Margin);
        }

        

        // queries
        


        // other

    }
}