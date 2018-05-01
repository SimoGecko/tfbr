// This will use the texture bound to the object( like from the sprite batch ).
sampler SceneSampler : register(s0);

// this needs to be on the global minimap 
// origin top left
// x_dir is the side of the wanted subimage
float2 x_dir;
float2 y_dir;
	
texture minimap;
sampler MinimapSampler = sampler_state
{
   Texture = <minimap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};



float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
	float2 imageCoord = textureCoordinate.x * x_dir;
	imageCoord = imageCoord +  textureCoordinate.y * y_dir;
	
	// clamp it to zero and one 
	imageCoord = clamp(imageCoord, 0.0, 1.0);
	
	// Get the scene texel
	float4 color = tex2D(MinimapSampler, imageCoord);
	
	return color;
}

technique PostProcess
{
	pass P0
	{
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}