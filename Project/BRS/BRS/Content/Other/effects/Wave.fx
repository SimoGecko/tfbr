//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;

// ------------------------------- VARS
float time;
float4 startTime;
float2 centerCoord0;			// 0.5, 0.5 is the screen center
float2 centerCoord1;			// 0.5, 0.5 is the screen center
float2 centerCoord2;			// 0.5, 0.5 is the screen center
float2 centerCoord3;			// 0.5, 0.5 is the screen center
float3 shockParams;				// 10.0, 0.8, 0.1

float4 animationLength = float4(1.0, 1.0, 1.0, 1.0);
float4 active;
float players;
float4 cameraDistance;
static const float PI = 3.14159265f;

// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state {
	Texture = <ScreenTexture>;
};


// Calculate the shockwave based on the player
float4 CalculateShockWave(float2 textureCoordinate : TEXCOORD0, float4 defColor, int playerId, float animationLength) {
	// First get the correct center of the shockwave
	float2 centerCoord;
	float playerCameraDist;
	

	switch (playerId) {
	case 0: 
		centerCoord = centerCoord0; 
		playerCameraDist = cameraDistance.x;
		break;
	case 1: 
		centerCoord = centerCoord1;
		playerCameraDist = cameraDistance.y;		
		break;
	case 2: 
		centerCoord = centerCoord2; 
		playerCameraDist = cameraDistance.z;	
		break;
	case 3: 
		centerCoord = centerCoord3; 
		playerCameraDist = cameraDistance.w;	
		break;
	}

	float2 distVec = textureCoordinate.xy - centerCoord;
	float distance = length(distVec);
	float duration = (time - startTime[playerId]) / animationLength;
	
	float sinArg = distance * 5.0 - duration * 5.0;
	float slope = cos(sinArg);
	
	float2 diffUV = normalize(distVec);
	float2 newTexCoord = textureCoordinate.xy + (diffUV * slope * 0.05);
	float4 color = tex2D(TextureSampler, newTexCoord.xy);
	
	
	float4 origColor = tex2D(TextureSampler, textureCoordinate.xy);
	float distScale = lerp(0.0, 1.0, playerCameraDist / 50.0);
	float scaleFactor = lerp(0.1, 0.01, distScale);
	
	if(distance < scaleFactor) {
		return lerp(color, origColor, distance / scaleFactor);
	}
	return origColor;
}

//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader will simply look up the color of the texture at the
// requested point, and turns it into a shade of gray
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
	float4 defColor = tex2D(TextureSampler, textureCoordinate.xy);
	// check if the shader needs to be applied to this part of the screen
	if (textureCoordinate.x < 0.5 && textureCoordinate.y < 0.5) {
		if (active.x == 0) {
			return defColor;
		} else {
			return CalculateShockWave(textureCoordinate, defColor, 0, animationLength.x);
		}
	}
	if (textureCoordinate.x >= 0.5 && textureCoordinate.y < 0.5) {
		if (active.y == 0) {
			return defColor;
		} else {
			return CalculateShockWave(textureCoordinate, defColor, 1, animationLength.y);
		}
	}
	if (textureCoordinate.x < 0.5 && textureCoordinate.y >= 0.5) {
		if (active.z == 0) {
			return defColor;
		} else {
			return CalculateShockWave(textureCoordinate, defColor, 2, animationLength.z);
		}
	}
	if (textureCoordinate.x >= 0.5 && textureCoordinate.y >= 0.5) {
		if (active.w == 0) {
			return defColor;
		} else {
			return CalculateShockWave(textureCoordinate, defColor, 3, animationLength.w);
		}
	}

	return defColor;
}

//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique ShockWave {
	pass Pass1 {
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}
