// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Elements {
    delegate bool Predicate();

    class Arrow : Component {
        ////////// shows an arrow that always points in the direction of the interest point //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float Margin = .4f;
        const float SmoothTime = .1f;
        //const float smallSize = .05f;
        //const float bigSize = .2f;


        //private
        private float _smoothAngle, _smoothRefAngle;
        private float scale;
        bool _followEnemy;
        int _playerIndex;

        //reference
        private readonly GameObject _follow; // attached player
        private Transform _target; // enemy player
        Predicate _condition;


        // --------------------- BASE METHODS ------------------
        public Arrow(GameObject follow, bool followEnemy, int playerIndex, Predicate condition) {
            _follow = follow;
            _followEnemy = followEnemy;
            _playerIndex = playerIndex;
            _condition = condition;
        }

        public override void Start() {
            scale = transform.scale.X;
            int teamIndex = _playerIndex % 2;
            if (_followEnemy) {
                if (ElementManager.Instance.Enemy(teamIndex) != null)
                    _target = ElementManager.Instance.Enemy(teamIndex).transform;
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
        void LookAtPoi() {
            transform.position = _follow.transform.position+Vector3.Up*.05f;
            Vector3 direction = _target.position - transform.position;
            float angle = MathHelper.ToDegrees((float) Math.Atan2(direction.Z, direction.X));
            _smoothAngle = Utility.SmoothDampAngle(_smoothAngle, angle, ref _smoothRefAngle, SmoothTime);
            transform.eulerAngles = new Vector3(0, -_smoothAngle, 0);
            transform.Translate(Vector3.Right * Margin);
        }

        void CheckIfDisplay() {
            if (_condition()) {
                transform.scale = Vector3.One * scale;
            } else {
                transform.scale = Vector3.Zero;
            }
        }

        // queries
        


        // other

    }
}