// This will use the texture bound to the object( like from the sprite batch ).


float Distance;
float Range;
float Near;
float Far;
float4 active; 
float players;

texture ScreenTexture;
sampler SceneSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
texture DepthTexture;
sampler2D DepthSampler = sampler_state {
	Texture = <DepthTexture>;
	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};

texture BlurScene;
sampler2D BlurSceneSampler = sampler_state {
	Texture = <BlurScene>;
	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};


float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
	// Get the scene texel
	float4 NormalScene = tex2D(SceneSampler, textureCoordinate);
	
	// Get the blurred scene texel
	float4 BlurScene = tex2D(BlurSceneSampler, textureCoordinate);
	
	// Get the depth texel
	float  fDepth = tex2D(DepthSampler, textureCoordinate).r;
	return tex2D(DepthSampler, textureCoordinate);
	
	
	
	
	
	
	
	
	
	// Invert the depth texel so the background is white and the nearest objects are black
	fDepth = 1 - fDepth;
	
	// Calculate the distance from the selected distance and range on our DoF effect, set from the application
	float fSceneZ = ( -Near * Far ) / ( fDepth - Far);
	float blurFactor = saturate(abs(fSceneZ-Distance)/Range);
	
	// Based on how far the texel is from "distance" in Distance, stored in blurFactor, mix the scene
	return lerp(NormalScene, BlurScene, blurFactor);
	
}

technique PostProcess
{
	pass P0
	{
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}