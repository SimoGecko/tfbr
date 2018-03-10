// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    //!!! MOST IMPORTANT CLASS
    // Class that represents a position, rotation and scale in 3D space (possibly relatieve to a parent)

    //[Serializable]
    //it doesn't necessarily have a gameobject!

    public class Transform {
        // INFO
        // right-handed system: X=right, Y=up, -Z=forward

        // local space members
        Vector3 m_position; 
        Quaternion m_rotation;
        Vector3 m_scale;

        Transform parent;
        public bool draw = false;
        public bool isstatic = false; // Do optimization
        //bool dirty -> recompute only when necessary


        //constructors
        public Transform() { Reset(); alltransforms.Add(this); }
        /*
        public Transform(Vector3 pos) { Reset(); position = pos; }
        public Transform(float x, float y, float z) { Reset(); position = new Vector3(x, y, z); }
        public Transform(Vector3 pos, Vector3 rot) { Reset(); position = pos; eulerAngles = rot; }
        public Transform(Vector3 pos, Vector3 rot, Vector3 sc) { Reset(); position = pos; eulerAngles = rot; scale = sc; }
        public Transform(Transform t) { Reset(); CopyTransform(t); }
        */

        public Matrix World {
            get {// SUPER IMPORTANT, CHECK!!!
                Matrix parentM = Matrix.Identity;
                if (parent != null) parentM = parent.World;
                return Matrix.CreateScale(m_scale) * rotmatrix * Matrix.CreateTranslation(m_position) * parentM; // order is super important - this appears correct (although not sure why) why not S, T, R?
                //return Matrix.CreateWorld(position, Forward, Up); // maybe cheaper
            }
        }

        
        //BASIS CHANGE
        public Vector3 toWorld(Vector3 v) { return Vector3.Transform(v, parentMatrix); }
        public Vector3 toLocal(Vector3 v) { return Vector3.Transform(v, Matrix.Invert(parentMatrix)); }
        public Vector3 toLocalRotation(Vector3 v) {
            v = Vector3.Transform(v, Matrix.Invert(parentMatrix));
            return Vector3.Transform(v, rotmatrix);
        }

        //MATRIX
        Matrix parentMatrix { get { return parent == null ? Matrix.Identity : parent.World; } }
        Matrix rotmatrix    { get { return Matrix.CreateFromQuaternion(m_rotation); } }
        // Matrix rotmatrix { get { return Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(m_rotation.Y), MathHelper.ToRadians(m_rotation.X), MathHelper.ToRadians(m_rotation.Z)); } }

        //local coords
        public Vector3 Right   { get { return toLocalRotation(Vector3.Right  ); } }//+X
        public Vector3 Up      { get { return toLocalRotation(Vector3.Up     ); } }//+Y
        public Vector3 Forward { get { return toLocalRotation(Vector3.Forward); } }//-Z

        /*
        public Vector3 eulerAngles { // in degrees
            get {
                return rotation.toEulerAngle();
            }
            set {
                float yaw   = MathHelper.ToRadians(-value.Y);//YXZ // - because of conjugate?
                float pitch = MathHelper.ToRadians(-value.X);
                float roll  = MathHelper.ToRadians( value.Z);
                rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
                //rotation.Conjugate();
            }
        }*/


        //ACCESSORS
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
            //get { return m_rotation; } // TODO
            set { m_rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.X), MathHelper.ToRadians(value.Z)); }
        }

        //COMMANDS
        public void Reset() { m_position = Vector3.Zero; m_rotation = Quaternion.Identity; m_scale = Vector3.One; }
        public void CopyFrom(Transform t) { m_position = t.m_position; m_rotation = t.m_rotation; m_scale = t.m_scale; }

        public void Translate(Vector3 v) { m_position += toLocalRotation( v); } // local space
        public void TranslateGlobal(Vector3 v) { m_position +=  v; } // global space
        public void Rotate(Vector3 axis, float angle) {
            m_rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(angle)), m_rotation);
        }
        public void RotateAround(Vector3 point, Vector3 axis, float angle) {
            //TODO implement;

            //update position
            Rotate(axis, angle);
        }

        public void Scale(Vector3 s) { m_scale += s; }
        public void Scale(float s)   { m_scale *= s; }

        public void SetParent(Transform t) { parent = t; }
        public void LookAt(Vector3 p) { // this caused the use of quaternions
            Matrix look = Matrix.CreateLookAt(m_position, p, Up);
            //eulerAngles = look.toEulerAngles();
            m_rotation = look.Rotation;
            m_rotation.Conjugate();
        }


        //STATIC
        public static List<Transform> alltransforms = new List<Transform>();
        public static void Draw(Camera cam) {
            foreach(Transform t in alltransforms) {
                if (t.draw) {
                    Utility.DrawModel(Prefabs.emptymodel, cam.View, cam.Proj, t.World);
                }
            }
        }
    }
}
