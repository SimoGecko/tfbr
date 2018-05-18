// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BRS.Scripts.Elements.Lighting {
    /// <summary>
    /// Initialize the dynamic shadow which will be synchronised with the position of the game-object
    /// </summary>
    class AlarmLight : FollowerComponent {

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private readonly FollowerType _leftType;
        private readonly Vector3 _leftOffset;

        private readonly FollowerType _rightType;
        private readonly Vector3 _rightOffset;

        private const float Speed = 10.0f;
        private readonly Vector3 _lightSize = new Vector3(0.1f, 1.0f, 0.1f);
        private readonly float[] _alphaStart;

        // const

        //reference

        // --------------------- BASE METHODS ------------------

        public AlarmLight(FollowerType leftType, Vector3 leftOffset, FollowerType rightType, Vector3 rightOffset) {
            _leftType = leftType;
            _leftOffset = leftOffset;
            _rightType = rightType;
            _rightOffset = rightOffset;

            float randomStart = MyRandom.Range(0.0f, (float)Math.PI);
            _alphaStart = new[] { randomStart, randomStart + (float)Math.PI };
        }

        protected override List<Follower> CreateFollower() {
            Transform target = gameObject.transform;

            GameObject leftLight = GameObject.Instantiate(_leftType.GetDescription(), target);
            leftLight.transform.scale = _lightSize;

            GameObject rightLight = GameObject.Instantiate(_rightType.GetDescription(), target);
            rightLight.transform.scale = _lightSize;

            return new List<Follower> {
                new Follower(leftLight, _leftOffset, Quaternion.Identity, Follower.FollowingType.Orientated),
                new Follower(rightLight, _rightOffset, Quaternion.Identity, Follower.FollowingType.Orientated)
            };
        }

        public override void Update() {
            for (int i = 0; i < Followers.Count; ++i) {
                float t = _alphaStart[i] + Speed * (float)Time.Gt.TotalGameTime.TotalSeconds;
                Followers[i].GameObject.Alpha = MathHelper.Clamp((float)Math.Sin(t), 0.0f, 1.0f);
            }
        }

        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other

    }
}