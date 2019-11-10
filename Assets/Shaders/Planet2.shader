// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "Custom/AtmosphereFromSpace" {
    Properties {
      _CameraPosition("Camera Position",Vector) = (0,0,0,0)
      _LightDir("Light Direction",Vector) = (0,0,0,0)
      _InvWaveLength("Inverse WaveLength",Color) = (0,0,0,0)
      _CameraHeight("Camera Height",Float) = 0
      _CameraHeight2("Camera Height2",Float) = 0
      _OuterRadius("Outer Radius",Float) = 0
      _OuterRadius2("Outer Radius 2",Float) = 0
      _InnerRadius("Inner Radius",Float) = 0
      _InnerRadius2("Inner Radius 2",Float) = 0
      _KrESun("KrESun",Float) = 0
      _KmESun("KmESun",Float) = 0
      _Kr4PI("Kr4PI",Float) = 0
      _Km4PI("Km4PI",Float) = 0
      _Scale("Scale",Float) = 0
      _ScaleDepth("Scale Depth",Float) = 0
      _ScaleOverScaleDepth("Scale Over Scale Depth",Float) = 0
      _Samples("Samples",Float) = 0
      _G("G",Float) = 0
      _G2("G2",Float) = 0
    }
    SubShader {
      Pass {
        Cull Front
        Blend One One
       
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
 
float4 _CameraPosition;
float4 _LightDir;
float4 _InvWaveLength;
float _CameraHeight;
float _CameraHeight2;    
float _OuterRadius;
float _OuterRadius2;     
float _InnerRadius;
float _InnerRadius2;
float _KrESun;
float _KmESun;
float _Kr4PI;
float _Km4PI;
float _Scale;
float _ScaleDepth;
float _ScaleOverScaleDepth;
float _Samples;
float _G;
float _G2;
 
struct v2f {
  float4 position : POSITION;
  float3 c0 : COLOR0;
  float3 c1 : COLOR1;
  float3 t0 : TEXCOORD0;
};
 
float getNearIntersection(float3 pos, float3 ray, float distance2, float radius2) {
  float B = 2.0 * dot(pos, ray);
  float C = distance2 - radius2;
  float det = max(0.0, B*B - 4.0 * C);
  return 0.5 * (-B - sqrt(det));
}
 
float expScale (float cos) {
    float x = 1 - cos;
    return _ScaleDepth * exp(-0.00287 + x*(0.459 + x*(3.83 + x*(-6.80 + x*5.25))));
}
 
v2f vert (float4 vertex : POSITION) {  
  float3 pos = vertex.xyz;
  float3 ray = pos - _CameraPosition.xyz;
  float far = length(ray);
  ray /= far;  
 
  float near = getNearIntersection(_CameraPosition.xyz,ray,_CameraHeight2,_OuterRadius2);
  float3 start = _CameraPosition.xyz + ray * near;
  far -= near;
 
  float startAngle = dot(ray,start) / _OuterRadius;
  //float startDepth = exp (_ScaleOverScaleDepth * (_InnerRadius - _CameraHeight));
  float startDepth = exp(-1.0 / _ScaleDepth);
  float startOffset = startDepth * expScale(startAngle);
  float sampleLength = far / _Samples;
  float scaledLength = sampleLength * _Scale;
  float3 sampleRay = ray * sampleLength;
  float3 samplePoint = start + sampleRay * 0.5f;
 
  float3 frontColor = float3(0,0,0);
  float3 attenuate;
 
  for (int i = 0; i < 1; i++) {
    float height = length (samplePoint);
    float depth = exp(_ScaleOverScaleDepth * (_InnerRadius - height));
    float lightAngle = dot(_LightDir.xyz, samplePoint) / height;
    float cameraAngle = dot(-ray, samplePoint) / height;
    float scatter = (startOffset + depth * (expScale (lightAngle) - expScale (cameraAngle)));
 
    attenuate = exp(-scatter * (_InvWaveLength.xyz * _Kr4PI + _Km4PI));
    frontColor += attenuate * (depth * scaledLength);
    samplePoint += sampleRay;
  }
 
  v2f OUT;
  OUT.position = UnityObjectToClipPos( vertex);
  OUT.t0 = _CameraPosition.xyz - vertex.xyz;
  OUT.c0.rgb = frontColor * (_InvWaveLength.xyz * _KrESun);
  OUT.c1.rgb = frontColor * _KmESun;
  return OUT;
}
 
float4 frag (v2f INPUT) : COLOR {
  float cos = dot(_LightDir.xyz, INPUT.t0) / length(INPUT.t0);
  float cos2 = cos * cos;
  float miePhase = 1.5 * ((1.0 - _G2) / (2.0 + _G2)) * (1.0 + cos2) / pow(1.0 + _G2 - 2.0*_G*cos, 1.5);
  float rayleighPhase = 0.75 * (1.0 + cos2);
  float4 fragColor;
 
  fragColor.xyz = (rayleighPhase * INPUT.c0) + (miePhase * INPUT.c1);
  fragColor.w = fragColor.z;
  return fragColor;
}
 
ENDCG  
      }
    }
    FallBack "None"
}
