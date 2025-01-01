using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class FlatKitDepthNormals : ScriptableRendererFeature
{
	private class DepthNormalsPass : ScriptableRenderPass
	{
		private readonly int _depthBufferBits = 32;

		private RenderTargetHandle _depthAttachmentHandle;

		private RenderTextureDescriptor _descriptor;

		private readonly Material _depthNormalsMaterial;

		private FilteringSettings _filteringSettings;

		private readonly string _profilerTag = "[Flat Kit] Depth Normals Pass";

		private readonly ShaderTagId _shaderTagId = new ShaderTagId("DepthOnly");

		public DepthNormalsPass(RenderQueueRange renderQueueRange, LayerMask layerMask, Material material)
		{
			_filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
			_depthNormalsMaterial = material;
		}

		public void Setup(RenderTextureDescriptor baseDescriptor, RenderTargetHandle depthAttachmentHandle)
		{
			_depthAttachmentHandle = depthAttachmentHandle;
			baseDescriptor.colorFormat = RenderTextureFormat.ARGB32;
			baseDescriptor.depthBufferBits = _depthBufferBits;
			_descriptor = baseDescriptor;
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			cmd.GetTemporaryRT(_depthAttachmentHandle.id, _descriptor, FilterMode.Point);
			ConfigureTarget(_depthAttachmentHandle.Identifier());
			ConfigureClear(ClearFlag.All, Color.black);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = CommandBufferPool.Get(_profilerTag);
			using (new ProfilingScope(commandBuffer, new ProfilingSampler(_profilerTag)))
			{
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				SortingCriteria defaultOpaqueSortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
				DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagId, ref renderingData, defaultOpaqueSortFlags);
				drawingSettings.perObjectData = PerObjectData.None;
				Camera camera = renderingData.cameraData.camera;
				if (XRSettings.enabled)
				{
					context.StartMultiEye(camera);
				}
				drawingSettings.overrideMaterial = _depthNormalsMaterial;
				context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
				commandBuffer.SetGlobalTexture("_CameraDepthNormalsTexture", _depthAttachmentHandle.id);
			}
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
			if (!(_depthAttachmentHandle == RenderTargetHandle.CameraTarget))
			{
				cmd.ReleaseTemporaryRT(_depthAttachmentHandle.id);
				_depthAttachmentHandle = RenderTargetHandle.CameraTarget;
			}
		}
	}

	private DepthNormalsPass _depthNormalsPass;

	private RenderTargetHandle _depthNormalsTexture;

	private Material _depthNormalsMaterial;

	public FlatKitDepthNormals(RenderTargetHandle depthNormalsTexture)
	{
		_depthNormalsTexture = depthNormalsTexture;
	}

	public override void Create()
	{
		_depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
		_depthNormalsPass = new DepthNormalsPass(RenderQueueRange.opaque, -1, _depthNormalsMaterial)
		{
			renderPassEvent = RenderPassEvent.AfterRenderingPrePasses
		};
		_depthNormalsTexture.Init("_CameraDepthNormalsTexture");
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		_depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor, _depthNormalsTexture);
		renderer.EnqueuePass(_depthNormalsPass);
	}
}
