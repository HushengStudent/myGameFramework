// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FastShadowProjector/FSP_TerrainBaseMap" { 
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
	  Tags {"SplatCount" = "4"
	         "RenderType"="Transparent"
	         "IgnoreProjector"="True" 
	         "Queue"="Transparent+1"
	         }
	 Pass
         {
             ZWrite Off
             ColorMask RGBA
             Blend DstColor Zero
			 Offset -1, -1
			 Fog { Color (0, 0, 0) }
                          
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
   
             #include "UnityCG.cginc"
              
             struct v2f
             {
                 float4 pos : SV_POSITION;
                 float4 uv_Main : TEXCOORD0;
                 float4 uv_MainClip : TEXCOORD1;
             };
             
              
             sampler2D _ShadowTex;
             float4x4 _GlobalProjector;
             float4x4 _GlobalProjectorClip;
              
             v2f vert(appdata_tan v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos (v.vertex);
                 o.uv_Main = mul (_GlobalProjector, v.vertex);
                 o.uv_MainClip = mul (_GlobalProjectorClip, v.vertex);
                 return o;
             }
              
             half4 frag (v2f i) : COLOR
             {
                 half4 tex = half4(i.uv_Main.w, i.uv_Main.w, i.uv_Main.w, 1);
                 
                 if (i.uv_MainClip.w < 1.0f) {
               
                 	tex = tex2D(_ShadowTex, i.uv_Main.xy);
                 }
          
                 return tex;
             }
             ENDCG
      
         }
	}
}