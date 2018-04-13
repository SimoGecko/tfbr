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
{int kernelSize = 3;
    float4 result = float4(0.0f, 0.0f, 0.0f, 0.0f);
    
	for (int i = 0; i < 3; ++i) {
		for (int j = 0; j < 3; ++j) {
			float2 dis = float2(2*(-1 + i) / screenSize.x, 2*(-1 + j) / screenSize.y);
			result += tex2D(TextureSampler, textureCoordinate.xy + dis) * kernel[i][j];
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
#if SM4
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
#elif SM3
        PixelShader = compile ps_3_0 PixelShaderFunction();
#else
        PixelShader = compile ps_2_0 PixelShaderFunction();
#endif
    }
}
