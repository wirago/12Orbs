texture ScreenTexture;

sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(TextureSampler, TextureCoordinate);

	float4 outputColor = color;
	outputColor.r = (color.r * 0.393) + (color.g * 0.769) + (color.b * 0.189);
	outputColor.g = (color.r * 0.349) + (color.g * 0.686) + (color.b * 0.168);
	outputColor.b = (color.r * 0.272) + (color.g * 0.534) + (color.b * 0.131);

	return outputColor;
}

technique Sepia
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}