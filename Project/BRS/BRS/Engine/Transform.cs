// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Utilities;
using Microsoft.Xna.Framework;

namespace BRS.Engine {
    ////////// Class that represents a position, rotation and scale in 3D space (possibly relatieve to a parent) //////////
    /// <summary>
    /// (it doesn't necessarily have a gameobject)
    /// INFO: right-handed system: X=right, Y=up, -Z=forward
    /// 
    /// TODO test and complete functions
    /// </summary>
    public class Transform {
        // ---------- MEMBERS ----------
        // local space
        Vector3 _position; 
        Quaternion _rotation;
        Vector3 _scale;

        Transform _parent;
        public bool _isStatic; // Do optimization
        Matrix WorldCached;
        //bool dirty -> recompute only when necessary
        


        // ---------- CONSTRUCTORS ----------
        public Transform(Vector3 p, Quaternion r, Vector3 s) {
            _position = p;
            _rotation = r;
            _scale = s;
            _parent = null;
            _isStatic = false;
            //alltransforms.Add(this);
        }

        public Transform() : this(Vector3.Zero, Quaternion.Identity, Vector3.One) { }
        public Transform(Vector3 pos) : this(pos, Quaternion.Identity, Vector3.One) { }
        public Transform(float x, float y, float z) : this(new Vector3(x, y, z)) { }
        public Transform(Vector3 pos, Vector3 rot) : this(pos) { eulerAngles = rot; }
        public Transform(Vector3 pos, Vector3 rot, Vector3 sc) : this(pos, Quaternion.Identity, sc) { eulerAngles = rot; }
        public Transform(Transform t) : this(t._position, t._rotation, t._scale) { }


        // ---------- MATRIX ----------
        public Matrix World { // SUPER IMPORTANT, CHECK!!!
            get {
                if (_isStatic) return WorldCached;
                return Matrix.CreateScale(_scale) * rotmatrix * Matrix.CreateTranslation(_position) * parentMatrix; // order is super important - this appears correct (although not sure why). -> why not S, T, R?
                //DIFFERENCE BETWEEN VECTORS AND POINTS!!
                //return Matrix.CreateWorld(position, Forward, Up); // maybe cheaper
            }
        }

        Matrix parentMatrix { get { return _parent == null ? Matrix.Identity : _parent.World; } }
        Matrix rotmatrix { get { return Matrix.CreateFromQuaternion(_rotation); } }
        // Matrix rotmatrix { get { return Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(m_rotation.Y), MathHelper.ToRadians(m_rotation.X), MathHelper.ToRadians(m_rotation.Z)); } }

        // ---------- BASIS CHANGE ----------
        //TODO: take into account difference between point and vector (and direction?) and incorporate rotation and scale 
        public Vector3 toWorld(Vector3 v) { return Vector3.Transform(v, parentMatrix); }
        public Vector3 toLocal(Vector3 v) { return Vector3.Transform(v, Matrix.Invert(parentMatrix)); }

        public Vector3 toLocalRotation(Vector3 v) { // THIS is probably unnecessary
            v = Vector3.Transform(v, Matrix.Invert(parentMatrix));
            return Vector3.Transform(v, rotmatrix);
        }

        //TODO must take into consideration parent transform aswell?
        public Vector3 Right   { get { return toLocalRotation(Vector3.Right); } }  //+X
        public Vector3 Up      { get { return toLocalRotation(Vector3.Up); } }     //+Y
        public Vector3 Forward { get { return toLocalRotation(Vector3.Forward); } }//-Z



        // ---------- ACCESSORS ----------
        //local
        public Vector3 localPosition {
            get { return _position; }
            set { _position = value; }
        }
        public Quaternion localRotation {
            get { return _rotation; }
            set { _rotation = value; }
        }
        public Vector3 localScale {
            get { return _scale; }
            set { _scale = value; }
        }

        //global
        public Vector3 position {
            get { return toWorld(_position); }
            set { _position = toLocal(value); }
        }

        public Quaternion rotation { // TODO make world
            get { return _rotation; }
            set { _rotation = value; }
        }
        public Vector3 scale {
            get { return toWorld(_scale); }
            set { _scale = toLocal(value); }
        }

        public Vector3 eulerAngles {
            get { return _rotation.ToEuler(); }
            set { _rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.X), MathHelper.ToRadians(value.Z)); }
        }


        // ---------- COMMANDS ----------

        public void Reset() { _position = Vector3.Zero; _rotation = Quaternion.Identity; _scale = Vector3.One; }
        public void CopyFrom(Transform t) { _position = t._position; _rotation = t._rotation; _scale = t._scale; }

        //translate
        public void Translate(Vector3 v) { _position += toLocalRotation(v); } // local space
        public void TranslateGlobal(Vector3 v) { _position += v; } // global space

        //rotate
        public void Rotate(Vector3 axis, float angle) {
            _rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(angle)), _rotation);
        }
        public void RotateAround(Vector3 point, Vector3 axis, float angle) {
            Vector3 delta = _position - point;
            Quaternion rot = Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(angle));
            Vector3 result = Vector3.Transform(delta, rot);
            _position = point + result;

            Rotate(axis, angle);
        }
        public void LookAt(Vector3 p) { // this caused the use of quaternions
            Matrix look = Matrix.CreateLookAt(_position, p, Vector3.Up);
            //eulerAngles = look.toEulerAngles();
            _rotation = look.Rotation;
            _rotation.Conjugate();
        }

        //scale
        public void Scale(Vector3 s) { _scale += s; }
        public void Scale(float s) { _scale *= s; }



        // ---------- OTHER ----------
        public void SetParent(Transform t) { _parent = t; }

        public void SetStatic(bool b = true) {
            WorldCached = World;
            _isStatic = b;
        }

        static Transform _identity;
        public static Transform Identity {
            get {
                if (_identity == null) _identity = new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One); // lazy
                return _identity;
            }
        }

        //STATIC
        //public static List<Transform> alltransforms = new List<Transform>();
    }
}
