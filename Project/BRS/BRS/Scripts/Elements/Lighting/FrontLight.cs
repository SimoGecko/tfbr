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

        public enum Type { FrontAndBack, FrontOnly, BackOnly }

        //public

        //private
        private Type _type;
        private readonly Quaternion _orientationFront = Quaternion.CreateFromAxisAngle(Vector3.Right, -1 * (float)Math.PI / 2.0f);
        private readonly Quaternion _orientationBack = Quaternion.CreateFromAxisAngle(Vector3.Right, (float)Math.PI / 2.0f);
        private List<float> _alphaStart;
        private List<float> _min, _diff;
        private List<float> _speed;
        private List<Vector3> _offsetsFront, _offsetsBack;
        private Vector3 _lightSize = new Vector3(0.05f, 1.0f, 0.05f);
        private int _size;

        // const

        //reference

        // --------------------- BASE METHODS ------------------

        public FrontLight(Type type, Vector3 offsetFrontRight, Vector3 offsetBackLeft) {
            Init(type, offsetFrontRight, offsetBackLeft);
        }
        public FrontLight(Type type, int playerType) {
            switch (playerType) {
                default:
                    Init(type, new Vector3(0.30f, 0.41f, -0.38f), new Vector3(0.22f, 0.40f, 0.75f));
                    break;
                case 1:
                    Init(type, new Vector3(0.23f, 0.47f, -0.63f), new Vector3(0.23f, 0.47f, 0.72f));
                    break;
                case 2:
                    Init(type, new Vector3(0.20f, 0.67f, -0.11f), new Vector3(0.30f, 0.39f, 0.90f));
                    break;
            }
        }

        public void Init(Type type, Vector3 offsetFrontRight, Vector3 offsetBackLeft) {
            _offsetsFront = new List<Vector3>
            {
                new Vector3(offsetFrontRight.X, offsetFrontRight.Y, offsetFrontRight.Z + 0.001f),
                new Vector3(-offsetFrontRight.X, offsetFrontRight.Y, offsetFrontRight.Z)
            };
            _offsetsBack = new List<Vector3> {
                new Vector3(offsetBackLeft.X, offsetBackLeft.Y, offsetBackLeft.Z+0.001f),
                new Vector3(-offsetBackLeft.X, offsetBackLeft.Y, offsetBackLeft.Z)
            };

            _type = type;
            _size = type == Type.FrontAndBack ? 4 : 2;

            _alphaStart = new List<float>(_size);
            _min = new List<float>(_size);
            _diff = new List<float>(_size);
            _speed = new List<float>(_size);

            for (int i = 0; i < _size; ++i) {
                _alphaStart.Add(MyRandom.Range(0.0f, 2.0f * (float)Math.PI));
                _min.Add(MyRandom.Range(0.0f, 0.08f));
                _diff.Add(MyRandom.Range(0.3f, 0.4f) - _min[i]);
                _speed.Add(MyRandom.Range(5.0f, 7.5f));
            }
        }

        protected override List<Follower> CreateFollower() {
            Transform target = gameObject.transform;
            List<Follower> followers = new List<Follower>(4);

            if (_type == Type.FrontOnly || _type == Type.FrontAndBack) {
                for (int i = 0; i < 2; ++i) {
                    GameObject light = GameObject.Instantiate(FollowerType.LightYellow.GetDescription(), target);
                    light.transform.scale = _lightSize;
                    followers.Add(new Follower(light, _offsetsFront[i], _orientationFront, Follower.FollowingType.Orientated));
                }
            }

            if (_type == Type.BackOnly || _type == Type.FrontAndBack) {
                for (int i = 0; i < 2; ++i) {
                    GameObject light = GameObject.Instantiate(FollowerType.LightRed.GetDescription(), target);
                    light.transform.scale = _lightSize;
                    followers.Add(new Follower(light, _offsetsBack[i], _orientationBack, Follower.FollowingType.Orientated));
                }
            }

            return followers;
        }

        public override void Update() {
            for (int i = 0; i < _size; ++i) {
                float t = _alphaStart[i] + _speed[i] * (float)Time.Gt.TotalGameTime.TotalSeconds;

                Followers[i].GameObject.material.Alpha = MathHelper.Clamp(((float)Math.Sin(t) + 1) * (_diff[i]) / 2 + _min[i], 0.0f, 1.0f);
            }
        }

        // --------------------- CUSTOM METHODS ----------------


        // queries


        // other

    }
}