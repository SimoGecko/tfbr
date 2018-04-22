//-----------------------------------------------------------------------------
// GeometricPrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
// Managed for this project by Andreas Emch

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics.Primitives3D {
    /// <summary>
    /// Base class for simple geometric primitive models. This provides a vertex
    /// buffer, an index buffer, plus methods for drawing the model. Classes for
    /// specific types of primitive (CubePrimitive, SpherePrimitive, etc.) are
    /// derived from this common base, and use the AddVertex and AddIndex methods
    /// to specify their geometry.
    /// </summary>
    public abstract class GeometricPrimitive : IDisposable {

        #region Fields

        // During the process of constructing a primitive model, vertex
        // and index data is stored on the CPU in these managed lists.
        readonly List<VertexPositionNormal> _vertices = new List<VertexPositionNormal>();
        readonly List<ushort> _indices = new List<ushort>();

        // Once all the geometry has been specified, the InitializePrimitive
        // method copies the vertex and index data into these buffers, which
        // store it on the GPU ready for efficient rendering.
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        #endregion

        #region Initialization


        /// <summary>
        /// Adds a new vertex to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddVertex(Vector3 position, Vector3 normal) {
            _vertices.Add(new VertexPositionNormal(position, normal));
        }


        /// <summary>
        /// Adds a new index to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddIndex(int index) {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index));

            _indices.Add((ushort)index);
        }


        /// <summary>
        /// Queries the index of the current vertex. This starts at
        /// zero, and increments every time AddVertex is called.
        /// </summary>
        protected int CurrentVertex {
            get { return _vertices.Count; }
        }


        /// <summary>
        /// Once all the geometry has been specified by calling AddVertex and AddIndex,
        /// this method copies the vertex and index data into GPU format buffers, ready
        /// for efficient rendering.
        /// </summary>
        protected void InitializePrimitive(GraphicsDevice graphicsDevice) {
            // Create a vertex declaration, describing the format of our vertex data.

            // Create a vertex buffer, and copy our vertex data into it.
            _vertexBuffer = new VertexBuffer(graphicsDevice,
                                            typeof(VertexPositionNormal),
                                            _vertices.Count, BufferUsage.None);

            _vertexBuffer.SetData(_vertices.ToArray());

            // Create an index buffer, and copy our index data into it.
            _indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort),
                                          _indices.Count, BufferUsage.None);

            _indexBuffer.SetData(_indices.ToArray());

        }


        /// <summary>
        /// Finalizer.
        /// </summary>
        ~GeometricPrimitive() {
            Dispose(false);
        }


        /// <summary>
        /// Frees resources used by this object.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Frees resources used by this object.
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _vertexBuffer?.Dispose();
                _indexBuffer?.Dispose();
            }
        }


        #endregion

        #region Draw

        private Matrix[] _worlds = new Matrix[1];
        private int _index;

        public void AddWorldMatrix(Matrix matrix) {
            if (_index == _worlds.Length) {
                Matrix[] temp = new Matrix[_worlds.Length + 50];
                _worlds.CopyTo(temp, 0);
                _worlds = temp;
            }

            _worlds[_index] = matrix;
            _index++;
        }


        /// <summary>
        /// Draws the primitive model, using the specified effect. Unlike the other
        /// Draw overload where you just specify the world/view/projection matrices
        /// and color, this method does not set any renderstates, so you must make
        /// sure all states are set to sensible values before you call it.
        /// </summary>
        public void Draw(BasicEffect effect) {
            if (_index == 0) return;

            GraphicsDevice graphicsDevice = effect.GraphicsDevice;

            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            int primitiveCount = _indices.Count / 3;

            for (int i = 0; i < _index; i++) {
                effect.World = _worlds[i];
                effect.CurrentTechnique.Passes[0].Apply();
                effect.Alpha = 0.5f;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                     _vertices.Count, 0, primitiveCount);
            }

            _index = 0;

        }

        #endregion

    }
}
