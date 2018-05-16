﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BRS.Engine.Utilities {
    public class ModelInstance {
        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static readonly VertexDeclaration InstanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public readonly List<GameObject> GameObjects;
        public Matrix[] Matrices = new Matrix[0];
        public DynamicVertexBuffer VertexBuffer;

        public ModelInstance(List<GameObject> transforms) {
            GameObjects = transforms;
        }

        public void Add(GameObject transform) {
            GameObjects.Add(transform);
        }

        public void Remove(GameObject transform) {
            GameObjects.Remove(transform);
        }

        public void Update() {
            Array.Resize(ref Matrices, GameObjects.Count);

            for (int i = 0; i < GameObjects.Count; ++i) {
                Matrices[i] = GameObjects[i].transform.World;
            }

            if ((VertexBuffer == null) ||
                (GameObjects.Count > VertexBuffer.VertexCount)) {
                if (VertexBuffer != null)
                    VertexBuffer.Dispose();

                VertexBuffer = new DynamicVertexBuffer(Graphics.gD, InstanceVertexDeclaration,
                    GameObjects.Count, BufferUsage.WriteOnly);
            }

            if (Matrices.Length >= 0) {
                // Transfer the latest instance transform matrices into the instanceVertexBuffer.
                VertexBuffer.SetData(Matrices, 0, Matrices.Length, SetDataOptions.Discard);
            }
        }
    }
}
