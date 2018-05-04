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
    class FrontLight : FollowerComponent {

        // --------------------- VARIABLES ---------------------


        //public

        //private
        private readonly List<float> _alphaStart;
        private readonly List<float> _min, _diff;
        private readonly List<float> _speed;
        private readonly List<Vector3> _offsets;
        private readonly Vector3 _lightSize = new Vector3(0.05f, 1.0f, 0.05f);

        // const

        //reference

        // --------------------- BASE METHODS ------------------

        public FrontLight(Vector3 offsetFrontRight, Vector3 offsetBackLeft) {
            _offsets = new List<Vector3> {
                new Vector3(offsetFrontRight.X, offsetFrontRight.Y, offsetFrontRight.Z+0.001f),
                new Vector3(-offsetFrontRight.X, offsetFrontRight.Y, offsetFrontRight.Z),
                new Vector3(offsetBackLeft.X, offsetBackLeft.Y, offsetBackLeft.Z+0.001f),
                new Vector3(-offsetBackLeft.X, offsetBackLeft.Y, offsetBackLeft.Z)
            };


            _alphaStart = new List<float>(4);
            _min = new List<float>(4);
            _diff = new List<float>(4);
            _speed = new List<float>(4);

            for (int i = 0; i < 4; ++i) {
                _alphaStart.Add(MyRandom.Range(0.0f, 2.0f*(float)Math.PI));
                _min.Add(MyRandom.Range(0.0f, 0.08f));
                _diff.Add(MyRandom.Range(0.5f, 0.7f) - _min[i]);
                _speed.Add(MyRandom.Range(50.0f, 75.0f));
            }
        }

        protected override List<Follower> CreateFollower() {
            Transform target = gameObject.transform;
            List<Follower> followers = new List<Follower>(4);

            for (int i = 0; i < 4; ++i) {
                Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Right, (i < 2 ? -1 : 1) * (float)Math.PI / 2.0f);
                GameObject light = GameObject.Instantiate(FollowerType.LightYellow.GetDescription(), target);
                light.transform.scale = _lightSize;
                followers.Add(new Follower(light, _offsets[i], orientation, Follower.FollowingType.Orientated));
            }

            return followers;
        }

        public override void Update() {
            for (int i = 0; i < 4; ++i) {
                float t = _alphaStart[i] + _speed[i] * (float)Time.Gt.TotalGameTime.TotalSeconds;

                Followers[i].GameObject.material.Alpha = MathHelper.Clamp(((float)Math.Sin(t) + 1) * (_diff[i]) / 2 + _min[i], 0.0f, 1.0f);
            }
        }

        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other

    }
}