//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;


//softness of our vignette
float SOFTNESS = 1.8;
 
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
	
	//1. VIGNETTE
	
	float2 dist = textureCoordinate - 0.5f;
	color.rgb = color.rgb * (1.0f - dot(dist, dist) * SOFTNESS);
 
    return color;
}

//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique VIGNETTE
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}