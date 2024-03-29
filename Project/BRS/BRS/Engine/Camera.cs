﻿// (c) Simone Guggiari 2018
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
        public const float Near = 0.3f;
        public const float Far = 1000f;
        public const float FarDepth = 100f;
        const float defaultFOV = 40;
        public const float FocusDistance = 14;

        //private
        Projection projectiontype = Projection.Perspective;

        readonly float _fov; // degrees
        readonly float _aspectRatio;
        //readonly int _index;

        public Viewport Viewport { get; }

        public Matrix Proj; // precomputed
        public Matrix ProjDepth; // precomputed
        public Matrix View { get { return Matrix.Invert(transform.World); } }
        //public int Index { get { return _index; } }

        public Camera(Viewport vp, float fov = defaultFOV)  {
            //_index = index;
            _fov = fov;
            _aspectRatio = vp.AspectRatio;
            Viewport = vp;
            ComputeProj(projectiontype);
        }


        void ComputeProj(Projection projectionType) {
            if (projectionType == Projection.Perspective) {
                Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), _aspectRatio, Near, Far);
                ProjDepth = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), _aspectRatio, Near, FarDepth);
            } else {
                Proj = Matrix.CreateOrthographic(20, 20 / _aspectRatio, Near, Far);
                ProjDepth = Matrix.CreateOrthographic(20, 20 / _aspectRatio, Near, FarDepth);
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
            bool isInRightHalf = Vector3.Dot(world - transform.position, transform.Forward) > 0;
            if (!isInRightHalf) return Vector2.One * -1000;
            return new Vector2((int)Math.Round(result.X), (int)Math.Round(result.Y)) - Viewport.Bounds.Location.ToVector2();
        }

        /// <summary>
        /// Returns the pixel-location of an object in the range of [0,1]x[0,1] for the shader-processing
        /// </summary>
        /// <param name="world">Coordinate of the position in 3D-space</param>
        /// <returns>Location on the screen (full screen) within the range of [0,1]x[0,1]</returns>
        public Vector2 WorldToScreenPoint01(Vector3 world) {
            Vector3 result = Viewport.Project(world, Proj, View, Matrix.Identity);
            Vector2 resolution = new Vector2((int)Math.Round(result.X), (int)Math.Round(result.Y));
            return new Vector2(resolution.X / Screen.Width, resolution.Y / Screen.Height);
        }


        //static METHODS
        public static Camera Main { get { return Screen.Cameras[0]; } }
        public static Camera GetCamera(int i) {  return Screen.Cameras[i]; }
    }
}
