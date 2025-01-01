Shader "Hidden/Vol/Vhs" {
	Properties {
		_Intensity ("Intensity", Range(0, 1)) = 1
		_Rocking ("Rocking", Range(0, 0.1)) = 0.01
		_VhsTex ("VhsTex", 2D) = "white" {}
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
}