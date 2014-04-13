float4x4 World;
float4x4 View;
float4x4 Projection;

texture2D Texture;
sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 OldPosition: COLOR1;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Color = input.Color;
	output.OldPosition = abs(input.Position);
	output.OldPosition.x = output.OldPosition.x/2 + 0.5f;
	output.OldPosition.y = output.OldPosition.y/2 + 0.5f;
    return output;
}

float4 PixelShaderSimple(VertexShaderOutput input) : COLOR0
{
	input.Color.a = tex2D(TextureSampler, input.OldPosition.xy).a;
	return input.Color;
}

technique None
{
    pass P0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderSimple();
    }
}
