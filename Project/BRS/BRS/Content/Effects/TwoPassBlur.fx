//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
sampler TextureSampler : register(s0);

// Screensize: passed by monogame
float2 screenSize;
float4 active; 
float players;

// Classic gaussian kernel
float Pixels[7] =
{
   0,
    1,
    2,
    3,
    4,
    5,
    6
};

float BlurWeights[7] =
{
   0.199471,
   0.176033,
   0.120985,
   0.064759,
   0.026995,
   0.008764,
   0.002216
};


float4 generalBlur(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0, bool horizontal) : COLOR0
{
	int usedPixels = 16;
	float4 result = tex2D(TextureSampler, textureCoordinate.xy) * 1.0/usedPixels;
	float stepSize = 1;
	
	for (int i = 1; i < usedPixels; ++i) {
		
		float2 dis;
		if(horizontal) {
			dis = float2(stepSize * i / screenSize.x, 0.0);
		} else {
			dis = float2(0.0, stepSize * i / screenSize.y);
		}
		float weight = 1.0;
		
		//if(i == 1) weight = 0.176033;
		//if(i == 2) weight = 0.120985;
		//if(i == 3) weight = 0.064759;
		//if(i == 4) weight = 0.026995;
		//if(i == 5) weight = 0.008764;
		//if(i == 6) weight = 0.002216;
		if (i >= 1) weight = 1.0/ (2*usedPixels);
		result += tex2D(TextureSampler, textureCoordinate.xy + dis) * weight;
		result += tex2D(TextureSampler, textureCoordinate.xy - dis) * weight;
	}
	
	
	
	float4 color = tex2D(TextureSampler, textureCoordinate.xy);
	// check if the shader needs to be applied to this part of the screen
	if(active.x == 0 && textureCoordinate.x < 0.5 && textureCoordinate.y < 0.5) {
			return color;
	}
	if(active.y == 0 && textureCoordinate.x >= 0.5 && textureCoordinate.y < 0.5) {
			return color;
	}
	if(active.z == 0 && textureCoordinate.x < 0.5 && textureCoordinate.y >= 0.5) {
			return color;
	}
	if(active.w == 0 && textureCoordinate.x >= 0.5 && textureCoordinate.y >= 0.5) {
			return color;
	}
    
	return result;
}

//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader samples the 3x3-neighborhood around the pixel and averages
// the colors based on the kernel weights.
float4 PixelShaderFunctionHorizontal(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
	return generalBlur(pos, color1, textureCoordinate, true);
}

float4 PixelShaderFunctionVertical(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{	
    return generalBlur(pos, color1, textureCoordinate, false);
}

//-------------------------- TECHNIQUES ----------------------------------------
technique Blur
{
	pass HorizontalBlur {
		PixelShader = compile ps_4_0 PixelShaderFunctionHorizontal();
	}
	pass VerticalBlur {
		PixelShader = compile ps_4_0 PixelShaderFunctionVertical();
	}
	pass HorizontalBlur {
		PixelShader = compile ps_4_0 PixelShaderFunctionHorizontal();
	}
	pass VerticalBlur {
		PixelShader = compile ps_4_0 PixelShaderFunctionVertical();
	}
}
