Shader "FastShadowProjector/FSP_TerrainAddPass" { 
	Properties { 
		// Keeping these to make terrain happy. Please DO NOT remove these properties
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	 	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	 	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	 	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
		_MainTex ("BaseMap (RGB)", 2D) = "white" {}
		
		_ShadowTex ("Projector Texture", 2D) = "gray"
	} 
	
	Subshader {
	  Tags { "SplatCount" = "4"
	         "RenderType"="Transparent"
	         "IgnoreProjector"="True" 
	         "Queue"="Geometry-97" 
	         }
	  Pass {
		 ZWrite Off
		 Cull Front
		 ZTest LEqual
		 ColorMask 0 
	   }
	}
	
	Fallback off
}