//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;

float4 active; 
float players;

float rOffset = 0.005;
float gOffset = 0.01;
float bOffset = -0.005;
float SOFTNESS = 1;

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
	
	// center for dist computation
	float2 center = float2(0.5f, 0.5f);
	
	// check for player 1 out of 2
	if(active.x == 1 && active.z == 1 && players == 2 && textureCoordinate.x < 0.5) {
		center.x = 0.25f;
	}
	
	// check for player 1 out of 2
	if(active.y == 1 && active.w == 1 && players == 2 && textureCoordinate.x >= 0.5) {
		center.x = 0.75f;
	}
	
	// 4 PLAYERS in the game
	// check for player 1 out of 4
	if(active.x == 1 && players == 4 && textureCoordinate.x < 0.5 && textureCoordinate.y < 0.5) {
		center.x = 0.25f;
		center.y = 0.25f;
	}
	
	// check for player 2 out of 4
	if(active.y == 1 && players == 4 && textureCoordinate.x >= 0.5 && textureCoordinate.y < 0.5) {
		center.x = 0.75f;
		center.y = 0.25f;
	}
	// check for player 3 out of 4
	if(active.z == 1 && players == 4 && textureCoordinate.x < 0.5 && textureCoordinate.y >= 0.5) {
		center.x = 0.25f;
		center.y = 0.75f;
	}
	
	// check for player 4 out of 4
	if(active.w == 1 && players == 4 && textureCoordinate.x >= 0.5 && textureCoordinate.y >= 0.5) {
		center.x = 0.75f;
		center.y = 0.75f;
	}
	
	
	float2 dist = textureCoordinate - center;
	float factor = (1.0f - dot(dist, dist) * SOFTNESS);
    
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
