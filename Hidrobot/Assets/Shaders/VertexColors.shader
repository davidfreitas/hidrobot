Shader "Custom/VertexColors" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
    _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
    _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
    _EmsColor ("Emission Color", Color) = (1, 1, 1, 1)
}
SubShader {
	Tags { "RenderType"="Opaque" }
   
CGPROGRAM
#pragma surface surf BlinnPhong
 
 
sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float4 _EmsColor;
 
struct Input {
    float2 uv_MainTex;
    float2 uv_BumpMap;
    half4 color : COLOR0;
    float3 viewDir;
};
 
void surf (Input IN, inout SurfaceOutput o) {
	o.Albedo = IN.color.rgb;
	
//    o.Specular = _Shininess;

    half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
    o.Emission = _EmsColor.rgb * pow (rim, 5.0);// + floor(IN.color.r * IN.color.g * IN.color.b);
    
    o.Gloss = 0;
    o.Alpha = 1.0;
}

ENDCG
}
 
FallBack "Diffuse"
}
 