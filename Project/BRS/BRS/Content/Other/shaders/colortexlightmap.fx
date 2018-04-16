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

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;


texture ColorTexture;
sampler2D textureSampler1 = sampler_state {
    Texture = (ColorTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture LightmapTexture;
sampler2D textureSampler2 = sampler_state {
    Texture = (LightmapTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};




struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.UV0 = input.UV0;
    output.UV1 = input.UV1;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
 
    float4 textureColor = tex2D(textureSampler1, input.UV0);
    textureColor.a = 1;

    float4 lightColor = tex2D(textureSampler2, input.UV1);
    lightColor.a = 1;
 
 	//return textureColor;
 	return saturate(textureColor * lightColor);
}

technique TexturedLight
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}