Shader "FastShadowProjector/BlobShadowAdd" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (1,1,1,1) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

	SubShader {
		Pass {
			Color [color]
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}
