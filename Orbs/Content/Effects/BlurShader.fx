texture Texture;

sampler2D TextureSampler = sampler_state
{
	texture = <Texture>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};

float weights[15];
float offsets[15];

float4 BlurFunction(float4 position : POSITION0, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : Color0
{
	float4 output = float4(0, 0, 0, 1);

	for (int i = 0; i < 15; i++)
	{
		output += tex2D(TextureSampler, texCoord + float2(0, offsets[i])) * weights[i];
	}

	return output;
}

technique BlurFunction
{
	pass Blur
	{
		PixelShader = compile ps_2_0 BlurFunction();
	}
}