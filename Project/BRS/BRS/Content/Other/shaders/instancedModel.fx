//-----------------------------------------------------------------------------
// InstancedModel.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Camera settings.
float4x4 World;
float4x4 View;
float4x4 Projection;


texture ColorTexture;
sampler2D colorTextureSampler = sampler_state {
	Texture = (ColorTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture LightmapTexture;
sampler2D lightTextureSampler = sampler_state {
	Texture = (LightmapTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};


struct VertexShaderInputBaked {
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 ColorUV : TEXCOORD0;
	float2 LightUV : TEXCOORD1;
};

struct VertexShaderInputTexture {
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 ColorUV : TEXCOORD0;
	float Alpha: BLENDWEIGHT4;
};

struct VertexShaderOutput {
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 ColorUV : TEXCOORD0;
	float2 LightUV : TEXCOORD1;
	float Alpha: BLENDWEIGHT0;
};


// Vertex shader helper function shared between the two techniques.
VertexShaderOutput VertexShaderBaked(VertexShaderInputBaked input, float4x4 instanceTransform) {
	VertexShaderOutput output;

	// Apply the world and camera matrices to compute the output position.
	float4 worldPosition = mul(input.Position, instanceTransform);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// Copy across the input texture coordinate.
	output.ColorUV = input.ColorUV;
	output.LightUV = input.LightUV;

	return output;
}

VertexShaderOutput VertexShaderTexture(VertexShaderInputTexture input, float4x4 instanceTransform, float alpha) {
	VertexShaderOutput output;

	// Apply the world and camera matrices to compute the output position.
	float4 worldPosition = mul(input.Position, instanceTransform);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// Copy across the input texture coordinate.
	output.ColorUV = input.ColorUV;
	output.Alpha = alpha;

	return output;
}


// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
VertexShaderOutput HardwareInstancingVertexShaderBaked(VertexShaderInputBaked input, float4x4 instanceTransform : BLENDWEIGHT, float alpha : BLENDWEIGHT4) {
	return VertexShaderBaked(input, mul(World, transpose(instanceTransform)));
}

VertexShaderOutput HardwareInstancingVertexShaderTexture(VertexShaderInputTexture input, float4x4 instanceTransform : BLENDWEIGHT, float alpha : BLENDWEIGHT4) {
	return VertexShaderTexture(input, mul(World, transpose(instanceTransform)), alpha);
}


// Both techniques share this same pixel shader.
float4 PixelShaderFunctionBaked(VertexShaderOutput input) : COLOR0
{
	float4 textureColor = tex2D(colorTextureSampler, input.ColorUV);
	textureColor.a = 1;

	float4 lightColor = tex2D(lightTextureSampler, input.LightUV);
	lightColor.a = 1;

	return saturate(textureColor * lightColor);
}

float4 PixelShaderFunctionTextured(VertexShaderOutput input) : COLOR0
{
	float4 textureColor = tex2D(colorTextureSampler, input.ColorUV);
	textureColor.a = 1;

	return saturate(textureColor);
}

float4 PixelShaderFunctionTexturedAlpha(VertexShaderOutput input) : COLOR0
{
	float4 textureColor = tex2D(colorTextureSampler, input.ColorUV);

	return saturate(textureColor);
}

float4 PixelShaderFunctionTexturedAlphaAnimated(VertexShaderOutput input) : COLOR0
{
	float4 textureColor = tex2D(colorTextureSampler, input.ColorUV);
	textureColor.a = lerp(textureColor.a, 0, input.Alpha);

	return saturate(textureColor);
}

// Hardware instancing technique with baked light maps.
technique HIBaked {
	pass Pass1 {
		VertexShader = compile VS_SHADERMODEL HardwareInstancingVertexShaderBaked();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunctionBaked();
	}
}


// Hardware instancing technique with only texture.
technique HITexture {
	pass Pass1 {
		VertexShader = compile VS_SHADERMODEL HardwareInstancingVertexShaderTexture();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunctionTextured();
	}
}

// Hardware instancing technique with only texture.
technique HITextureAlpha {
	pass Pass1 {
		VertexShader = compile VS_SHADERMODEL HardwareInstancingVertexShaderTexture();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunctionTexturedAlpha();
	}
}

// Hardware instancing technique with only texture.
technique HITextureAlphaAnimated {
	pass Pass1 {
		VertexShader = compile VS_SHADERMODEL HardwareInstancingVertexShaderTexture();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunctionTexturedAlphaAnimated();
	}
}