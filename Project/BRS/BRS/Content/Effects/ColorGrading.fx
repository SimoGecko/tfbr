////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VARIABLES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

float Size = 16;
float SizeRoot = 4;

//Texture2D InputTexture;
texture ScreenTexture;
 
// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
Texture2D LUT;

/*SamplerState LinearSampler
{
	Texture = ( LUT );
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;

	AddressU = WRAP;
	AddressV = CLAMP;
};*/


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  PIXEL SHADER
////////////////////////////////////////////////////////////////////////////////////////////////////////////


float4 PixelShaderApplyLUT(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 textureCoordinate : TEXCOORD0) : COLOR0
{
	
	//Our input
	//float4 baseTexture = InputTexture.Load(int3(input.Position.xy, 0));
	float4 baseTexture = tex2D(TextureSampler, textureCoordinate.xy);
	

	//Manual trilinear interpolation

	//We need to clamp since our values go, for example, from 0 to 15. But with a red value of 1.0 we would get 16, which is on the next table already.

	//OBSOLETE: We also need to shift half a pixel to the left, since our sampling locations do not match the storage location (see CreateLUT)
	//float halfOffset = 0.5f;

	float red = baseTexture.r * (Size - 1);

	float redinterpol = frac(red);

	float green = baseTexture.g * (Size - 1);
	float greeninterpol = frac(green);

	float blue = baseTexture.b * (Size - 1);
	float blueinterpol = frac(blue);

	//Blue base value

	float row = trunc(blue / SizeRoot);
	float col = trunc(blue % SizeRoot);

	float2 blueBaseTable = float2(trunc(col * Size), trunc(row * Size));

	float4 b0r1g0;
	float4 b0r0g1;
	float4 b0r1g1;
	float4 b1r0g0;
	float4 b1r1g0;
	float4 b1r0g1;
	float4 b1r1g1;

	/*
	We need to read 8 values (like in a 3d LUT) and interpolate between them.
	This cannot be done with default hardware filtering so I am doing it manually.
	Note that we must not interpolate when on the borders of tables!
	*/

	//Red 0 and 1, Green 0

	float4 b0r0g0 = LUT.Load(int3(blueBaseTable.x + red, blueBaseTable.y + green, 0));

	[branch]
	if (red < Size - 1)
		b0r1g0 = LUT.Load(int3(blueBaseTable.x + red + 1, blueBaseTable.y + green, 0));
	else
		b0r1g0 = b0r0g0;

	// Green 1

	[branch]
	if (green < Size - 1)
	{
		//Red 0 and 1

		b0r0g1 = LUT.Load(int3(blueBaseTable.x + red, blueBaseTable.y + green + 1, 0));

		[branch]
		if (red < Size - 1)
			b0r1g1 = LUT.Load(int3(blueBaseTable.x + red + 1, blueBaseTable.y + green + 1, 0));
		else
			b0r1g1 = b0r0g1;
	}
	else
	{
		b0r0g1 = b0r0g0;
		b0r1g1 = b0r0g1;
	}

	[branch]
	if (blue < Size - 1)
	{
		blue += 1;
		row = trunc(blue / SizeRoot);
		col = trunc(blue % SizeRoot);

		blueBaseTable = float2(trunc(col * Size), trunc(row * Size));

		b1r0g0 = LUT.Load(int3(blueBaseTable.x + red, blueBaseTable.y + green, 0));

		[branch]
		if (red < Size - 1)
			b1r1g0 = LUT.Load(int3(blueBaseTable.x + red + 1, blueBaseTable.y + green, 0));
		else
			b1r1g0 = b0r0g0;

		// Green 1

		[branch]
		if (green < Size - 1)
		{
			//Red 0 and 1

			b1r0g1 = LUT.Load(int3(blueBaseTable.x + red, blueBaseTable.y + green + 1, 0));

			[branch]
			if (red < Size - 1)
				b1r1g1 = LUT.Load(int3(blueBaseTable.x + red + 1, blueBaseTable.y + green + 1, 0));
			else
				b1r1g1 = b0r0g1;
		}
		else
		{
			b1r0g1 = b0r0g0;
			b1r1g1 = b0r0g1;
		}
	}
	else
	{
		b1r0g0 = b0r0g0;
		b1r1g0 = b0r1g0;
		b1r0g1 = b0r0g0;
		b1r1g1 = b0r1g1;
	}

	float4 result = lerp(lerp(b0r0g0, b0r1g0, redinterpol), lerp(b0r0g1, b0r1g1, redinterpol), greeninterpol);
	float4 result2 = lerp(lerp(b1r0g0, b1r1g0, redinterpol), lerp(b1r0g1, b1r1g1, redinterpol), greeninterpol);

	result = lerp(result, result2, blueinterpol);

	return result;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  TECHNIQUES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique ApplyLUT
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 PixelShaderApplyLUT();
	}
}
