//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture ScreenTexture;

// shader-parameters
float time;
float4 startTime;

// not zero means active
float4 active;
float players;

// Shader-settings
float durationFadeIn = 0.1;
float durationFadeOut = 1.9;
float max = 1.0;
float min = 0.1;

// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state {
	Texture = <ScreenTexture>;
};

// Interpolate the color based on the time-difference and based on the player
float4 Interpolation(float4 color, int playerId) {
	float timeDiff = time - startTime[playerId];
	float alpha = 0.0;

	if (timeDiff < durationFadeIn) {
		alpha = max + (min - max) * (timeDiff / durationFadeIn);
	} else {
		alpha = min + (max - min) * (timeDiff - durationFadeIn) / durationFadeOut;
	}

	// make sure alpha does not exceed the min or max value
	alpha = clamp(alpha, min, max);
	float beta = 1.0 - alpha;

	// Interpolate between the true color and the greyscale
	float value = (color.r + color.g + color.b) / 3;
	color.r = (alpha * color.r) + (beta * value);
	color.g = (alpha * color.g) + (beta * value);
	color.b = (alpha * color.b) + (beta * value);

	return color;
}

//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader will simply look up the color of the texture at the
// requested point, and turns it into a shade of gray
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(TextureSampler, textureCoordinate.xy);
	// check if the shader needs to be applied to this part of the screen
	if (textureCoordinate.x < 0.5 && textureCoordinate.y < 0.5) {
		if (active.x == 0) {
			return color;
		} else {
			return Interpolation(color, 0);
		}
	}
	if (textureCoordinate.x >= 0.5 && textureCoordinate.y < 0.5) {
		if (active.y == 0) {
			return color;
		} else {
			return Interpolation(color, 1);
		}
	}
	if (textureCoordinate.x < 0.5 && textureCoordinate.y >= 0.5) {
		if (active.z == 0) {
			return color;
		} else {
			return Interpolation(color, 2);
		}
	}
	if (textureCoordinate.x >= 0.5 && textureCoordinate.y >= 0.5) {
		if (active.w == 0) {
			return color;
		} else {
			return Interpolation(color, 3);
		}
	}

	return color;
}

//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique BlackAndWhite {
	pass Pass1 {
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}
