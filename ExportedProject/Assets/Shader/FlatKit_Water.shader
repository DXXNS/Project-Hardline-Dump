Shader "FlatKit/Water" {
	Properties {
		[Header(Colors)] [Space] [KeywordEnum(Linear, Gradient Texture)] _ColorMode ("     Source{Colors}", Float) = 0
		_ColorShallow ("[_COLORMODE_LINEAR]     Shallow", Vector) = (0.35,0.6,0.75,0.8)
		_ColorDeep ("[_COLORMODE_LINEAR]     Deep", Vector) = (0.65,0.9,1,1)
		[NoScaleOffset] _ColorGradient ("[_COLORMODE_GRADIENT_TEXTURE]     Gradient", 2D) = "white" {}
		_FadeDistance ("     Shallow depth", Float) = 0.5
		_WaterDepth ("     Gradient size", Float) = 5
		[Space] _WaterClearness ("     Transparency", Range(0, 1)) = 0.3
		_ShadowStrength ("     Shadow strength", Range(0, 1)) = 0.35
		[Header(Crest)] [Space] _CrestColor ("     Color{Crest}", Vector) = (1,1,1,0.9)
		_CrestSize ("     Size{Crest}", Range(0, 1)) = 0.1
		_CrestSharpness ("     Sharp transition{Crest}", Range(0, 1)) = 0.1
		[Space] [Header(Wave geometry)] [Space] [KeywordEnum(None, Round, Grid, Pointy)] _WaveMode ("     Shape{Wave}", Float) = 1
		_WaveSpeed ("[!_WAVEMODE_NONE]     Speed{Wave}", Float) = 0.5
		_WaveAmplitude ("[!_WAVEMODE_NONE]     Amplitude{Wave}", Float) = 0.25
		_WaveFrequency ("[!_WAVEMODE_NONE]     Frequency{Wave}", Float) = 1
		_WaveDirection ("[!_WAVEMODE_NONE]     Direction{Wave}", Range(-1, 1)) = 0
		_WaveNoise ("[!_WAVEMODE_NONE]     Noise{Wave}", Range(0, 1)) = 0.25
		[Space] [Header(Foam)] [Space] [KeywordEnum(None, Gradient Noise, Texture)] _FoamMode ("     Source{Foam}", Float) = 1
		[NoScaleOffset] _NoiseMap ("[_FOAMMODE_TEXTURE]           Texture{Foam}", 2D) = "white" {}
		_FoamColor ("[!_FOAMMODE_NONE]     Color{Foam}", Vector) = (1,1,1,1)
		[Space] _FoamDepth ("[!_FOAMMODE_NONE]     Shore Depth{Foam}", Float) = 0.5
		_FoamNoiseAmount ("[!_FOAMMODE_NONE]     Shore Blending{Foam}", Range(0, 1)) = 1
		[Space] _FoamAmount ("[!_FOAMMODE_NONE]     Amount{Foam}", Range(0, 3)) = 0.25
		[Space] _FoamScale ("[!_FOAMMODE_NONE]     Scale{Foam}", Range(0, 3)) = 1
		_FoamStretchX ("[!_FOAMMODE_NONE]     Stretch X{Foam}", Range(0, 10)) = 1
		_FoamStretchY ("[!_FOAMMODE_NONE]     Stretch Y{Foam}", Range(0, 10)) = 1
		[Space] _FoamSharpness ("[!_FOAMMODE_NONE]     Sharpness{Foam}", Range(0, 1)) = 0.5
		[Space] _FoamSpeed ("[!_FOAMMODE_NONE]     Speed{Foam}", Float) = 0.1
		_FoamDirection ("[!_FOAMMODE_NONE]     Direction{Foam}", Range(-1, 1)) = 0
		[Space] [Header(Refraction)] [Space] _RefractionFrequency ("     Frequency", Float) = 35
		_RefractionAmplitude ("     Amplitude", Range(0, 0.1)) = 0.01
		_RefractionSpeed ("     Speed", Float) = 0.1
		_RefractionScale ("     Scale", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	//CustomEditor "FlatKitWaterEditor"
}