// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

struct appdata_color {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

struct line_v2f {
	half4 col : COLOR;
	float2 normal : TEXCOORD0;
	float4 screenPos : TEXCOORD1;
	float4 originScreenPos : TEXCOORD2;
};

// d = normalized distance to line
float lineAA(float d) {
	d = max(min(d, 1.0), 0) * 1.116;
	float v = 0.93124*d*d*d - 1.42215*d*d - 0.42715*d + 0.95316;
	v /= 0.95316;
	return max(v, 0);
}

line_v2f line_vert (appdata_color v, float pixelWidth, out float4 outpos : SV_POSITION) {
	line_v2f o;
	// UnityObjectToClipPos only exists in Unity 5.4 or above and there it has to be used
#if defined(UNITY_USE_PREMULTIPLIED_MATRICES)
	float4 p1 = UnityObjectToClipPos(v.vertex);
	float4 p2 = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal*0.0001, v.vertex.w));
#else
	float4 p1 = UnityObjectToClipPos(v.vertex);
    float4 p2 = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal*0.0001, v.vertex.w));
#endif

	float4 p1s = p1/p1.w;
	float4 p2s = p2/p2.w;
	float4 delta = p2s - p1s;

	// Handle DirectX properly. See https://docs.unity3d.com/Manual/SL-PlatformDifferences.html
	float2 screenSpaceNormal = float2(-delta.y, delta.x) * _ProjectionParams.x;
	float2 normalizedScreenSpaceNormal = normalize(screenSpaceNormal);
	screenSpaceNormal = normalizedScreenSpaceNormal / _ScreenParams.xy;
	float4 sn = float4(screenSpaceNormal.x, screenSpaceNormal.y, 0, 0);
	
	if (p1.w < 0) {
		// Seems to have a very minor effect, but the distance
		// seems to be more accurate with this enabled
		sn *= -1;
	}
	
	float side = (v.uv.x - 0.5)*2;
	outpos = p1s + side*sn*pixelWidth*0.5;
	// Multiply by w because homogeneous coordinates (it still needs to be clipped)
	outpos *= p1.w;
	o.normal = normalizedScreenSpaceNormal;
	o.originScreenPos = ComputeScreenPos(p1);
	o.screenPos = ComputeScreenPos(outpos);
	return o;
}

/** Copied from UnityCG.cginc because this function does not exist in Unity 5.2 */ 
inline bool IsGammaSpaceCompatibility() {
#if defined(UNITY_NO_LINEAR_COLORSPACE)
	return true;
#else
	// unity_ColorSpaceLuminance.w == 1 when in Linear space, otherwise == 0
	return unity_ColorSpaceLuminance.w == 0;
#endif
}