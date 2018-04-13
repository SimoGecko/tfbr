//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;
 
// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
 
//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader will simply look up the color of the texture at the
// requested point, and turns it into a shade of gray
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, textureCoordinate.xy);
 
    float value = (color.r + color.g + color.b) / 3; 
    color.r = value;
    color.g = value;
    color.b = value;
 
    return color;
}
 
//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique BlackAndWhite
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}