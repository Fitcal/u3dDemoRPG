Shader "Custom/ambient" {

Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;
fixed4 _Ambient;


struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Emission = c.rgb * _Ambient.rgb;
}
ENDCG
}



}
