 Shader "Custom/Vertex Emission" {
 Properties {
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Base (RGB)", 2D) = "white" {}
 }
  
 SubShader {
     Pass {
         Material {
             Ambient [_Color]
             Diffuse [_Color]
         }
         ColorMaterial Emission
         Lighting On
         SetTexture [_MainTex] {
             Combine texture * primary Double, texture * primary
         }
     }
 }
  
 Fallback "Mobile/VertexLit", 1
 }