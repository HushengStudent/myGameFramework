Shader "FastShadowProjector/BlobShadowMultiply" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend Zero SrcColor, One Zero
	Cull Off Lighting Off ZWrite Off Fog { Color (1,1,1,1) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
			SetTexture [_MainTex] {
				constantColor (1,1,1,0)
				combine previous lerp (previous) constant
			}
		}
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor (1,1,1,0)
				combine texture lerp(texture) constant
			}
		}
	}
}
}
