// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Arrow : Component {
        ////////// shows an arrow that always points in the direction of the interest point //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float Margin = .4f;
        const float SmoothTime = .1f;


        //private
        private readonly int _playerIndex;
        private float _smoothAngle, _smoothRefAngle;
        private Vector3 _teamBase;
        private Vector3 _point; // in case the point is fixed
        Color _arrowColor;

        //reference
        private readonly GameObject _follow; // attached player
        private PlayerInventory _pI;
        private Transform _target; // enemy player


        // --------------------- BASE METHODS ------------------
        public Arrow(GameObject follow, Transform target, int index) {
            _target = target;
            _follow = follow;
            _playerIndex = index;
        }

        public override void Start() {
            _point = Vector3.Zero;
            _pI = _follow.GetComponent<PlayerInventory>();

            if(Elements.Instance.Player(1 - _playerIndex)!=null)
                _target = Elements.Instance.Player(1 - _playerIndex).transform;

            _teamBase = Elements.Instance.Base(_playerIndex%2).transform.position;
        }

        public override void Update() {
            LookAtPoi();
            //TODO apply arrow color
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void LookAtPoi() {
            transform.position = _follow.transform.position;
            Vector3 direction = Poi() - transform.position;
            float angle = MathHelper.ToDegrees((float) Math.Atan2(direction.Z, direction.X));
            _smoothAngle = Utility.SmoothDampAngle(_smoothAngle, angle, ref _smoothRefAngle, SmoothTime);
            transform.eulerAngles = new Vector3(0, -_smoothAngle, 0);
            transform.Translate(Vector3.Right * Margin);
        }



        // queries
        Vector3 Poi() {
            if (_pI.IsFull()) {
                _arrowColor = Color.Green;
                return _teamBase;
            }

            _arrowColor = Color.Red;
            return _target != null ? _target.position : _point;
        }


        // other

    }
}