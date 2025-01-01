Shader "SyntyStudios/PolygonTrees_LOD" {
	Properties {
		Vector1_CE098D07 ("Wind Density", Float) = 0.1
		Vector1_C1D16DBE ("Wind Strength", Float) = 0.5
		Vector1_EBB5CF27 ("Wind Speed", Float) = 1
		[NoScaleOffset] Texture2D_CAD82441 ("Base Material", 2D) = "white" {}
		Vector1_AADD838F ("Small Wind Density", Float) = 10
		Vector1_C39D93FF ("Small Wind Speed", Float) = 0.2
		Vector1_7722A149 ("Small Wind Strength", Float) = 0.05
		[NoScaleOffset] Texture2D_38206155 ("Emissive Mask", 2D) = "white" {}
		Color_FA85148A ("Emissive Color", Vector) = (0.2660625,1,0,0)
		Color_369F793F ("LeafColourTint", Vector) = (1,1,1,0)
		[NoScaleOffset] Texture2D_896AB8C ("ColourLodMask", 2D) = "white" {}
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
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
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}