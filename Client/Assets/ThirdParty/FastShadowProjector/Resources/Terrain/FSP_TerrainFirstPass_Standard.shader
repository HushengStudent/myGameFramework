Shader "FastShadowProjector/FSP_Standard-TerrainFirstPass" {
	Properties {
		// set by terrain engine
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
	// DO NOT UNCOMMENT THESE !
	//	[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
	//  [HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
	//	[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
	//  [HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		[HideInInspector] [Gamma] _Metallic0 ("Metallic 0", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic1 ("Metallic 1", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic2 ("Metallic 2", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic3 ("Metallic 3", Range(0.0, 1.0)) = 0.0
		[HideInInspector] _Smoothness0 ("Smoothness 0", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness1 ("Smoothness 1", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness2 ("Smoothness 2", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness3 ("Smoothness 3", Range(0.0, 1.0)) = 1.0

		// used in fallback on old cards & base map
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
		
		[HideInInspector] _ShadowTex ("Projector Texture", 2D) = "white" {}
		
		[HideInInspector] _UnityBugHack("UnityBugHack", Float) = 0.1
	}

	SubShader {
		Tags {
			"SplatCount" = "4"
			"Queue" = "Geometry-98"
			"RenderType" = "Transparent"
		}
		
		Blend DstColor Zero
		ZWrite Off
		
		CGPROGRAM
		// As we can't blend normals in g-buffer, this shader will not work in standard deferred path. 
		// So we use exclude_path:deferred to force it to only use the forward path.
		#pragma surface surf Standard exclude_path:prepass vertex:shadowVert finalcolor:finalColorFunc exclude_path:deferred
		#pragma multi_compile_fog
		#pragma target 3.0
		// needs more than 8 texcoords
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		#pragma multi_compile __ _TERRAIN_NORMAL_MAP

		#define TERRAIN_STANDARD_SHADER

		half _Metallic0;
		half _Metallic1;
		half _Metallic2;
		half _Metallic3;
		
		half _Smoothness0;
		half _Smoothness1;
		half _Smoothness2;
		half _Smoothness3;
		
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

	Dependency "AddPassShader" = "FastShadowProjector/FSP_Standard-TerrainAddPass"
	Dependency "BaseMapShader" = "FastShadowProjector/FSP_Standard-TerrainBaseMap"

	Fallback "FastShadowProjector/FSP_TerrainFirstPass"
}
