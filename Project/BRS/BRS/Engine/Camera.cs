// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS {
    public class Camera {
        //class that represents a virtual camera in space with projection characteristics
        public static Camera main;
        static List<Camera> camList = new List<Camera>();

        public enum Projection { Orthographic, Perspective};
        public Projection projectiontype = Projection.Perspective;
        public float fov = 60; // degrees
        public float near = 0.3f;
        public float far = 1000f;
        float aspectRatio = 1.33333333f; //1280/720 or 1920/1080 =1.7778, 1200/900 = 1.3333

        public Viewport viewport;

        public Transform transform = new Transform();

        public Matrix View {
            get { return Matrix.Invert(transform.World); }
        }
        public Matrix Proj; // precomputed

        public Camera(Viewport vp, float _fov = 60)  {
            if (main == null) main = this;
            camList.Add(this);
            viewport = vp;
            fov = _fov;
            aspectRatio = vp.AspectRatio;
            MakeProjMatrix(projectiontype);
        }


        void MakeProjMatrix(Projection projectionType) {
            if (projectiontype == Projection.Perspective) {
                Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, near, far);
            } else {
                Proj = Matrix.CreateOrthographic(20, 20 / aspectRatio, near, far);
            }
        }

        public void Start() {
            //default position and rotation
            transform.position = new Vector3(0, 10, 7);
            transform.eulerAngles = new Vector3(-45, 0, 0);
        }

        public void Update() { }

        //METHODS
        public Ray ScreenPointToRay(Vector2 point) {
            Vector3 nearPoint = viewport.Unproject(new Vector3(point.X, point.Y, 0.0f), Proj, View, Matrix.Identity);
            Vector3 farPoint  = viewport.Unproject(new Vector3(point.X, point.Y, 1.0f), Proj, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        public static Camera GetCamera(int i) {
            return camList[i];
        }
    }
}
