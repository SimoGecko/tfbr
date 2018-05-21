#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Camera settings
float4x4 World;
float4x4 View;
float4x4 Projection;


struct VertexShaderInput {
	float4 Position : SV_POSITION;
};

struct VertexShaderOutput {
	float4 Position : SV_POSITION;
	float4 Depth : TEXCOORD0;
};

// check if in switch screen window the pixel is
VertexShaderOutput VertexShaderFunction(VertexShaderInput input, float4x4 instanceTransform) {
	VertexShaderOutput output;

	float4 posN = input.Position;
	posN.w = 1.0;

	float4 worldPosition = mul(posN, instanceTransform);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);


	// Translate the vertex using matWorldViewProj.
	// Get the distance of the vertex between near and far clipping plane in matWorldViewProj.
	output.Depth.x = 1 - (output.Position.z / output.Position.w);

	return output;
}

VertexShaderOutput HardwareInstancingVertexShaderFunction(VertexShaderInput input, float4x4 instanceTransform : BLENDWEIGHT) {
	return VertexShaderFunction(input, mul(World, transpose(instanceTransform)));
}


float4 RenderDepthMapPS(VertexShaderOutput input) : COLOR0
{
	// should not really happen
	//if(In.Distance.x < 0) {
	//	return float4(0,0,1,1);
	//}
	float depth = input.Depth.x;

	return float4(depth, 0, 0, 1);
}

//-------------------------- TECHNIQUES ----------------------------------------
technique DepthMapShader {
	pass P0 {
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = FALSE;

		VertexShader = compile VS_SHADERMODEL HardwareInstancingVertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL RenderDepthMapPS();

	}
}
