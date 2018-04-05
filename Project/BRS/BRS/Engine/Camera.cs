// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS {
    public class Camera : Component {
        ////////// class that represents a virtual camera in space with projection characteristics //////////
        public enum Projection { Orthographic, Perspective};

        //public
        public static Camera main;
        static List<Camera> camList = new List<Camera>();

        //private
        Projection projectiontype = Projection.Perspective;
        float fov = 60; // degrees
        const float near = 0.3f;
        const float far = 1000f;
        float aspectRatio = 1.33333333f; //1280/720 or 1920/1080 =1.7778, 1200/900 = 1.3333

        public Viewport viewport { get; }

        public Matrix Proj; // precomputed
        public Matrix View {
            get { return Matrix.Invert(transform.World); }
        }

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

        public override void Start() { }
        public override void Update() { }

        //static METHODS
        public Ray ScreenPointToRay(Vector2 point) {
            Vector3 nearPoint = viewport.Unproject(new Vector3(point.X, point.Y, 0.0f), Proj, View, Matrix.Identity);
            Vector3 farPoint  = viewport.Unproject(new Vector3(point.X, point.Y, 1.0f), Proj, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        public Vector2 WorldToScreenPoint(Vector3 world) {
            Vector3 result = viewport.Project(world, Proj, View, Matrix.Identity);
            return new Vector2((int)System.Math.Round(result.X), (int)System.Math.Round(result.Y));
            /* // DUMB me that cannot have this snippet of code work
            Vector4 worldH = new Vector4(world, 1);
            Vector4 view = Vector4.Transform(worldH, View);
            Vector4 result = Vector4.Transform(view, Proj);
            return new Vector2(result.X/result.W, result.Y / result.W);*/
        }

        public static Camera GetCamera(int i) {
            return camList[i];
        }
    }
}
