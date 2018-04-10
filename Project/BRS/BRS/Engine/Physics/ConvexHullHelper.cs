using System.Collections.Generic;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {
    /// <summary>
    /// Helper to calculate the bounding box of a given model.
    /// Todo: (to Andy from Andy) Check if really used, otherwise delete
    /// </summary>
    public static class ConvexHullHelper {

        /// <summary>
        /// Calculate the convex-hull of the model without special world-transformation.
        /// </summary>
        /// <param name="model">XNA-model file</param>
        /// <returns>Jitter-shape</returns>
        public static ConvexHullShape Calculate(Model model) {
            return Calculate(model, Matrix.Identity);
        }


        /// <summary>
        /// Calculate the convex-hull of the model without special world-transformation.
        /// </summary>
        /// <param name="model">XNA-model file</param>
        /// <param name="worldTransform">World-transformation matrix</param>
        /// <returns>Jitter-shape</returns>
        public static ConvexHullShape Calculate(Model model, Matrix worldTransform) {
            List<JVector> vertices = new List<JVector>();

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart meshPart in mesh.MeshParts) {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float)) {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);
                        
                        vertices.Add(Conversion.ToJitterVector(transformedPosition));
                    }
                }
            }

            return new ConvexHullShape(vertices);
        }


    }
}
