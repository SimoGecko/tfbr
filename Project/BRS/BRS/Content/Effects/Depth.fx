float4x4 matWorldViewProj;


///////////////////////////////////////////////////////////
// Depth texture shader
struct OUT_DEPTH
{
	float4 Position : SV_POSITION;
	float Distance : TEXCOORD0;
};

OUT_DEPTH RenderDepthMapVS(float4 vPos: POSITION)
{
	OUT_DEPTH Out;
	// Translate the vertex using matWorldViewProj.
	Out.Position = mul(vPos, matWorldViewProj);
	// Get the distance of the vertex between near and far clipping plane in matWorldViewProj.
	Out.Distance.x = 1-(Out.Position.z/Out.Position.w);	
	
	return Out;
}

float4 RenderDepthMapPS( OUT_DEPTH In ) : COLOR0
{ 
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
		
        VertexShader = compile vs_4_0 RenderDepthMapVS();
        PixelShader  = compile ps_4_0 RenderDepthMapPS();
	
	}
}
