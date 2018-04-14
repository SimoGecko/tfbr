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
        private const float Near = 0.3f;
        private const float Far = 1000f;

        //private
        Projection projectiontype = Projection.Perspective;

        readonly float _fov; // degrees
        readonly float _aspectRatio;
        //readonly int _index;

        public Viewport Viewport { get; }

        public Matrix Proj; // precomputed
        public Matrix View { get { return Matrix.Invert(transform.World); } }
        //public int Index { get { return _index; } }

        public Camera(Viewport vp, float fov = 60)  {
            //_index = index;
            _fov = fov;
            _aspectRatio = vp.AspectRatio;
            Viewport = vp;
            ComputeProj(projectiontype);
        }


        void ComputeProj(Projection projectionType) {
            if (projectionType == Projection.Perspective) {
                Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), _aspectRatio, Near, Far);
            } else {
                Proj = Matrix.CreateOrthographic(20, 20 / _aspectRatio, Near, Far);
            }
        }

        //conversion methods
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
        }

        //static METHODS
        public static Camera Main { get { return Screen.Cameras[0]; } }
        public static Camera GetCamera(int i) {  return Screen.Cameras[i]; }
    }
}
