// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {
    /// <summary>
    /// Helper to calculate the bounding box of a given model.
    /// </summary>
    public static class BoundingBoxHelper {

        /// <summary>
        /// Calculate the bounding-box of the model without special world-transformation.
        /// </summary>
        /// <param name="model">XNA-model file</param>
        /// <returns>XNA-bounding-box</returns>
        public static BoundingBox Calculate(Model model) {
            return Calculate(model, Matrix.Identity);
        }


        /// <summary>
        /// Calculate the bounding-box of the model with a specified world-transformation.
        /// </summary>
        /// <param name="model">XNA-model file</param>
        /// <param name="worldTransform">World-transformation matrix</param>
        /// <returns>XNA-bounding-box</returns>
        public static BoundingBox Calculate(Model model, Matrix worldTransform) {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

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

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            return new BoundingBox(min, max);
        }


        /// <summary>
        /// Calculate the local bounding-box-size of the model with the correct scaling.
        /// </summary>
        /// <param name="model">XNA-model file</param>
        /// <param name="scale">Scaling per axis</param>
        /// <returns>Size of the scaled local-bounding</returns>
        public static Vector3 CalcualteSize(Model model, Vector3 scale) {
            BoundingBox bb = Calculate(model);
            Vector3 size = bb.Max - bb.Min;

            return new Vector3(
                scale.X * size.X,
                scale.Y * size.Y,
                scale.Z * size.Z
            );
        }

    }
}
