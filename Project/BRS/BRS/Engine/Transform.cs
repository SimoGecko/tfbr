// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    //!!! VERY IMPORTANT CLASS
    ////////// Class that represents a position, rotation and scale in 3D space (possibly relatieve to a parent) //////////

    //TODO test and complete functions

    //(it doesn't necessarily have a gameobject)
    // INFO
    // right-handed system: X=right, Y=up, -Z=forward

    public class Transform {
        // local space members
        Vector3 m_position; 
        Quaternion m_rotation;
        Vector3 m_scale;

        Transform parent;
        public bool isStatic; // Do optimization
        Matrix WorldCached;
        //bool dirty -> recompute only when necessary


        //constructors
        public Transform(Vector3 p, Quaternion r, Vector3 s) {
            m_position = p;
            m_rotation = r;
            m_scale = s;
            parent = null;
            isStatic = false;
            alltransforms.Add(this);
        }

        public Transform() : this(Vector3.Zero, Quaternion.Identity, Vector3.One) { }
        public Transform(Vector3 pos) : this(pos, Quaternion.Identity, Vector3.One) { }
        public Transform(float x, float y, float z) : this(new Vector3(x, y, z)) { }
        public Transform(Vector3 pos, Vector3 rot) : this(pos) { eulerAngles = rot; }
        public Transform(Vector3 pos, Vector3 rot, Vector3 sc) : this(pos, Quaternion.Identity, sc) { eulerAngles = rot;}
        public Transform(Transform t) : this(t.m_position, t.m_rotation, t.m_scale) { }

        public Matrix World {
            get {// SUPER IMPORTANT, CHECK!!!
                if (isStatic) return WorldCached;
                return Matrix.CreateScale(m_scale) * rotmatrix * Matrix.CreateTranslation(m_position) * parentMatrix; // order is super important - this appears correct (although not sure why) why not S, T, R?
                //DIFFERENCE BETWEEN VECTORS AND POINTS!!
                //return Matrix.CreateWorld(position, Forward, Up); // maybe cheaper
            }
        }

        
        //BASIS CHANGE // incorporate rotation and scale 
        //TAKE into account difference between point and vector (and direction?)
        public Vector3 toWorld(Vector3 v) { return Vector3.Transform(v, parentMatrix); }
        public Vector3 toLocal(Vector3 v) { return Vector3.Transform(v, Matrix.Invert(parentMatrix)); }

        public Vector3 toLocalRotation(Vector3 v) { // THIS is probably unnecessary
            v = Vector3.Transform(v, Matrix.Invert(parentMatrix));
            return Vector3.Transform(v, rotmatrix);
        }

        //MATRIX
        Matrix parentMatrix { get { return parent == null ? Matrix.Identity : parent.World; } }
        Matrix rotmatrix    { get { return Matrix.CreateFromQuaternion(m_rotation); } }
        // Matrix rotmatrix { get { return Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(m_rotation.Y), MathHelper.ToRadians(m_rotation.X), MathHelper.ToRadians(m_rotation.Z)); } }

        //local coords
        //TODO must take into consideration parent transform aswell?
        public Vector3 Right   { get { return toLocalRotation(Vector3.Right  ); } }//+X
        public Vector3 Up      { get { return toLocalRotation(Vector3.Up     ); } }//+Y
        public Vector3 Forward { get { return toLocalRotation(Vector3.Forward); } }//-Z

        //ACCESSORS
        //local
        public Vector3 localPosition {
            get { return m_position; }
            set { m_position = value; }
        }
        public Quaternion localRotation {
            get { return m_rotation; }
            set { m_rotation = value; }
        }
        public Vector3 localScale {
            get { return m_scale; }
            set { m_scale = value; }
        }

        //global
        public Vector3 position {
            get { return toWorld(m_position); }
            set { m_position = toLocal(value); }
        }
        
        public Quaternion rotation { // TODO make world
            get { return  m_rotation; }
            set { m_rotation = value; }
        }
        public Vector3 scale {
            get { return toWorld(m_scale); }
            set { m_scale = toLocal(value); }
        }

        public Vector3 eulerAngles {
            get { return m_rotation.ToEuler(); }
            set { m_rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.X), MathHelper.ToRadians(value.Z)); }
        }

        //COMMANDS (change members)
        public void Reset() { m_position = Vector3.Zero; m_rotation = Quaternion.Identity; m_scale = Vector3.One; }
        public void CopyFrom(Transform t) { m_position = t.m_position; m_rotation = t.m_rotation; m_scale = t.m_scale; }

        //translate
        public void Translate(Vector3 v) { m_position += toLocalRotation( v); } // local space
        public void TranslateGlobal(Vector3 v) { m_position +=  v; } // global space

        //rotate
        public void Rotate(Vector3 axis, float angle) {
            m_rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(angle)), m_rotation);
        }
        public void RotateAround(Vector3 point, Vector3 axis, float angle) {
            Vector3 delta = m_position - point;
            Quaternion rot = Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(angle));
            Vector3 result = Vector3.Transform(delta, rot);
            m_position = point + result;

            Rotate(axis, angle);
        }
        public void LookAt(Vector3 p) { // this caused the use of quaternions
            Matrix look = Matrix.CreateLookAt(m_position, p, Vector3.Up);
            //eulerAngles = look.toEulerAngles();
            m_rotation = look.Rotation;
            m_rotation.Conjugate();
        }

        //scale
        public void Scale(Vector3 s) { m_scale += s; }
        public void Scale(float s)   { m_scale *= s; }

        //other
        public void SetParent(Transform t) { parent = t; }

        public void SetStatic() {
            WorldCached = World;
            isStatic = true;
        }

        //STATIC
        public static List<Transform> alltransforms = new List<Transform>();
    }
}
