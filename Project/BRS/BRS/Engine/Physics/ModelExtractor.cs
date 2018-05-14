using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRS.Engine.Physics.Primitives3D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {
    public class ModelExtractor {
        private ModelMeshPart mmpModel;
        public  Vector3[] ArrVectors;
        public short[] ArrIndices;
        public VertexPositionColor[] VpcVertices;

        public ModelExtractor(ModelMeshPart mmp, Vector3[] av, VertexPositionColor[] vv)
        {
            mmpModel = mmp;
            ArrVectors = av;
            VpcVertices = vv;
        }

        public void ExtractVertices() {
            this.mmpModel.VertexBuffer.GetData<Vector3>(this.ArrVectors);
            for (int a = 0; a < VpcVertices.Length; a += 2) {
                this.VpcVertices[a].Position.X = ArrVectors[a].X;
                this.VpcVertices[a].Position.Y = ArrVectors[a].Y;
                this.VpcVertices[a].Position.Z = ArrVectors[a].Z;
            }
        }

        public void ExtractIndices()
        {
            this.mmpModel.IndexBuffer.GetData<short>(this.ArrIndices);

        }


        public static VertexBuffer ExtractVertexBuffer(Model modModel)
        {
            ModelExtractor modelExtractor = null;

            foreach (ModelMesh modmModel in modModel.Meshes) {
                foreach (ModelMeshPart mmpModel in modmModel.MeshParts) {
                    modelExtractor = new ModelExtractor(mmpModel, new Vector3[mmpModel.NumVertices * 2], new VertexPositionColor[mmpModel.NumVertices]);
                    modelExtractor.ExtractVertices();
                }
            }

            if (modelExtractor == null)
            {
                return null;
            }

            for (int a = 0; a < modelExtractor.ArrVectors.Length; a++) {
                Debug.Log(modelExtractor.ArrVectors[a]);
            }
            VertexBuffer vertexBuffer = new VertexBuffer(Graphics.gD, typeof(VertexPositionColor), modelExtractor.VpcVertices.Length, BufferUsage.None);
            vertexBuffer.SetData(modelExtractor.VpcVertices);

            return vertexBuffer;
        }


        public static void ModelData(Model model, out VertexBuffer vertices, out IndexBuffer indices, out VertexPositionColorTexture[] v, out short[] i) {
            try
            {
                ModelMeshPart part = model.Meshes[0].MeshParts[0];

                VertexPositionColorTexture[] modelVertices =
                    new VertexPositionColorTexture[part.VertexBuffer.VertexCount];
                part.VertexBuffer.GetData(modelVertices);

                vertices = new VertexBuffer(Graphics.gD, typeof(VertexPositionColorTexture),
                    part.VertexBuffer.VertexCount, BufferUsage.None);
                vertices.SetData(modelVertices);

                short[] modelIndices = new short[part.IndexBuffer.IndexCount];
                part.IndexBuffer.GetData<short>(modelIndices);

                indices = new IndexBuffer(Graphics.gD, typeof(ushort), part.IndexBuffer.IndexCount,
                    BufferUsage.None);
                indices.SetData(modelIndices);

                v = modelVertices;
                i = modelIndices;
            }
            catch
            {
                vertices = null;
                indices = null;
                v = new VertexPositionColorTexture[0];
                i = new short[0];
            }
        }
    }
}
