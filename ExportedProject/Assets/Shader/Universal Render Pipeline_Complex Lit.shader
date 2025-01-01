Shader "Universal Render Pipeline/Complex Lit" {
	Properties {
		_WorkflowMode ("WorkflowMode", Float) = 1
		_BaseMap ("Albedo", 2D) = "white" {}
		_BaseColor ("Color", Vector) = (1,1,1,1)
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0
		_Metallic ("Metallic", Range(0, 1)) = 0
		_MetallicGlossMap ("Metallic", 2D) = "white" {}
		_SpecColor ("Specular", Vector) = (0.2,0.2,0.2,1)
		_SpecGlossMap ("Specular", 2D) = "white" {}
		[ToggleOff] _SpecularHighlights ("Specular Highlights", Float) = 1
		[ToggleOff] _EnvironmentReflections ("Environment Reflections", Float) = 1
		_BumpScale ("Scale", Float) = 1
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Parallax ("Scale", Range(0.005, 0.08)) = 0.005
		_ParallaxMap ("Height Map", 2D) = "black" {}
		_OcclusionStrength ("Strength", Range(0, 1)) = 1
		_OcclusionMap ("Occlusion", 2D) = "white" {}
		[HDR] _EmissionColor ("Color", Vector) = (0,0,0,1)
		_EmissionMap ("Emission", 2D) = "white" {}
		_DetailMask ("Detail Mask", 2D) = "white" {}
		_DetailAlbedoMapScale ("Scale", Range(0, 2)) = 1
		_DetailAlbedoMap ("Detail Albedo x2", 2D) = "linearGrey" {}
		_DetailNormalMapScale ("Scale", Range(0, 2)) = 1
		[Normal] _DetailNormalMap ("Normal Map", 2D) = "bump" {}
		[ToggleUI] _ClearCoat ("Clear Coat", Float) = 0
		_ClearCoatMap ("Clear Coat Map", 2D) = "white" {}
		_ClearCoatMask ("Clear Coat Mask", Range(0, 1)) = 0
		_ClearCoatSmoothness ("Clear Coat Smoothness", Range(0, 1)) = 1
		_Surface ("__surface", Float) = 0
		_Blend ("__mode", Float) = 0
		_Cull ("__cull", Float) = 2
		[ToggleUI] _AlphaClip ("__clip", Float) = 0
		[HideInInspector] _BlendOp ("__blendop", Float) = 0
		[HideInInspector] _SrcBlend ("__src", Float) = 1
		[HideInInspector] _DstBlend ("__dst", Float) = 0
		[HideInInspector] _SrcBlendAlpha ("__srcA", Float) = 1
		[HideInInspector] _DstBlendAlpha ("__dstA", Float) = 0
		[HideInInspector] _ZWrite ("__zw", Float) = 1
		[HideInInspector] _BlendModePreserveSpecular ("_BlendModePreserveSpecular", Float) = 1
		[HideInInspector] _AlphaToMask ("__alphaToMask", Float) = 0
		[ToggleUI] _ReceiveShadows ("Receive Shadows", Float) = 1
		_QueueOffset ("Queue offset", Float) = 0
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
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
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
	//CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.LitShader"
}