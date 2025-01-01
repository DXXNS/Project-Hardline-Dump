Shader "SyntyStudios/PolygonNatureWater" {
	Properties {
		[HDR] _shallowColor ("Shallow Color", Vector) = (0.07421358,0.1529262,0.1470273,1)
		[HDR] _deepColor ("Deep Color", Vector) = (0.01938236,0.06480328,0.07036011,1)
		_waterDepth ("Water Depth", Range(0, 20)) = 9.91
		_waterSmoothness ("Water Smoothness", Float) = 0
		[HDR] _edgeFoamColor ("Edge Foam Color", Vector) = (0.7454045,0.7454045,0.7454045,1)
		_edgeFoamAmount ("Edge Foam Amount", Float) = 5
		_edgeFoamCutoff ("Edge Foam Cutoff", Float) = 4.8
		_fakeSpecular ("Specular", Float) = 0.3
		[NoScaleOffset] Texture2D_CFD6D642 ("Normal", 2D) = "white" {}
		Vector1_989A1C37 ("Opacity", Float) = 0.5
		[NoScaleOffset] _Texture2DAsset_65bdf33da96f7189aea056d97a11186e_Out_0_Texture2D ("Texture2D", 2D) = "white" {}
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