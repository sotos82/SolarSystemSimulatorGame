Shader "TerraViz/Clouds" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	Offset -1, -2

CGPROGRAM
#pragma surface surf Lambert finalcolor:nightcolor alpha

sampler2D _MainTex;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}

void nightcolor (Input IN, SurfaceOutput o, inout fixed4 color)
{
	color.r = saturate(o.Albedo.r - (o.Albedo.r - color.r) * 1.1);
	color.g = saturate(o.Albedo.g - (o.Albedo.g - color.g) * 1.05);
	//color.rgb = saturate(o.Albedo - (o.Albedo - color.rgb) * 1.1);
}

ENDCG
}

Fallback "Transparent/VertexLit"
}
