﻿Shader "Custom/shadowShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)
		
		_ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
		
	}
	
	SubShader {
		/*
		Pass {
			Tags {"RenderType"="Opaque"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			uniform fixed4 _Color;
			float4 vert(float4 vertexPos : POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, vertexPos);
			}
			float4 frag(void) : COLOR
			{
				return _Color;
			}
			ENDCG
		}
		*/
		
		UsePass "Custom/ambient/BASE"
		
		
		Pass {
			Tags {"RenderType"="Opaque"}
			
			Offset -1.0, -2.0
			CGPROGRAM
			#pragma vertex vert 
	         #pragma fragment frag
	 
	         #include "UnityCG.cginc"
	 
	         // User-specified uniforms
	         uniform float4 _ShadowColor;
	         uniform float4x4 _World2Receiver; // transformation from 
	         uniform float4 _LightDir;
	         
	            // world coordinates to the coordinate system of the plane
	 
	         float4 vert(float4 vertexPos : POSITION) : SV_POSITION
	         {
	            float4x4 modelMatrix = _Object2World;
	            float4x4 modelMatrixInverse = 
	               _World2Object * unity_Scale.w;
	            modelMatrixInverse[3][3] = 1.0; 
	            float4x4 viewMatrix = 
	               mul(UNITY_MATRIX_MV, modelMatrixInverse);
	 
	            float4 lightDirection = _LightDir;
	            lightDirection = normalize(lightDirection);
	            
	            /*
	            if (0.0 != _WorldSpaceLightPos0.w) 
	            {
	               // point or spot light
	               lightDirection = normalize(
	                  mul(modelMatrix, vertexPos - _WorldSpaceLightPos0));
	            } 
	            else 
	            {
	               // directional light
	               lightDirection = -normalize(_WorldSpaceLightPos0); 
	            }
	            */
	 
	            float4 vertexInWorldSpace = mul(modelMatrix, vertexPos);
	            
	         
	            //float4 world2ReceiverRow1 = 
	            //   float4(_World2Receiver[0][1], _World2Receiver[1][1], 
	            //   _World2Receiver[2][1], _World2Receiver[3][1]);
	           	
	           	float4 world2ReceiverRow1 = 
	               float4(_World2Receiver[1][0], _World2Receiver[1][1], 
	               _World2Receiver[1][2], _World2Receiver[1][3]);
	           
	              
	            //float4 world2ReceiverRow1 = float4(-2, 5, 0, 0);
	            
	            float distanceOfVertex = 
	               dot(world2ReceiverRow1, vertexInWorldSpace); 
	               //(_World2Receiver * vertexInWorldSpace).y;
	               //mul(_World2Receiver, vertexInWorldSpace).y;
	                
	               // = height over plane 
	            
	            float lengthOfLightDirectionInY = 
	               dot(world2ReceiverRow1, lightDirection); 
	               //(_World2Receiver * lightDirection).y ;
	               //mul(_World2Receiver, lightDirection).y;
	               // = length in y direction
	 
	            if (distanceOfVertex > 0.0 && lengthOfLightDirectionInY < 0.0)
	            {
	               lightDirection = lightDirection 
	                  * (distanceOfVertex / (-lengthOfLightDirectionInY));
	            }
	            else
	            {
	               lightDirection = float4(0.0, 0.0, 0.0, 0.0); 
	                  // don't move vertex
	            }
	 
	            return mul(UNITY_MATRIX_P, mul(viewMatrix, 
	               vertexInWorldSpace + lightDirection));
	         	//return mul(UNITY_MATRIX_P, mul(viewMatrix, vertexInWorldSpace));
	         	
	         }
	 
	         float4 frag(void) : COLOR 
	         {
	            return _ShadowColor;
	         }
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
