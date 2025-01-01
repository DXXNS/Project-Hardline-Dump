using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FlatKit
{
	public class FlatKitFog : ScriptableRendererFeature
	{
		private class EffectPass : ScriptableRenderPass
		{
			private ScriptableRenderer _renderer;

			private RenderTargetHandle _destination;

			private readonly Material _effectMaterial;

			private RenderTargetHandle _temporaryColorTexture;

			public void Setup(ScriptableRenderer renderer, RenderTargetHandle dst)
			{
				_renderer = renderer;
				_destination = dst;
			}

			public EffectPass(Material effectMaterial)
			{
				_effectMaterial = effectMaterial;
			}

			public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer commandBuffer = CommandBufferPool.Get("FlatKitFogPass");
				RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
				cameraTargetDescriptor.depthBufferBits = 0;
				if (_destination == RenderTargetHandle.CameraTarget)
				{
					commandBuffer.GetTemporaryRT(_temporaryColorTexture.id, cameraTargetDescriptor, FilterMode.Point);
					Blit(commandBuffer, _renderer.cameraColorTarget, _temporaryColorTexture.Identifier(), _effectMaterial);
					Blit(commandBuffer, _temporaryColorTexture.Identifier(), _renderer.cameraColorTarget);
				}
				else
				{
					Blit(commandBuffer, _renderer.cameraColorTarget, _destination.Identifier(), _effectMaterial);
				}
				context.ExecuteCommandBuffer(commandBuffer);
				CommandBufferPool.Release(commandBuffer);
			}

			public override void FrameCleanup(CommandBuffer cmd)
			{
				if (_destination == RenderTargetHandle.CameraTarget)
				{
					cmd.ReleaseTemporaryRT(_temporaryColorTexture.id);
				}
			}
		}

		[Header("Create > FlatKit > Fog Settings")]
		public FogSettings settings;

		private Material _material;

		private EffectPass _effectPass;

		private RenderTargetHandle _effectTexture;

		private Texture2D _lutDepth;

		private Texture2D _lutHeight;

		private static readonly string ShaderName = "Hidden/FlatKit/FogFilter";

		private static readonly int DistanceLut = Shader.PropertyToID("_DistanceLUT");

		private static readonly int Near = Shader.PropertyToID("_Near");

		private static readonly int Far = Shader.PropertyToID("_Far");

		private static readonly int UseDistanceFog = Shader.PropertyToID("_UseDistanceFog");

		private static readonly int UseDistanceFogOnSky = Shader.PropertyToID("_UseDistanceFogOnSky");

		private static readonly int DistanceFogIntensity = Shader.PropertyToID("_DistanceFogIntensity");

		private static readonly int HeightLut = Shader.PropertyToID("_HeightLUT");

		private static readonly int LowWorldY = Shader.PropertyToID("_LowWorldY");

		private static readonly int HighWorldY = Shader.PropertyToID("_HighWorldY");

		private static readonly int UseHeightFog = Shader.PropertyToID("_UseHeightFog");

		private static readonly int UseHeightFogOnSky = Shader.PropertyToID("_UseHeightFogOnSky");

		private static readonly int HeightFogIntensity = Shader.PropertyToID("_HeightFogIntensity");

		private static readonly int DistanceHeightBlend = Shader.PropertyToID("_DistanceHeightBlend");

		public override void Create()
		{
			if (settings == null)
			{
				Debug.LogWarning("[FlatKit] Missing Fog Settings");
				return;
			}
			InitMaterial();
			_effectPass = new EffectPass(_material)
			{
				renderPassEvent = RenderPassEvent.AfterRenderingTransparents
			};
			_effectTexture.Init("_FogTexture");
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (settings == null)
			{
				Debug.LogWarning("[FlatKit] Missing Fog Settings");
				return;
			}
			InitMaterial();
			_effectPass.Setup(renderer, RenderTargetHandle.CameraTarget);
			renderer.EnqueuePass(_effectPass);
		}

		private void InitMaterial()
		{
			if (_material == null)
			{
				Shader shader = Shader.Find(ShaderName);
				if (shader == null)
				{
					return;
				}
				_material = new Material(shader);
			}
			if (_material == null)
			{
				Debug.LogWarning("[FlatKit] Missing Fog Material");
			}
			UpdateShader();
		}

		private void UpdateShader()
		{
			if (!(_material == null))
			{
				UpdateDistanceLut();
				_material.SetTexture(DistanceLut, _lutDepth);
				_material.SetFloat(Near, settings.near);
				_material.SetFloat(Far, settings.far);
				_material.SetFloat(UseDistanceFog, settings.useDistance ? 1f : 0f);
				_material.SetFloat(UseDistanceFogOnSky, settings.useDistanceFogOnSky ? 1f : 0f);
				_material.SetFloat(DistanceFogIntensity, settings.distanceFogIntensity);
				UpdateHeightLut();
				_material.SetTexture(HeightLut, _lutHeight);
				_material.SetFloat(LowWorldY, settings.low);
				_material.SetFloat(HighWorldY, settings.high);
				_material.SetFloat(UseHeightFog, settings.useHeight ? 1f : 0f);
				_material.SetFloat(UseHeightFogOnSky, settings.useHeightFogOnSky ? 1f : 0f);
				_material.SetFloat(HeightFogIntensity, settings.heightFogIntensity);
				_material.SetFloat(DistanceHeightBlend, settings.distanceHeightBlend);
			}
		}

		private void UpdateDistanceLut()
		{
			if (settings.distanceGradient == null)
			{
				return;
			}
			if (_lutDepth != null)
			{
				Object.DestroyImmediate(_lutDepth);
			}
			_lutDepth = new Texture2D(256, 1, TextureFormat.RGBA32, mipChain: false)
			{
				wrapMode = TextureWrapMode.Clamp,
				hideFlags = HideFlags.HideAndDontSave,
				filterMode = FilterMode.Bilinear
			};
			for (float num = 0f; num < 256f; num += 1f)
			{
				Color color = settings.distanceGradient.Evaluate(num / 255f);
				for (float num2 = 0f; num2 < 1f; num2 += 1f)
				{
					_lutDepth.SetPixel(Mathf.CeilToInt(num), Mathf.CeilToInt(num2), color);
				}
			}
			_lutDepth.Apply();
		}

		private void UpdateHeightLut()
		{
			if (settings.heightGradient == null)
			{
				return;
			}
			if (_lutHeight != null)
			{
				Object.DestroyImmediate(_lutHeight);
			}
			_lutHeight = new Texture2D(256, 1, TextureFormat.RGBA32, mipChain: false)
			{
				wrapMode = TextureWrapMode.Clamp,
				hideFlags = HideFlags.HideAndDontSave,
				filterMode = FilterMode.Bilinear
			};
			for (float num = 0f; num < 256f; num += 1f)
			{
				Color color = settings.heightGradient.Evaluate(num / 255f);
				for (float num2 = 0f; num2 < 1f; num2 += 1f)
				{
					_lutHeight.SetPixel(Mathf.CeilToInt(num), Mathf.CeilToInt(num2), color);
				}
			}
			_lutHeight.Apply();
		}
	}
}
