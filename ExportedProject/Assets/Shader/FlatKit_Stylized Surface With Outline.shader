Shader "FlatKit/Stylized Surface With Outline" {
	Properties {
		_BaseColor ("Color", Vector) = (1,1,1,1)
		[Space(10)] [KeywordEnum(None, Single, Steps, Curve)] _CelPrimaryMode ("Cel Shading Mode", Float) = 1
		_ColorDim ("[_CELPRIMARYMODE_SINGLE]Color Shaded", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_ColorDimSteps ("[_CELPRIMARYMODE_STEPS]Color Shaded", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_ColorDimCurve ("[_CELPRIMARYMODE_CURVE]Color Shaded", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_SelfShadingSize ("[_CELPRIMARYMODE_SINGLE]Self Shading Size", Range(0, 1)) = 0.5
		_ShadowEdgeSize ("[_CELPRIMARYMODE_SINGLE]Shadow Edge Size", Range(0, 0.5)) = 0.05
		_Flatness ("[_CELPRIMARYMODE_SINGLE]Localized Shading", Range(0, 1)) = 1
		[IntRange] _CelNumSteps ("[_CELPRIMARYMODE_STEPS]Number Of Steps", Range(1, 10)) = 3
		_CelStepTexture ("[_CELPRIMARYMODE_STEPS][LAST_PROP_STEPS]Cel steps", 2D) = "black" {}
		_CelCurveTexture ("[_CELPRIMARYMODE_CURVE][LAST_PROP_CURVE]Ramp", 2D) = "black" {}
		[Space(10)] [Toggle(DR_CEL_EXTRA_ON)] _CelExtraEnabled ("Enable Extra Cel Layer", Float) = 0
		_ColorDimExtra ("[DR_CEL_EXTRA_ON]Color Shaded", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_SelfShadingSizeExtra ("[DR_CEL_EXTRA_ON]Self Shading Size", Range(0, 1)) = 0.6
		_ShadowEdgeSizeExtra ("[DR_CEL_EXTRA_ON]Shadow Edge Size", Range(0, 0.5)) = 0.05
		_FlatnessExtra ("[DR_CEL_EXTRA_ON]Localized Shading", Range(0, 1)) = 1
		[Space(10)] [Toggle(DR_SPECULAR_ON)] _SpecularEnabled ("Enable Specular", Float) = 0
		[HDR] _FlatSpecularColor ("[DR_SPECULAR_ON]Specular Color", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_FlatSpecularSize ("[DR_SPECULAR_ON]Specular Size", Range(0, 1)) = 0.1
		_FlatSpecularEdgeSmoothness ("[DR_SPECULAR_ON]Specular Edge Smoothness", Range(0, 1)) = 0
		[Space(10)] [Toggle(DR_RIM_ON)] _RimEnabled ("Enable Rim", Float) = 0
		[HDR] _FlatRimColor ("[DR_RIM_ON]Rim Color", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_FlatRimLightAlign ("[DR_RIM_ON]Light Align", Range(0, 1)) = 0
		_FlatRimSize ("[DR_RIM_ON]Rim Size", Range(0, 1)) = 0.5
		_FlatRimEdgeSmoothness ("[DR_RIM_ON]Rim Edge Smoothness", Range(0, 1)) = 0.5
		[Space(10)] [Toggle(DR_GRADIENT_ON)] _GradientEnabled ("Enable Height Gradient", Float) = 0
		[HDR] _ColorGradient ("[DR_GRADIENT_ON]Gradient Color", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_GradientCenterX ("[DR_GRADIENT_ON]Center X", Float) = 0
		_GradientCenterY ("[DR_GRADIENT_ON]Center Y", Float) = 0
		_GradientSize ("[DR_GRADIENT_ON]Size", Float) = 10
		_GradientAngle ("[DR_GRADIENT_ON]Gradient Angle", Range(0, 360)) = 0
		[Space(10)] [Toggle(DR_VERTEX_COLORS_ON)] _VertexColorsEnabled ("Enable Vertex Colors", Float) = 0
		_LightContribution ("[FOLDOUT(Advanced Lighting){5}]Light Color Contribution", Range(0, 1)) = 0
		_LightFalloffSize ("Falloff size (point / spot)", Range(0.0001, 1)) = 0.0001
		[Space(5)] [Toggle(DR_ENABLE_LIGHTMAP_DIR)] _OverrideLightmapDir ("Override light direction", Float) = 0
		_LightmapDirectionPitch ("[DR_ENABLE_LIGHTMAP_DIR]Pitch", Range(0, 360)) = 0
		_LightmapDirectionYaw ("[DR_ENABLE_LIGHTMAP_DIR]Yaw", Range(0, 360)) = 0
		[HideInInspector] _LightmapDirection ("Direction", Vector) = (0,1,0,0)
		[KeywordEnum(None, Multiply, Color)] _UnityShadowMode ("[FOLDOUT(Unity Built-in Shadows){4}]Mode", Float) = 0
		_UnityShadowPower ("[_UNITYSHADOWMODE_MULTIPLY]Power", Range(0, 1)) = 0.2
		_UnityShadowColor ("[_UNITYSHADOWMODE_COLOR]Color", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_UnityShadowSharpness ("Sharpness", Range(1, 10)) = 1
		_BaseMap ("[FOLDOUT(Texture maps){4}]Albedo", 2D) = "white" {}
		[Space] [KeywordEnum(Multiply, Add)] _TextureBlendingMode ("[_]Blending Mode", Float) = 0
		[Space] _TextureImpact ("[_]Texture Impact", Range(0, 1)) = 1
		[Space(20)] _BumpMap ("Bump Map", 2D) = "bump" {}
		[HideInInspector] _Surface ("__surface", Float) = 0
		[HideInInspector] _Blend ("__blend", Float) = 0
		[HideInInspector] _AlphaClip ("__clip", Float) = 0
		[HideInInspector] _SrcBlend ("__src", Float) = 1
		[HideInInspector] _DstBlend ("__dst", Float) = 0
		[HideInInspector] _ZWrite ("__zw", Float) = 1
		[HideInInspector] _Cull ("__cull", Float) = 2
		[HideInInspector] _QueueOffset ("Queue offset", Float) = 0
		[HideInInspector] _MainTex ("BaseMap", 2D) = "white" {}
		[HideInInspector] _Color ("Base Color", Vector) = (1,1,1,1)
		[Space(25)] _OutlineColor ("[FOLDOUT(Outline){4}]Outline Color", Vector) = (0.85023,0.85034,0.85045,0.85056)
		_OutlineWidth ("Outline Width", Float) = 0.01
		_OutlineDepthOffset ("Outline Depth Offset", Range(0, 1)) = 0
		_CameraDistanceImpact ("Camera Distance Impact", Range(0, 1)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "FlatKit/Stylized Surface"
	//CustomEditor "StylizedSurfaceEditor"
}