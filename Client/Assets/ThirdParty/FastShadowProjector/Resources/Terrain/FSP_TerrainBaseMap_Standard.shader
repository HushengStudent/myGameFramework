Shader "FastShadowProjector/FSP_Standard-TerrainBaseMap" { 
	Properties {
		_MainTex ("Base (RGB) Smoothness (A)", 2D) = "white" {}
		_MetallicTex ("Metallic (R)", 2D) = "white" {}

		// used in fallback on old cards
		_Color ("Main Color", Color) = (1,1,1,1)
		
		[HideInInspector] _ShadowTex ("Projector Texture", 2D) = "white" {}
		[HideInInspector] _UnityBugHack("UnityBugHack", Float) = 0.1
	}

	SubShader {
		Tags {
	         "RenderType"="Transparent"
	         "IgnoreProjector"="True" 
	         "Queue"="Transparent+1"
		}
		LOD 200
		
		ZWrite Off
        Blend DstColor Zero
		
		CGPROGRAM
		#pragma surface surf Standard vertex:shadowVert finalcolor:finalColorFunc fullforwardshadows
		#pragma target 3.0
		// needs more than 8 texcoords
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;
		sampler2D _MetallicTex;

		sampler2D _ShadowTex;
		float4x4 _GlobalProjector;
		float _Random;
     
		struct Input {
          float4 _ShadowTexUV;
        };
        
		void surf (Input IN, inout SurfaceOutputStandard o) {
		}
		
		void finalColorFunc (Input IN, SurfaceOutputStandard o, inout fixed4 color)
      	{
			half4 shadowColor = tex2D (_ShadowTex, IN._ShadowTexUV.xy);
			
        	color.rgb = shadowColor.rgb;
      		color.a = shadowColor.a;
      	}
		
		void shadowVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data._ShadowTexUV = mul(_GlobalProjector, v.vertex);
		}

		ENDCG
	}

	FallBack "Diffuse"
}
