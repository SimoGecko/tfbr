//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;

// ------------------------------- VARS
float time;
float4 startTime;
float2 centerCoord0;        	// 0.5, 0.5 is the screen center
float2 centerCoord1;        	// 0.5, 0.5 is the screen center
float2 centerCoord2;        	// 0.5, 0.5 is the screen center
float2 centerCoord3;        	// 0.5, 0.5 is the screen center
float3 shockParams;			// 10.0, 0.8, 0.1
float ANIMATION_LENGTH = 1;
float4 active;
float players;

// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state {
	Texture = <ScreenTexture>;
};


// Calculate the shockwave based on the player
float4 CalculateShockWave(float2 textureCoordinate : TEXCOORD0, float4 defColor, int playerId) {
	// First get the correct center of the shockwave
	float2 centerCoord;

	switch (playerId) {
	case 0: centerCoord = centerCoord0; break;
	case 1: centerCoord = centerCoord1; break;
	case 2: centerCoord = centerCoord2; break;
	case 3: centerCoord = centerCoord3; break;
	}

	float2 distVec = textureCoordinate.xy - centerCoord;
	float distance = length(distVec);
	float duration = (time - startTime[playerId]) / ANIMATION_LENGTH;

	if ((distance <= (duration + shockParams.z)) && (distance >= (duration - shockParams.z))) {
		float diff = (distance - duration);
		float powDiff = 1.0 - pow(abs(diff*shockParams.x), shockParams.y);
		float diffTime = diff  * powDiff;
		float2 diffUV = normalize(distVec);
		float2 newTexCoord = textureCoordinate.xy + (diffUV * diffTime);
		float4 color = tex2D(TextureSampler, newTexCoord.xy);
		return color;
	}

	float4 color = tex2D(TextureSampler, textureCoordinate.xy);
	return color;
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
			return CalculateShockWave(textureCoordinate, defColor, 0);
		}
	}
	if (textureCoordinate.x >= 0.5 && textureCoordinate.y < 0.5) {
		if (active.y == 0) {
			return defColor;
		} else {
			return CalculateShockWave(textureCoordinate, defColor, 1);
		}
	}
	if (textureCoordinate.x < 0.5 && textureCoordinate.y >= 0.5) {
		if (active.z == 0) {
			return defColor;
		} else {
			return CalculateShockWave(textureCoordinate, defColor, 2);
		}
	}
	if (textureCoordinate.x >= 0.5 && textureCoordinate.y >= 0.5) {
		if (active.w == 0) {
			return defColor;
		} else {
			return CalculateShockWave(textureCoordinate, defColor, 3);
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
