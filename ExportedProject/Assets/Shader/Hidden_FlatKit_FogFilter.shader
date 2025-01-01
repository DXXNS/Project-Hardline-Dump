Shader "Hidden/FlatKit/FogFilter" {
	Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "white" {}
		[Toggle(USE_DISTANCE_FOG)] _UseDistanceFog ("Use Distance", Float) = 0
		[Toggle(USE_DISTANCE_FOG_ON_SKY)] _UseDistanceFogOnSky ("Use Distance Fog On Sky", Float) = 0
		[Space] _Near ("Near", Float) = 0
		_Far ("Far", Float) = 100
		[Space] _DistanceFogIntensity ("Distance Fog Intensity", Range(0, 1)) = 1
		[Space(25)] [Toggle(USE_HEGHT_FOG)] _UseHeightFog ("Use Height", Float) = 0
		[Toggle(USE_HEGHT_FOG_ON_SKY)] _UseHeightFogOnSky ("Use Height Fog On Sky", Float) = 0
		[Space] _LowWorldY ("Low", Float) = 0
		_HighWorldY ("High", Float) = 10
		[Space] _HeightFogIntensity ("Height Fog Intensity", Range(0, 1)) = 1
		[Space(25)] _DistanceHeightBlend ("Distance / Height blend", Range(0, 1)) = 0.5
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}