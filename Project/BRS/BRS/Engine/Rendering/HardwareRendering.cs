// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Rendering {
    public static class HardwareRendering {

        #region Properties and attributes

        public static GraphicsDeviceManager GraphicsDeviceManager { private get; set; }
        private static GraphicsDevice GraphicsDevice => GraphicsDeviceManager.GraphicsDevice;

        // Effects
        private static Effect _instanceEffect;
        private static Effect _zBufferEffect;


        //reference
        private static readonly Dictionary<ModelType, ModelInstance> ModelTransformations = new Dictionary<ModelType, ModelInstance>();

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Load all needed effects
        /// </summary>
        public static void Start() {
            _instanceEffect = File.Load<Effect>("Other/shaders/instancedModel");
            _zBufferEffect = File.Load<Effect>("Other/effects/DepthHardwareInstancing");
        }

        /// <summary>
        /// Reset all instances so that no game-object belongs to any hardware-instance
        /// </summary>
        public static void Reset() {
            foreach (var keyValue in ModelTransformations) {
                keyValue.Value.Reset();
            }
        }

        /// <summary>
        /// Update the informration which are needed for the instance-drawing
        /// </summary>
        public static void Update() {
            foreach (var keyValue in ModelTransformations) {
                keyValue.Value.Update();
            }
        }

        /// <summary>
        /// Draw all the models with hardware-instancing
        /// </summary>
        public static void Draw() {
            foreach (ModelType mt in Enum.GetValues(typeof(ModelType))) {
                if (ModelTransformations.ContainsKey(mt)) {
                    DrawModelInstanciated(ModelTransformations[mt]);
                }
            }
        }

        /// <summary>
        /// Draw all the models with hardware-instancing
        /// </summary>
        public static void DrawDepth() {
            foreach (ModelType mt in Enum.GetValues(typeof(ModelType))) {
                if (ModelTransformations.ContainsKey(mt)) {
                    DrawModelInstanciated(ModelTransformations[mt], _zBufferEffect);
                }
            }
        }

        #endregion

        #region Instanciation-handling

        /// <summary>
        /// Initialize a new model for hardware-instanciation
        /// </summary>
        /// <param name="modelType">Type of the model to store uniquely</param>
        /// <param name="model">Model which is used for instancing</param>
        /// <param name="material">Material which is used for instancing</param>
        public static void InitializeModel(ModelType modelType, Model model, Material material) {
            ModelTransformations[modelType] = new ModelInstance(model, material);
        }

        /// <summary>
        /// Add a gameobject to an instance to draw
        /// </summary>
        /// <param name="modelType">Model-type on which the gameobject is added</param>
        /// <param name="gameObject"></param>
        public static void AddInstance(ModelType modelType, GameObject gameObject) {
            if (ModelTransformations.ContainsKey(modelType)) {
                ModelInstance modelInstance = ModelTransformations[modelType];
                modelInstance.Add(gameObject);

                // Store reference to the model and material on the gameobject so that there is an instant access
                gameObject.Model = modelInstance.Model;
                gameObject.material = modelInstance.Material;
            } else {
                throw new Exception("Should not be here");
            }
        }

        /// <summary>
        /// Remove a gameobject to not be drawn anymore
        /// </summary>
        /// <param name="modelType">Model-type on which the gameobject was added</param>
        /// <param name="gameObject"></param>
        public static void RemoveInstance(ModelType modelType, GameObject gameObject) {
            if (ModelTransformations.ContainsKey(modelType)) {
                ModelTransformations[modelType].Remove(gameObject);
            }
        }

        /// <summary>
        /// Draw a model with the vertex-/index-buffers to accelerate the drawing
        /// </summary>
        /// <param name="modelInstance">Instanciation information</param>
        static void DrawModelInstanciated(ModelInstance modelInstance) {
            if (modelInstance.GameObjects.Count == 0) {
                return;
            }

            Material material = modelInstance.Material;
            Texture2D texture = material.colorTex;
            Texture2D lightmap = material.lightTex;

            foreach (ModelMesh mesh in modelInstance.Model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(part.VertexBuffer, part.VertexOffset, 0),
                        new VertexBufferBinding(modelInstance.VertexBuffer, 0, 1)
                    );

                    GraphicsDevice.Indices = part.IndexBuffer;

                    // load the instanciation effect and set the correct technique based on the material
                    part.Effect = _instanceEffect;
                    _instanceEffect.CurrentTechnique = _instanceEffect.Techniques[material.RenderingType.ToString()];

                    // Set properties which are valid for all cameras
                    // - Transformation
                    _instanceEffect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                    // - Texture
                    _instanceEffect.Parameters["ColorTexture"].SetValue(texture);
                    _instanceEffect.Parameters["LightmapTexture"].SetValue(lightmap);

                    foreach (Camera cam in Screen.Cameras) {
                        GraphicsDevice.Viewport = cam.Viewport;

                        // Set camera-properties for shader
                        _instanceEffect.Parameters["View"].SetValue(cam.View);
                        _instanceEffect.Parameters["Projection"].SetValue(cam.Proj);

                        // Draw all the instance copies in a single call.
                        foreach (EffectPass pass in _instanceEffect.CurrentTechnique.Passes) {
                            pass.Apply();

                            GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                part.NumVertices, part.StartIndex,
                                part.PrimitiveCount, modelInstance.VertexInformation.Length);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw a model with the vertex-/index-buffers to accelerate the drawing
        /// </summary>
        /// <param name="modelInstance">Instanciation information</param>
        /// <param name="effect"></param>
        static void DrawModelInstanciated(ModelInstance modelInstance, Effect effect) {
            if (modelInstance.GameObjects.Count == 0) {
                return;
            }

            foreach (ModelMesh mesh in modelInstance.Model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(part.VertexBuffer, part.VertexOffset, 0),
                        new VertexBufferBinding(modelInstance.VertexBuffer, 0, 1)
                    );

                    GraphicsDevice.Indices = part.IndexBuffer;

                    // load the instanciation effect and set the correct technique based on the material
                    part.Effect = effect;

                    // Set properties which are valid for all cameras
                    // - Transformation
                    effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);

                    foreach (Camera cam in Screen.Cameras) {
                        GraphicsDevice.Viewport = cam.Viewport;

                        // Set camera-properties for shader
                        effect.Parameters["View"].SetValue(cam.View);
                        effect.Parameters["Projection"].SetValue(cam.ProjDepth);

                        // Draw all the instance copies in a single call.
                        foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                            pass.Apply();

                            GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                part.NumVertices, part.StartIndex,
                                part.PrimitiveCount, modelInstance.VertexInformation.Length);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
