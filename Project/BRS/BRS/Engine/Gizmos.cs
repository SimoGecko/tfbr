﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    static class Gizmos {
        ////////// this class allows to draw wireframe shapes (cubes, spheres) to debug game info //////////

        public static Color color = Color.Red;

        public static List<Transform> cubeOrder = new List<Transform>();
        public static List<Transform> sphereOrder = new List<Transform>();

        public static void DrawWireCube(Vector3 position, Vector3 size) {
            Transform t = new Transform(); t.position = position; t.scale = size;
            cubeOrder.Add(t);
        }

        public static void DrawWireSphere(Vector3 position, float radius) {
            Transform t = new Transform(); t.position = position; t.Scale(radius);
            sphereOrder.Add(t);
        }

        public static void DrawLine(Vector3 from, Vector3 to) {
            //TODO implement
        }

        public static void Draw(Camera cam) {
            //called at actual draw time
            foreach(Transform cube in cubeOrder) {
                Utility.DrawModel(Prefabs.cubeModel, cam.View, cam.Proj, cube.World);
            }
            foreach(Transform sphere in sphereOrder) {
                Utility.DrawModel(Prefabs.sphereModel, cam.View, cam.Proj, sphere.World);
            }
        }
        public static void ClearOrders() {
            cubeOrder.Clear();
            sphereOrder.Clear();
        }

    }

}