#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


float4x4 World;
float4x4 View;
float4x4 Projection;

texture ColorTexture;
sampler2D textureSampler1 = sampler_state {
    Texture = (ColorTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};



struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 UV0 : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 UV0 : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.UV0 = input.UV0;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
 
    float4 textureColor = tex2D(textureSampler1, input.UV0);
    textureColor.a = 1;

 
 	//return textureColor;
 	return textureColor;
}

technique TexturedLight
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}