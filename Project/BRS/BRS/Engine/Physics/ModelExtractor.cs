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

        public static void ModelData(Model model, out VertexBuffer vertices, out IndexBuffer indices) {
            try
            {
                ModelMeshPart part = model.Meshes[0].MeshParts[0];

                VertexPositionColorNormal[] modelVertices =
                    new VertexPositionColorNormal[part.VertexBuffer.VertexCount];
                part.VertexBuffer.GetData<VertexPositionColorNormal>(modelVertices);

                vertices = new VertexBuffer(Graphics.gD, typeof(VertexPositionNormalColor),
                    part.VertexBuffer.VertexCount, BufferUsage.WriteOnly);
                vertices.SetData(modelVertices);

                ushort[] modelIndices = new ushort[part.IndexBuffer.IndexCount];
                part.IndexBuffer.GetData<ushort>(modelIndices);

                indices = new IndexBuffer(Graphics.gD, typeof(ushort), part.IndexBuffer.IndexCount,
                    BufferUsage.WriteOnly);
                indices.SetData(modelIndices);
            }
            catch
            {
                vertices = null;
                indices = null;
            }
        }
    }
}
