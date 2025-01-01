using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.PostProcessing;

namespace CompoundRendererFeature.PostProcess
{
	[CompoundRendererFeature("Stylized Detail", InjectionPoint.BeforePostProcess, false)]
	public class StylizedDetailRenderer : CompoundRenderer
	{
		private static class PropertyIDs
		{
			internal static readonly int Input = Shader.PropertyToID("_MainTex");

			internal static readonly int PingTexture = Shader.PropertyToID("_PingTexture");

			internal static readonly int BlurStrength = Shader.PropertyToID("_BlurStrength");

			internal static readonly int Blur1 = Shader.PropertyToID("_BlurTex1");

			internal static readonly int Blur2 = Shader.PropertyToID("_BlurTex2");

			internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

			internal static readonly int DownSampleScaleFactor = Shader.PropertyToID("_DownSampleScaleFactor");

			public static readonly int CoCParams = Shader.PropertyToID("_CoCParams");
		}

		private StylizedDetail _volumeComponent;

		private Material _effectMaterial;

		public override bool visibleInSceneView => false;

		public override ScriptableRenderPassInput input => ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Color;

		public override void Initialize()
		{
			base.Initialize();
			_effectMaterial = CoreUtils.CreateEngineMaterial("Hidden/CompoundRendererFeature/StylizedDetail");
		}

		public override bool Setup(in RenderingData renderingData, InjectionPoint injectionPoint)
		{
			base.Setup(in renderingData, injectionPoint);
			VolumeStack stack = VolumeManager.instance.stack;
			_volumeComponent = stack.GetComponent<StylizedDetail>();
			return _volumeComponent.intensity.value > 0f;
		}

		public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, ref RenderingData renderingData, InjectionPoint injectionPoint)
		{
			RenderTextureDescriptor tempRTDescriptor = CompoundRenderer.GetTempRTDescriptor(in renderingData);
			int num = tempRTDescriptor.width / 1;
			int height = tempRTDescriptor.height / 1;
			float a = _volumeComponent.blur.value * ((float)num / 1080f);
			a = Mathf.Min(a, 2f);
			float a2 = _volumeComponent.edgePreserve.value * ((float)num / 1080f);
			a2 = Mathf.Min(a2, 2f);
			float x = (_volumeComponent.rangeStart.overrideState ? _volumeComponent.rangeStart.value : 0f);
			float y = (_volumeComponent.rangeEnd.overrideState ? _volumeComponent.rangeEnd.value : (-1f));
			_effectMaterial.SetVector(PropertyIDs.CoCParams, new Vector2(x, y));
			_effectMaterial.SetFloat(PropertyIDs.Intensity, _volumeComponent.intensity.value);
			CompoundRenderer.SetSourceSize(cmd, tempRTDescriptor);
			RenderTextureDescriptor tempRTDescriptor2 = CompoundRenderer.GetTempRTDescriptor(in renderingData, num, height, _defaultHDRFormat);
			cmd.GetTemporaryRT(PropertyIDs.PingTexture, tempRTDescriptor2, FilterMode.Bilinear);
			cmd.GetTemporaryRT(PropertyIDs.Blur1, tempRTDescriptor2, FilterMode.Bilinear);
			cmd.GetTemporaryRT(PropertyIDs.Blur2, tempRTDescriptor2, FilterMode.Bilinear);
			cmd.SetGlobalVector(PropertyIDs.DownSampleScaleFactor, new Vector4(1f, 1f, 1f, 1f));
			cmd.SetGlobalFloat(PropertyIDs.BlurStrength, a2);
			cmd.SetGlobalTexture(PropertyIDs.Input, source);
			CoreUtils.DrawFullScreen(cmd, _effectMaterial, PropertyIDs.PingTexture, null, 1);
			cmd.SetGlobalTexture(PropertyIDs.Input, PropertyIDs.PingTexture);
			CoreUtils.DrawFullScreen(cmd, _effectMaterial, PropertyIDs.Blur1, null, 2);
			cmd.SetGlobalFloat(PropertyIDs.BlurStrength, a);
			cmd.SetGlobalTexture(PropertyIDs.Input, PropertyIDs.Blur1);
			CoreUtils.DrawFullScreen(cmd, _effectMaterial, PropertyIDs.PingTexture, null, 1);
			cmd.SetGlobalTexture(PropertyIDs.Input, PropertyIDs.PingTexture);
			CoreUtils.DrawFullScreen(cmd, _effectMaterial, PropertyIDs.Blur2, null, 2);
			cmd.SetGlobalTexture(PropertyIDs.Input, source);
			CoreUtils.DrawFullScreen(cmd, _effectMaterial, destination);
			cmd.ReleaseTemporaryRT(PropertyIDs.PingTexture);
			cmd.ReleaseTemporaryRT(PropertyIDs.Blur1);
			cmd.ReleaseTemporaryRT(PropertyIDs.Blur2);
		}

		public override void Dispose(bool disposing)
		{
		}
	}
}
