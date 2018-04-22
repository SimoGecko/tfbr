//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;


float rOffset = 0.005;
float gOffset = 0.01;
float bOffset = -0.005;
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
	float2 dist = textureCoordinate - 0.5f;
	float factor = (1.0f - dot(dist, dist) * SOFTNESS);
    float4 color = tex2D(TextureSampler, textureCoordinate.xy);
	float rCur = lerp(rOffset,0, factor);
	float gCur = lerp(gOffset,0, factor);
	float bCur = lerp(bOffset,0, factor);
	
	float4 rValue = tex2D(TextureSampler, textureCoordinate.xy + float2(rCur, rCur));  
    float4 gValue = tex2D(TextureSampler, textureCoordinate.xy + float2(gCur, gCur));
    float4 bValue = tex2D(TextureSampler, textureCoordinate.xy + float2(bCur, bCur));  
	
	//
	color.r = rValue.r;
	color.g = gValue.g;
	color.b = bValue.b;
 
    return color;
}

//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique Chromatic
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
