// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    ////////// This class allows to draw wireframe shapes (cubes, spheres) to debug game info. //////////
    static class Gizmos {

        private static readonly List<Transform> CubeOrder = new List<Transform>();
        private static readonly List<Transform> SphereOrder = new List<Transform>();
        private static readonly List<Transform> TransformOrder = new List<Transform>();


        public static void DrawWireCube(Vector3 position, Vector3 size) {
            Transform t = new Transform { position = position, scale = size };
            CubeOrder.Add(t);
        }

        public static void DrawWireSphere(Vector3 position, float radius) {
            Transform t = new Transform { position = position, scale = Vector3.One*radius };
            SphereOrder.Add(t);
        }

        public static void DrawLine(Vector3 from, Vector3 to) {
            //TODO implement
        }

        public static void DrawTransform(Transform transform, float scale) {
            Transform t = new Transform();
            t.CopyFrom(transform);
            t.Scale(scale);
            TransformOrder.Add(t);
        }

        public static void DrawWire(Camera cam) {
            //called at actual draw time
            foreach (Transform cube in CubeOrder) {
                Graphics.DrawModel(Prefabs.CubeModel, cam.View, cam.Proj, cube.World);
            }
            CubeOrder.Clear();
            foreach (Transform sphere in SphereOrder) {
                Graphics.DrawModel(Prefabs.SphereModel, cam.View, cam.Proj, sphere.World);
            }
            SphereOrder.Clear();
        }

        public static void DrawFull(Camera cam) {
            foreach (Transform trans in TransformOrder) {
                Graphics.DrawModel(Prefabs.Emptymodel, cam.View, cam.Proj, trans.World);
            }
            TransformOrder.Clear();
        }
    }

}