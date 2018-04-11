// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    ////////// Class that represents a virtual camera in space with projection characteristics //////////
    public class Camera : Component {
        public enum Projection { Orthographic, Perspective};

        //public
        public static Camera Main;
        static readonly List<Camera> Cameras = new List<Camera>();

        //private
        Projection projectiontype = Projection.Perspective;

        private readonly float _fov; // degrees
        readonly float _aspectRatio;
        private const float Near = 0.3f;
        private const float Far = 1000f;

        public Viewport Viewport { get; }

        public Matrix Proj; // precomputed
        public Matrix View {
            get { return Matrix.Invert(transform.World); }
        }

        public Camera(Viewport vp, float fov = 60)  {
            if (Main == null) Main = this;

            _fov = fov;
            _aspectRatio = vp.AspectRatio;
            Viewport = vp;

            Cameras.Add(this);
            MakeProjMatrix(projectiontype);
        }


        void MakeProjMatrix(Projection projectionType) {
            if (projectionType == Projection.Perspective) {
                Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), _aspectRatio, Near, Far);
            } else {
                Proj = Matrix.CreateOrthographic(20, 20 / _aspectRatio, Near, Far);
            }
        }

        //static METHODS
        public Ray ScreenPointToRay(Vector2 point) {
            Vector3 nearPoint = Viewport.Unproject(new Vector3(point.X, point.Y, 0.0f), Proj, View, Matrix.Identity);
            Vector3 farPoint  = Viewport.Unproject(new Vector3(point.X, point.Y, 1.0f), Proj, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        public Vector2 WorldToScreenPoint(Vector3 world) {
            Vector3 result = Viewport.Project(world, Proj, View, Matrix.Identity);
            return new Vector2((int)Math.Round(result.X), (int)Math.Round(result.Y)) - Viewport.Bounds.Location.ToVector2();
            /* // DUMB me that cannot have this snippet of code work
            Vector4 worldH = new Vector4(world, 1);
            Vector4 view = Vector4.Transform(worldH, View);
            Vector4 result = Vector4.Transform(view, Proj);
            return new Vector2(result.X/result.W, result.Y / result.W);*/
        }

        public static Camera GetCamera(int i) {
            return Cameras[i];
        }
    }
}
