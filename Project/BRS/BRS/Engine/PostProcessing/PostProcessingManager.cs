using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine.PostProcessing
{
    public enum PostprocessingType {
        BlackAndWhite,
        Chromatic,
        Vignette,
        GaussianBlur
    }
    class PostProcessingManager
    {
        public static PostProcessingManager Instance { get; private set; }

        private List<PostProcessingEffect> _effects = new List<PostProcessingEffect>();


        public static void SetUpPostProcessing(ContentManager content)
        {
            Instance = new PostProcessingManager(content);
        }

        private PostProcessingManager(ContentManager content)
        {
            foreach (PostprocessingType pType in Enum.GetValues(typeof(PostprocessingType)))
            {
                String fileName = pType.ToString();
                Effect ppShader = content.Load<Effect>("Effects/" + fileName);
                _effects.Add(new PostProcessingEffect(1, false, ppShader, fileName));
            }
        }

        public bool SetShaderParameter(PostprocessingType shader, String parameterName, Vector2 arg)
        {
            int index = _effects.FindIndex(x => x.Name == parameterName);
            if (index > 0)
            {
                _effects[index].SetParameter(parameterName, arg);
                return true;
            }
            return false;
        }

        public bool SetShaderStatus(String shader, bool active)
        {
            int index = _effects.FindIndex(x => x.Name == shader);
            if (index > 0)
            {
                _effects[index].Active = active;
                return true;
            }
            return false;
        }


        public void Update(GameTime gameTime)
        {
            if (Input.GetKeyDown(Keys.D1))
            {
                _effects[0].Active = !_effects[0].Active;
            }
            if (Input.GetKeyDown(Keys.D2))
            {
                _effects[1].Active = !_effects[1].Active;
            }
            if (Input.GetKeyDown(Keys.D3))
            {
                _effects[2].Active = !_effects[2].Active;
            }
        }

        public void Draw(RenderTarget2D renderTarget, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            RenderTarget2D curTarget = renderTarget;
            RenderTarget2D nextTarget = new RenderTarget2D(
                spriteBatch.GraphicsDevice,
                Screen.Width,                   // GraphicsDevice.PresentationParameters.BackBufferWidth,
                Screen.Height,                  // GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            graphicsDevice.SetRenderTarget(nextTarget);

            // if dynamic props are needed
            foreach (var ppShader in _effects)
            {

                if (ppShader.Active)
                {
                    for (int i = 0; i < ppShader.Passes; i++)
                    {
                        // apply post processing shader
                        spriteBatch.Begin(SpriteSortMode.Immediate,
                                BlendState.AlphaBlend,
                                SamplerState.LinearClamp,
                                DepthStencilState.Default,
                                RasterizerState.CullNone);
                        ppShader.Effect.CurrentTechnique.Passes[0].Apply();
                        spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
                        spriteBatch.End();
                    }
                    curTarget = nextTarget;
                }
                
          
            }
            // draw to screen
            graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate,
                                BlendState.AlphaBlend,
                                SamplerState.LinearClamp,
                                DepthStencilState.Default,
                                RasterizerState.CullNone);
            spriteBatch.Draw(curTarget, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.White);
            spriteBatch.End();




        }
    }
}
