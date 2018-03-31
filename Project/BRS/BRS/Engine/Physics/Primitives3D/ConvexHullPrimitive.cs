using System;
using System.Collections.Generic;
using Jitter.LinearMath;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.Primitives3D {
    public class ConvexHullPrimitive : GeometricPrimitive {

        //public JConvexHull ConvexHull = new JConvexHull();

        public ConvexHullPrimitive(GraphicsDevice device, List<JVector> pointCloud, List<Tuple<int, int, int>> indices) {
            //JConvexHull.Build(pointCloud, JConvexHull.Approximation.Level5);

            int counter = 0;

            foreach (Tuple<int, int, int> face in indices) {
                JVector a = pointCloud[face.Item2] - pointCloud[face.Item1];
                JVector b = pointCloud[face.Item3] - pointCloud[face.Item1];
                JVector normal = JVector.Cross(a, b);

                AddVertex(Conversion.ToXnaVector(pointCloud[face.Item1]), Conversion.ToXnaVector(normal));
                AddVertex(Conversion.ToXnaVector(pointCloud[face.Item2]), Conversion.ToXnaVector(normal));
                AddVertex(Conversion.ToXnaVector(pointCloud[face.Item3]), Conversion.ToXnaVector(normal));

                AddIndex(counter + 0);
                AddIndex(counter + 1);
                AddIndex(counter + 2);

                counter += 3;
            }

            //foreach (JConvexHull.Face face in ConvexHull.HullFaces) {
            //    AddVertex(Conversion.ToXnaVector(pointCloud[face.VertexC]), Conversion.ToXnaVector(face.Normal));
            //    AddVertex(Conversion.ToXnaVector(pointCloud[face.VertexB]), Conversion.ToXnaVector(face.Normal));
            //    AddVertex(Conversion.ToXnaVector(pointCloud[face.VertexA]), Conversion.ToXnaVector(face.Normal));

            //    AddIndex(counter + 0);
            //    AddIndex(counter + 1);
            //    AddIndex(counter + 2);

            //    counter += 3;
            //}


            InitializePrimitive(device);
        }

    }
}
