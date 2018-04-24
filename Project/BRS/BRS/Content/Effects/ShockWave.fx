//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;

// ------------------------------- VARS
float time;
float startTime;
float2 centerCoord;        	// 0.5, 0.5 is the screen center
float3 shockParams;			// 10.0, 0.8, 0.1
float ANIMATION_LENGTH = 1;
 
// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
 
//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader will simply look up the color of the texture at the
// requested point, and turns it into a shade of gray
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	float2 distVec = texCoord.xy - centerCoord;
	float distance = length(distVec);
	float duration = (time - startTime) / ANIMATION_LENGTH;
	if ( (distance <= (duration + shockParams.z)) && (distance >= (duration - shockParams.z)) ) {
		float diff = (distance - duration); 
		float powDiff = 1.0 - pow(abs(diff*shockParams.x),  shockParams.y); 
		float diffTime = diff  * powDiff; 
		float2 diffUV = normalize(distVec); 
		float2 newTexCoord = texCoord.xy + (diffUV * diffTime);
		float4 color = tex2D(TextureSampler, newTexCoord.xy);
		return color;
	} 
	
	float4 color = tex2D(TextureSampler, texCoord.xy);
	return color;
	
}
 
//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique ShockWave
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
