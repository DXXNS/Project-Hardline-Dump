using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.PostProcessing;

namespace CompoundRendererFeature.PostProcess
{
	[CompoundRendererFeature("Stylized Color Grading", InjectionPoint.BeforePostProcess, false)]
	public class ColorGradingRenderer : CompoundRenderer
	{
		private static class PropertyIDs
		{
			internal static readonly int Input = Shader.PropertyToID("_MainTex");

			internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

			internal static readonly int ShadowBezierPoints = Shader.PropertyToID("_ShadowBezierPoints");

			internal static readonly int HighlightBezierPoints = Shader.PropertyToID("_HighlightBezierPoints");

			internal static readonly int Contrast = Shader.PropertyToID("_Contrast");

			internal static readonly int Vibrance = Shader.PropertyToID("_Vibrance");

			internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
		}

		private ColorGrading _volumeComponent;

		private Material _effectMaterial;

		public override bool visibleInSceneView => false;

		public override ScriptableRenderPassInput input => ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Color;

		public override void Initialize()
		{
			base.Initialize();
			_effectMaterial = CoreUtils.CreateEngineMaterial("Hidden/CompoundRendererFeature/ColorGrading");
		}

		public override bool Setup(in RenderingData renderingData, InjectionPoint injectionPoint)
		{
			base.Setup(in renderingData, injectionPoint);
			VolumeStack stack = VolumeManager.instance.stack;
			_volumeComponent = stack.GetComponent<ColorGrading>();
			return _volumeComponent.intensity.value > 0f;
		}

		public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, ref RenderingData renderingData, InjectionPoint injectionPoint)
		{
			RenderTextureDescriptor tempRTDescriptor = CompoundRenderer.GetTempRTDescriptor(in renderingData);
			_effectMaterial.SetFloat(PropertyIDs.Intensity, _volumeComponent.intensity.value);
			_effectMaterial.SetVector(PropertyIDs.ShadowBezierPoints, new Vector4(_volumeComponent.blueShadows.value, _volumeComponent.greenShadows.value));
			_effectMaterial.SetVector(PropertyIDs.HighlightBezierPoints, new Vector4(_volumeComponent.redHighlights.value, 0f, 0f, 0f));
			_effectMaterial.SetFloat(PropertyIDs.Contrast, _volumeComponent.contrast.value);
			_effectMaterial.SetFloat(PropertyIDs.Vibrance, _volumeComponent.vibrance.value * 0.5f);
			_effectMaterial.SetFloat(PropertyIDs.Saturation, _volumeComponent.saturation.value * 0.5f);
			CompoundRenderer.SetSourceSize(cmd, tempRTDescriptor);
			cmd.SetGlobalTexture(PropertyIDs.Input, source);
			CoreUtils.DrawFullScreen(cmd, _effectMaterial, destination);
		}

		public override void Dispose(bool disposing)
		{
		}
	}
}
