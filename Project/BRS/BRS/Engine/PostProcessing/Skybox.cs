using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BRS.Engine.PostProcessing
{
    public static class Skybox {
        static Model skyBox; // cube with normals inwards
        static TextureCube skyBoxTexture;
        static Effect skyBoxEffect; // skybox shader
        static float size = 500f; // size of cube

        public static void Start() {
            skyBox = File.Load<Model>("Models/primitives/cube"); // normals should be inward!
            skyBoxTexture = File.Load<TextureCube>("Images/skyboxes/Skybox");
            skyBoxEffect = File.Load<Effect>("Effects/Skybox");
        }
        
        public static void Draw(Camera cam) {
            return;
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes) {
                foreach (ModelMesh mesh in skyBox.Meshes) {
                    foreach (ModelMeshPart part in mesh.MeshParts) {
                        part.Effect = skyBoxEffect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size) * Matrix.CreateTranslation(cam.transform.position));
                        part.Effect.Parameters["View"].SetValue(cam.View);
                        part.Effect.Parameters["Projection"].SetValue(cam.Proj);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cam.transform.position);
                    }
                    mesh.Draw();
                }
            }
        }
    }
}

