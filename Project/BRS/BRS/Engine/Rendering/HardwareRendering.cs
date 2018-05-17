using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Rendering {
    public static class HardwareRendering {
        public static GraphicsDeviceManager gDM;
        public static GraphicsDevice gD { get { return gDM.GraphicsDevice; } }

        // Effects
        private static Effect _instanceEffect;

        //reference
        private static readonly Dictionary<ModelType, ModelInstance> ModelTransformations = new Dictionary<ModelType, ModelInstance>();

        public static void Start() {
            _instanceEffect = File.Load<Effect>("Other/shaders/instancedModel");
        }


        public static void UpdateModelInstances() {
            foreach (var keyValue in ModelTransformations) {
                keyValue.Value.Update();
            }
        }

        public static void DrawModelInstances(Camera camera) {
            foreach (ModelType mt in Enum.GetValues(typeof(ModelType))) {
                if (ModelTransformations.ContainsKey(mt))
                    DrawModelInstanciated(camera.View, camera.Proj, ModelTransformations[mt]);
            }
            //foreach (var keyValue in ModelTransformations) {
            //    //Model modelName = keyValue.Key;


            //    DrawModelInstanciated(camera.View, camera.Proj, keyValue.Value);
            //}
        }

        static void DrawModelInstanciated(Matrix view, Matrix projection, ModelInstance modelInstance) {
            if (modelInstance.GameObjects.Count == 0) {
                return;
            }

            Material material = modelInstance.Material;
            Texture2D texture = material.colorTex;
            Texture2D lightmap = material.lightTex;

            foreach (ModelMesh mesh in modelInstance.Model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    gD.SetVertexBuffers(
                        new VertexBufferBinding(part.VertexBuffer, part.VertexOffset, 0),
                        new VertexBufferBinding(modelInstance.VertexBuffer, 0, 1)
                    );

                    gD.Indices = part.IndexBuffer;

                    part.Effect = _instanceEffect;

                    _instanceEffect.CurrentTechnique = _instanceEffect.Techniques[material.RenderingType.ToString()];

                    _instanceEffect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                    _instanceEffect.Parameters["View"].SetValue(view);
                    _instanceEffect.Parameters["Projection"].SetValue(projection);

                    _instanceEffect.Parameters["ColorTexture"].SetValue(texture);
                    _instanceEffect.Parameters["LightmapTexture"].SetValue(lightmap);


                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in _instanceEffect.CurrentTechnique.Passes) {
                        pass.Apply();

                        gD.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                            part.NumVertices, part.StartIndex,
                            part.PrimitiveCount, modelInstance.VertexInformation.Length);
                    }
                }
            }
        }


        public static void InitializeModel(ModelType modelType, Model model, Material material) {
            ModelTransformations[modelType] = new ModelInstance(model, material);
        }

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

        public static void RemoveInstance(ModelType modelName, GameObject gameObject) {
            if (ModelTransformations.ContainsKey(modelName)) {
                ModelTransformations[modelName].Remove(gameObject);
            }
        }

    }
}
