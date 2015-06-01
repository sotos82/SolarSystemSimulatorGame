Shader "TerraViz/Earth-NightLights"
{
  Properties 
  {
_MainTex("_MainTex", 2D) = "black" {}
_Normals("_Normals", 2D) = "black" {}
_Lights("_Lights", 2D) = "black" {}
_LightScale("_LightScale", Float) = 1
_AtmosNear("_AtmosNear", Color) = (0.1686275,0.7372549,1,1)
_AtmosFar("_AtmosFar", Color) = (0.4557808,0.5187039,0.9850746,1)
_AtmosFalloff("_AtmosFalloff", Float) = 3

  }
  
  SubShader 
  {
    Tags
    {
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

    }

    
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Offset 1, 2
Fog{
}


    CGPROGRAM
#pragma surface surf BlinnPhongEditor
#pragma target 2.0

sampler2D _MainTex;
sampler2D _Normals;
sampler2D _Lights;
float _LightScale;
float4 _AtmosNear;
float4 _AtmosFar;
float _AtmosFalloff;

      struct EditorSurfaceOutput {
        half3 Albedo;
        half3 Normal;
        half3 Emission;
        half3 Gloss;
        half Specular;
        half Alpha;
        half4 Custom;
      };
      
      inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
      {
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.g -= .01 * s.Alpha;
c.r -= .03 * s.Alpha;
c.rg += min(s.Custom, s.Alpha);
c.b += 0.75 * min(s.Custom, s.Alpha);
c.b = saturate(c.b + s.Alpha * .02);
c.a = 1.0;
return c;

      }

      inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
      {
        half3 h = normalize (lightDir + viewDir);
        
        half diff = max (0, dot ( lightDir, s.Normal ));
        
        float nh = max (0, dot (s.Normal, h));
        float spec = pow (nh, s.Specular*128.0);
        
        half4 res;
        res.rgb = _LightColor0.rgb * diff;
        res.w = spec * Luminance (_LightColor0.rgb);
        res *= atten * 2.0;

		//s.Custom is now 1 where the earth is dark.  Save out value of night lights to Alpha
		half invdiff = 1 - saturate(16 * diff);
		s.Alpha = invdiff;
		
        return LightingBlinnPhongEditor_PrePass( s, res );
      }
      
      struct Input {
        float3 viewDir;
float2 uv_MainTex;
float2 uv_Normals;
float2 uv_Lights;

      };

      void surf (Input IN, inout EditorSurfaceOutput o) {
        o.Gloss = 0.0;
        o.Specular = 0.0;
        o.Custom = 0.0;
        o.Alpha = 1.0;
        
		float4 Fresnel0_1_NoInput = float4(0,0,1,1);
		float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
		float4 Pow0=pow(Fresnel0,_AtmosFalloff.xxxx);
		float4 Saturate0=saturate(Pow0);
		float4 Lerp0=lerp(_AtmosNear,_AtmosFar,Saturate0);
		float4 Multiply1=Lerp0 * Saturate0;
		float4 Sampled2D2=tex2D(_MainTex,IN.uv_MainTex.xy);
		float4 Add0=Multiply1 + Sampled2D2;
		float4 Sampled2D0=tex2D(_Normals,IN.uv_Normals.xy);
		float4 UnpackNormal0=float4(UnpackNormal(Sampled2D0).xyz, 1.0);
		
		o.Albedo = Add0;
		o.Normal = UnpackNormal0;
		//o.Emission = Multiply0;
        o.Emission = 0.0;
        
        //float4 Multiply0=Sampled2D1 * _LightScale.xxxx;
		o.Custom = tex2D(_Lights,IN.uv_Lights.xy).r * _LightScale;

        o.Normal = normalize(o.Normal);
      }
    ENDCG
  }
  Fallback "Diffuse"
}