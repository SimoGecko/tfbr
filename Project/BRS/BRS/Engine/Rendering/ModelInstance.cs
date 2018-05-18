using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Rendering {
    /// <summary>
    /// Stores the information for a hardware instanciation of a model.
    /// </summary>
    public class ModelInstance {

        #region Properties and attributes

        /// <summary>
        /// Model for the current instance.
        /// </summary>
        public readonly Model Model;

        /// <summary>
        /// Material of the model-instance.
        /// </summary>
        /// <remarks>
        /// Important: One material to fit them all for the current model. 
        /// If for a model there are different materials needed, then different instances have to be created.
        /// </remarks>
        public readonly Material Material;

        /// <summary>
        /// All game-objects which belongs to the current instance
        /// </summary>
        public readonly List<GameObject> GameObjects = new List<GameObject>();

        /// <summary>
        /// Vertex-Array for the vertex-buffer
        /// </summary>
        public VertexPositionAlpha[] VertexInformation = new VertexPositionAlpha[0];

        /// <summary>
        /// Vertex-buffer for the instance
        /// </summary>
        public DynamicVertexBuffer VertexBuffer;

        #endregion

        #region Constructor

        /// <summary>
        /// Instanciate a new model
        /// </summary>
        /// <param name="model">Model to render</param>
        /// <param name="material">Material for the model</param>
        public ModelInstance(Model model, Material material) {
            Model = model;
            Material = material;
        }

        #endregion

        #region Collection-handling

        /// <summary>
        /// Add a new gameobject to the instance to draw in a batch
        /// </summary>
        /// <param name="gameObject">New instance of the model</param>
        public void Add(GameObject gameObject) {
            GameObjects.Add(gameObject);
        }

        /// <summary>
        /// Remove a gameobject from the instance so it's not drawn anymore
        /// </summary>
        /// <param name="gameObject"></param>
        public void Remove(GameObject gameObject) {
            GameObjects.Remove(gameObject);
        }

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Updates the vertex-buffer with the newest information
        /// </summary>
        public void Update() {
            // Vertex-buffer with 0 elements fails => handle this by explicitly doing nothing.
            // Important: Now there is most likely an instance remaining in the buffer => handle this case in the draw
            if (GameObjects.Count == 0) {
                return;
            }

            // Update the size (which is only done if needed) and transfer all needed values for the vertex-description
            Array.Resize(ref VertexInformation, GameObjects.Count);

            for (int i = 0; i < GameObjects.Count; ++i) {
                VertexInformation[i].Matrix = GameObjects[i].transform.World;
                VertexInformation[i].Alpha = GameObjects[i].material.Alpha;
            }

            // Re-Initialize the vertex-buffer if needed
            if (VertexBuffer == null || GameObjects.Count > VertexBuffer.VertexCount) {
                VertexBuffer?.Dispose();

                VertexBuffer = new DynamicVertexBuffer(Graphics.gD, VertexPositionAlpha.InstanceVertexDeclaration,
                    GameObjects.Count, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance gameObject matrices into the vertex-buffer.
            VertexBuffer.SetData(VertexInformation, 0, VertexInformation.Length, SetDataOptions.Discard);
        }

        /// <summary>
        /// Reset all instances so that no game-object belongs to any hardware-instance
        /// </summary>
        public void Reset() {
            GameObjects.Clear();
        }

        #endregion
    }
}
