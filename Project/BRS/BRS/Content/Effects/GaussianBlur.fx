//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
sampler TextureSampler : register(s0);

// Screensize: passed by monogame
float2 screenSize;

// Classic gaussian kernel
float3x3 kernel = float3x3(
	1.0, 2.0, 1.0,
	2.0, 4.0, 2.0,
	1.0, 2.0, 1.0);

// Gaussian kernel only at edges
//float3x3 kernel = float3x3(
//	4.0, 0.0, 4.0,
//	0.0, 0.0, 0.0,
//	4.0, 0.0, 4.0);

//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader samples the 3x3-neighborhood around the pixel and averages
// the colors based on the kernel weights.
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
	int kernelSize = 3;
    float4 result = float4(0.0f, 0.0f, 0.0f, 0.0f);
    
	for (int i = -1; i < 2; ++i) {
		for (int j = -1; j < 2; ++j) {
			float2 dis = float2(i / screenSize.x, j / screenSize.y);
			result += tex2D(TextureSampler, textureCoordinate.xy + dis) * kernel[i+1][j+1];
		}
	}
    
	return result / 16.0;
}

//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
