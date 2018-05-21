float4x4 World;
float4x4 View;
float4x4 Projection;



///////////////////////////////////////////////////////////
// Depth texture shader
struct OUT_DEPTH
{
	float4 Position : SV_POSITION;
	float4 Distance : TEXCOORD0;
};

// check if in switch screen window the pixel is
OUT_DEPTH VertexShaderFunction(float4 pos : SV_POSITION) 
{
	OUT_DEPTH Out;
	float4 posN = pos;
	posN.w = 1.0;
	float4 worldPosition = mul(posN, World);
	float4 viewPosition = mul(worldPosition, View);
	Out.Position = mul(viewPosition, Projection);
	
	
	// Translate the vertex using matWorldViewProj.
	// Get the distance of the vertex between near and far clipping plane in matWorldViewProj.
	Out.Distance.x = 1 - (Out.Position.y/Out.Position.w);	
	
	
	return Out;
}



float4 RenderDepthMapPS( OUT_DEPTH In ) : COLOR0
{ 
	// should not really happen
	if(In.Distance.x < 0) {
		return float4(0,0,1,1);
	}
	
    return float4(In.Distance.x,0,0,1);
}

//-------------------------- TECHNIQUES ----------------------------------------
technique DepthMapShader
{
	pass P0
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = FALSE;
		
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader  = compile ps_4_0 RenderDepthMapPS();
	
	}
}
