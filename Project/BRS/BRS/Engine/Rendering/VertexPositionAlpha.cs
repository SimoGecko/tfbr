using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Rendering {
    /// <summary>
    /// Vertex structure which is used for the shaders on the GPU
    /// </summary>
    public struct VertexPositionAlpha : IVertexType {
        /// <summary>
        /// World-matrix which is used for an instance of a model to render
        /// </summary>
        public Matrix Matrix;

        /// <summary>
        /// Alpha-transparency for an instance of a model to use for texture
        /// </summary>
        public float Alpha;

        /// <summary>
        /// Vertex-declaration which has space for the position-matrix and the float for the alpha-value.
        /// This type has to be used for the vertex-buffer which is shared with the GPU.
        /// </summary>
        public static readonly VertexDeclaration InstanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3),
            new VertexElement(64, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 4)
        );

        /// <summary>
        /// Define vertex-declaration
        /// </summary>
        VertexDeclaration IVertexType.VertexDeclaration => InstanceVertexDeclaration;
    };
}