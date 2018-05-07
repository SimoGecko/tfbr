// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Scripts.Elements.Lighting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BRS.Scripts.Elements {
    /// <summary>
    /// Animates the rotation of the wheels for a driving car (only y-axis-rotations)
    /// </summary>
    class AnimatedWheels : FollowerComponent {

        // --------------------- VARIABLES ---------------------
        public enum Type { FrontAndBack, FrontOnly, BackOnly }

        private enum Offset { FrontLeft, FrontRight, BackLeft, BackRight }

        //public


        //private
        private readonly Type _type;
        //private Matrix[] _meshPartTransforms;
        private readonly Vector3[] _wheelOffsets;
        private readonly int[] _wheelMeshPartIndex;
        private readonly List<Offset> _toUpdate;
        private List<Quaternion> _localRotations;
        private readonly int _factor;
        private readonly string _model;

        //reference


        // --------------------- BASE METHODS ------------------
        public AnimatedWheels(Type type, int factor, int playerType) {
            _type = type;
            _factor = factor;
            _model = playerType == 2 ? "wheelType2" : "wheelType1";

            _toUpdate = new List<Offset>();

            if (_type == Type.FrontOnly || _type == Type.FrontAndBack) {
                _toUpdate.Add(Offset.FrontLeft);
                _toUpdate.Add(Offset.FrontRight);
            }

            if (_type == Type.BackOnly || _type == Type.FrontAndBack) {
                _toUpdate.Add(Offset.BackLeft);
                _toUpdate.Add(Offset.BackRight);
            }

            _wheelMeshPartIndex = new int[4];
            _wheelOffsets = new Vector3[4];


            switch (playerType) {
                default: // forklift
                    _model = "wheelType1";
                    _wheelOffsets[(int)Offset.FrontLeft] = new Vector3(-0.3149025f, 0.1552318f + 0.1f, -0.3100869f);
                    _wheelOffsets[(int)Offset.FrontRight] = new Vector3(0.3149025f, 0.1552318f + 0.1f, -0.3100869f);
                    _wheelOffsets[(int)Offset.BackLeft] = new Vector3(-0.2811967f, 0.1572949f + 0.1f, 0.5519256f);
                    _wheelOffsets[(int)Offset.BackRight] = new Vector3(0.2811967f, 0.1572949f + 0.1f, 0.5519256f);
                    break;

                case 1: // sweeper
                    _model = "wheelType1";
                    _wheelOffsets[(int)Offset.FrontLeft] = new Vector3(-0.3388945f, 0.155463f + 0.1f, -0.02759502f);
                    _wheelOffsets[(int)Offset.FrontRight] = new Vector3(0.3612739f, 0.1896283f + 0.1f, -0.04256354f);
                    _wheelOffsets[(int)Offset.BackLeft] = new Vector3(-0.2638865f, 0.1711256f + 0.1f, 0.529377f);
                    _wheelOffsets[(int)Offset.BackRight] = new Vector3(0.2802619f, 0.1896285f + 0.1f, 0.4977417f);
                    break;

                case 2: // bulldozer
                    _model = "wheelType2";
                    _wheelOffsets[(int)Offset.FrontLeft] = new Vector3(-0.3703101f, 0.250674f + 0.1f, 0.002451748f);
                    _wheelOffsets[(int)Offset.FrontRight] = new Vector3(0.3590385f, 0.2506741f + 0.1f, -0.00906501f);
                    _wheelOffsets[(int)Offset.BackLeft] = new Vector3(-0.362828f, 0.2506741f + 0.1f, 0.5842964f);
                    _wheelOffsets[(int)Offset.BackRight] = new Vector3(0.359039f, 0.2506743f + 0.1f, 0.58277f);
                    break;

                case 3: // police
                    _model = "wheelType1";
                    _model = "wheelPolice";
                    _wheelOffsets[(int)Offset.FrontLeft] = new Vector3(-0.4113479f, 0.1297145f, -0.6306446f);
                    _wheelOffsets[(int)Offset.FrontRight] = new Vector3(0.4113479f, 0.1297147f, -0.6306445f);
                    _wheelOffsets[(int)Offset.BackLeft] = new Vector3(-0.4162008f, 0.1297147f, 0.587538f);
                    _wheelOffsets[(int)Offset.BackRight] = new Vector3(0.4162008f, 0.1297147f, 0.587538f);
                    break;
            }


        }


        //public override void Start() {

        //    //    Model current = gameObject.Model;

        //    //    List<ModelBone> bones = new List<ModelBone>();

        //    //    foreach (var tmp in current.Bones) {
        //    //        ModelBone newBone = new ModelBone();
        //    //        newBone.Transform = tmp.Transform;
        //    //        newBone.Name = tmp.Name;
        //    //        newBone.Index = tmp.Index;
        //    //        newBone.ModelTransform = tmp.ModelTransform;

        //    //        foreach (var ms in tmp.Meshes) {
        //    //            newBone.AddMesh(ms);
        //    //        }
        //    //    }

        //    //    gameObject.Model = new Model(Graphics.gD, bones, current.Meshes.ToList());

        //    //if (gameObject.HasComponent<MovingRigidBody>()) {
        //    //    _collider = gameObject.GetComponent<MovingRigidBody>().RigidBody;
        //    //}
        //    //if (gameObject.HasComponent<AnimatedRigidBody>()) {
        //    //    _collider = gameObject.GetComponent<AnimatedRigidBody>().RigidBody;
        //    //}
        //    //if (gameObject.HasComponent<DynamicRigidBody>()) {
        //    //    _collider = gameObject.GetComponent<DynamicRigidBody>().RigidBody;
        //    //}

        //    _meshPartTransforms = new Matrix[gameObject.Model.Bones.Count];
        //    gameObject.Model.CopyBoneTransformsTo(_meshPartTransforms);

        //    for (int i = 0; i < gameObject.Model.Bones.Count; ++i) {
        //        var bone = gameObject.Model.Bones[i];

        //        Vector3 offset = bone.Meshes.Count > 0 ? bone.Meshes[0].BoundingSphere.Center : Vector3.Zero;

        //        switch (bone.Name) {
        //            case "wheel_fl":
        //                _wheelMeshPartIndex[(int)Offset.FrontLeft] = i;
        //                _wheelOffsets[(int)Offset.FrontLeft] = offset;
        //                break;
        //            case "wheel_fr":
        //                _wheelMeshPartIndex[(int)Offset.FrontRight] = i;
        //                _wheelOffsets[(int)Offset.FrontRight] = offset;
        //                break;
        //            case "wheel_bl":
        //                _wheelMeshPartIndex[(int)Offset.BackLeft] = i;
        //                _wheelOffsets[(int)Offset.BackLeft] = offset;
        //                break;
        //            case "wheel_br":
        //                _wheelMeshPartIndex[(int)Offset.BackRight] = i;
        //                _wheelOffsets[(int)Offset.BackRight] = offset;
        //                break;
        //        }
        //    }
        //}

        protected override List<Follower> CreateFollower() {
            Transform target = gameObject.transform;
            List<Follower> followers = new List<Follower>();
            _localRotations = new List<Quaternion>();
            int index = 0;

            if (_type == Type.FrontOnly || _type == Type.FrontAndBack) {
                GameObject wheelFrontLeft = GameObject.Instantiate(_model, target);
                followers.Add(new Follower(wheelFrontLeft, _wheelOffsets[(int)Offset.FrontLeft], Quaternion.Identity,
                    Follower.FollowingType.Orientated));
                _wheelMeshPartIndex[(int)Offset.FrontLeft] = index++;

                GameObject wheelFrontRight = GameObject.Instantiate(_model, target);
                followers.Add(new Follower(wheelFrontRight, _wheelOffsets[(int)Offset.FrontRight], Quaternion.Identity,
                    Follower.FollowingType.Orientated));
                _wheelMeshPartIndex[(int)Offset.FrontRight] = index++;

                _localRotations.Add(Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI));
                _localRotations.Add(Quaternion.Identity);
            }

            if (_type == Type.BackOnly || _type == Type.FrontAndBack) {
                GameObject wheelBackLeft = GameObject.Instantiate(_model, target);
                followers.Add(new Follower(wheelBackLeft, _wheelOffsets[(int)Offset.BackLeft], Quaternion.Identity,
                    Follower.FollowingType.Orientated));
                _wheelMeshPartIndex[(int)Offset.BackLeft] = index++;

                GameObject wheelBackRight = GameObject.Instantiate(_model, target);
                followers.Add(new Follower(wheelBackRight, _wheelOffsets[(int)Offset.BackRight], Quaternion.Identity,
                    Follower.FollowingType.Orientated));
                _wheelMeshPartIndex[(int)Offset.BackRight] = index++;

                if (_model == "wheelPolice") {
                    _localRotations.Add(Quaternion.Identity);
                    _localRotations.Add(Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI));
                } else {
                    _localRotations.Add(Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI));
                    _localRotations.Add(Quaternion.Identity);
                }
            }

            return followers;
        }

        public override void Update() {
            foreach (Offset offset in _toUpdate) {
                int index = _wheelMeshPartIndex[(int)offset];
                Follower follower = Followers[index];
                Quaternion local = _localRotations[index];
                int factor = (int)offset < 2 ? _factor : -_factor;
                follower.Orientation = local *
                    Quaternion.CreateFromAxisAngle(Vector3.Up, factor * MathHelper.ToRadians(Collider.LastRotation));
            }
            if (_model == "wheelPolice") {
                Debug.Log(Collider.LastRotation);
            }
            //foreach (Offset offset in _toUpdate) {
            //    int i = (int)offset;
            //    int index = _wheelMeshPartIndex[i];

            //    Matrix toLocal = Matrix.CreateTranslation(_wheelOffsets[i]) * transform.World;
            //    Matrix t = Matrix.Invert(toLocal);
            //    int factor = i < 2 ? _factor : -_factor;
            //    _meshPartTransforms[index] = t * Matrix.CreateRotationY(factor * MathHelper.ToRadians(_collider.LastRotation)) * toLocal;
            //    //_meshPartTransforms[index] = t * Matrix.CreateRotationY(factor * MathHelper.ToRadians((float)Time.Gt.TotalGameTime.TotalSeconds)) * toLocal;
            //    Debug.Log(_collider.LastRotation);
            //}

            //gameObject.Model.CopyBoneTransformsFrom(_meshPartTransforms);
        }


        // --------------------- CUSTOM METHODS ----------------

        // commands

        // queries

        // other

    }
}