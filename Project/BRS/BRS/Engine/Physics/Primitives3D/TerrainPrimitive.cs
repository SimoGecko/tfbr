// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB
// Copied from the Jitter-Physics-Demo

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.Primitives3D {
    public class TerrainPrimitive : GeometricPrimitive {
        public delegate float TerrainFunction(int coordX, int coordZ);

        private readonly float[,] _heights;

        public TerrainPrimitive(GraphicsDevice device, TerrainFunction function) {
            _heights = new float[100, 100];

            for (int i = 0; i < 100; i++) {
                for (int e = 0; e < 100; e++) {
                    _heights[i, e] = function(i, e);
                }
            }

            Vector3[] neighbour = new Vector3[4];

            for (int i = 0; i < 100; i++) {
                for (int e = 0; e < 100; e++) {
                    Vector3 pos = new Vector3(i, _heights[i, e], e);

                    if (i > 0) neighbour[0] = new Vector3(i - 1, _heights[i - 1, e], e);
                    else neighbour[0] = pos;

                    if (e > 0) neighbour[1] = new Vector3(i, _heights[i, e - 1], e - 1);
                    else neighbour[1] = pos;

                    if (i < 99) neighbour[2] = new Vector3(i + 1, _heights[i + 1, e], e);
                    else neighbour[2] = pos;

                    if (e < 99) neighbour[3] = new Vector3(i, _heights[i, e + 1], e + 1);
                    else neighbour[3] = pos;

                    Vector3 normal = Vector3.Zero;

                    normal += Vector3.Cross(neighbour[1] - pos, neighbour[0] - pos);
                    normal += Vector3.Cross(neighbour[2] - pos, neighbour[1] - pos);
                    normal += Vector3.Cross(neighbour[3] - pos, neighbour[2] - pos);
                    normal += Vector3.Cross(neighbour[0] - pos, neighbour[3] - pos);
                    normal.Normalize();

                    AddVertex(new Vector3(i, _heights[i, e], e), normal);
                }
            }

            for (int i = 1; i < 100; i++) {
                for (int e = 1; e < 100; e++) {
                    AddIndex((i - 1) * 100 + e);
                    AddIndex(i * 100 + (e - 1));
                    AddIndex(i * 100 + e);


                    AddIndex(i * 100 + (e - 1));
                    AddIndex((i - 1) * 100 + e);
                    AddIndex((i - 1) * 100 + (e - 1));

                }
            }

            InitializePrimitive(device);
        }

    }
}
