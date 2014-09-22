Shader "Custom/ambient" {

Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	//Pass {
		//Name "BASE"
		Pass {
			Name "BASE"
			Tags { "RenderType"="Opaque" }
			LOD 200
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	float2 uv : TEXCOORD0;
	        	
	        };
	        
	        uniform sampler2D _MainTex;
	        
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				//return mul(UNITY_MATRIX_MVP, vertexPos);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				return tex2D(_MainTex, i.uv);
				
			}
			 
	        
	        
	        ENDCG
			
		}
		
		
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
	//}
}



}
