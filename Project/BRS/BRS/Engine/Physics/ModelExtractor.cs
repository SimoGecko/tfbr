using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {
    public class ModelExtractor {
        private readonly ModelMeshPart _mmpModel;
        private readonly Vector3[] _arrVectors;
        private readonly VertexPositionColor[] _vpcVertices;

        public ModelExtractor(ModelMeshPart mmp, Vector3[] av, VertexPositionColor[] vv) {
            _mmpModel = mmp;
            _arrVectors = av;
            _vpcVertices = vv;
        }

        public void ExtractVertices() {
            _mmpModel.VertexBuffer.GetData(_arrVectors);
            for (int a = 0; a < _vpcVertices.Length; a += 2) {
                _vpcVertices[a].Position.X = _arrVectors[a].X;
                _vpcVertices[a].Position.Y = _arrVectors[a].Y;
                _vpcVertices[a].Position.Z = _arrVectors[a].Z;
            }
        }


        public static VertexBuffer ExtractVertexBuffer(Model modModel) {
            ModelExtractor modelExtractor = null;

            foreach (ModelMesh modmModel in modModel.Meshes) {
                foreach (ModelMeshPart mmpModel in modmModel.MeshParts) {
                    modelExtractor = new ModelExtractor(mmpModel, new Vector3[mmpModel.NumVertices * 2], new VertexPositionColor[mmpModel.NumVertices]);
                    modelExtractor.ExtractVertices();
                }
            }

            if (modelExtractor == null) {
                return null;
            }

            for (int a = 0; a < modelExtractor._arrVectors.Length; a++) {
                Debug.Log(modelExtractor._arrVectors[a]);
            }
            VertexBuffer vertexBuffer = new VertexBuffer(Graphics.gD, typeof(VertexPositionColor), modelExtractor._vpcVertices.Length, BufferUsage.None);
            vertexBuffer.SetData(modelExtractor._vpcVertices);

            return vertexBuffer;
        }


        public static void ModelData(Model model, out VertexBuffer vertices, out IndexBuffer indices, out VertexPositionColorTexture[] v, out short[] i) {
            try {
                ModelMeshPart part = model.Meshes[0].MeshParts[0];

                VertexPositionColorTexture[] modelVertices =
                    new VertexPositionColorTexture[part.VertexBuffer.VertexCount];
                part.VertexBuffer.GetData(modelVertices);

                vertices = new VertexBuffer(Graphics.gD, typeof(VertexPositionColorTexture),
                    part.VertexBuffer.VertexCount, BufferUsage.None);
                vertices.SetData(modelVertices);

                short[] modelIndices = new short[part.IndexBuffer.IndexCount];
                part.IndexBuffer.GetData(modelIndices);

                indices = new IndexBuffer(Graphics.gD, typeof(ushort), part.IndexBuffer.IndexCount,
                    BufferUsage.None);
                indices.SetData(modelIndices);

                v = modelVertices;
                i = modelIndices;
            } catch {
                vertices = null;
                indices = null;
                v = new VertexPositionColorTexture[0];
                i = new short[0];
            }
        }
    }
}
