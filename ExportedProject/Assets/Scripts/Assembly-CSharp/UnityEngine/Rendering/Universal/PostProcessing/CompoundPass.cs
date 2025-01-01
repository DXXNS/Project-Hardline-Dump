using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal.PostProcessing
{
	public class CompoundPass : ScriptableRenderPass
	{
		private readonly InjectionPoint _injectionPoint;

		private string m_PassName;

		private List<CompoundRenderer> m_PostProcessRenderers;

		private List<int> m_ActivePostProcessRenderers;

		private RenderTargetHandle[] m_Intermediate;

		private bool[] m_IntermediateAllocated;

		private RenderTextureDescriptor _intermediateDescriptor;

		private RenderTargetIdentifier m_Source;

		private RenderTargetIdentifier m_Destination;

		private List<ProfilingSampler> m_ProfilingSamplers;

		public bool HasPostProcessRenderers => m_PostProcessRenderers.Count != 0;

		public CompoundPass(InjectionPoint injectionPoint, List<CompoundRenderer> renderers)
		{
			_injectionPoint = injectionPoint;
			m_ProfilingSamplers = new List<ProfilingSampler>(renderers.Count);
			m_PostProcessRenderers = renderers;
			foreach (CompoundRenderer renderer in renderers)
			{
				CompoundRendererFeatureAttribute attribute = CompoundRendererFeatureAttribute.GetAttribute(renderer.GetType());
				m_ProfilingSamplers.Add(new ProfilingSampler(attribute?.Name));
			}
			m_ActivePostProcessRenderers = new List<int>(renderers.Count);
			switch (injectionPoint)
			{
			case InjectionPoint.AfterOpaqueAndSky:
				base.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
				m_PassName = "[Dustyroom] PostProcess after Opaque & Sky";
				break;
			case InjectionPoint.BeforePostProcess:
				base.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
				m_PassName = "[Dustyroom] PostProcess before PostProcess";
				break;
			case InjectionPoint.AfterPostProcess:
				base.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
				m_PassName = "[Dustyroom] PostProcess after PostProcess";
				break;
			}
			m_Intermediate = new RenderTargetHandle[2];
			m_Intermediate[0].Init("_IntermediateRT0");
			m_Intermediate[1].Init("_IntermediateRT1");
			m_IntermediateAllocated = new bool[2];
			m_IntermediateAllocated[0] = false;
			m_IntermediateAllocated[1] = false;
		}

		private RenderTargetIdentifier GetIntermediate(CommandBuffer cmd, int index)
		{
			if (!m_IntermediateAllocated[index])
			{
				cmd.GetTemporaryRT(m_Intermediate[index].id, _intermediateDescriptor);
				m_IntermediateAllocated[index] = true;
			}
			return m_Intermediate[index].Identifier();
		}

		private void CleanupIntermediate(CommandBuffer cmd)
		{
			for (int i = 0; i < 2; i++)
			{
				if (m_IntermediateAllocated[i])
				{
					cmd.ReleaseTemporaryRT(m_Intermediate[i].id);
					m_IntermediateAllocated[i] = false;
				}
			}
		}

		public void Setup(RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			m_Source = source;
			m_Destination = destination;
		}

		public bool PrepareRenderers(in RenderingData renderingData)
		{
			if (renderingData.cameraData.cameraType == CameraType.Preview)
			{
				return false;
			}
			bool flag = renderingData.cameraData.cameraType == CameraType.SceneView;
			ScriptableRenderPassInput scriptableRenderPassInput = ScriptableRenderPassInput.None;
			m_ActivePostProcessRenderers.Clear();
			for (int i = 0; i < m_PostProcessRenderers.Count; i++)
			{
				CompoundRenderer compoundRenderer = m_PostProcessRenderers[i];
				if ((!flag || compoundRenderer.visibleInSceneView) && compoundRenderer.Setup(in renderingData, _injectionPoint))
				{
					m_ActivePostProcessRenderers.Add(i);
					scriptableRenderPassInput |= compoundRenderer.input;
				}
			}
			ConfigureInput(scriptableRenderPassInput);
			return m_ActivePostProcessRenderers.Count != 0;
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			_intermediateDescriptor = renderingData.cameraData.cameraTargetDescriptor;
			_intermediateDescriptor.msaaSamples = 1;
			_intermediateDescriptor.depthBufferBits = 0;
			CommandBuffer commandBuffer = CommandBufferPool.Get(m_PassName);
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
			int width = _intermediateDescriptor.width;
			int height = _intermediateDescriptor.height;
			commandBuffer.SetGlobalVector("_ScreenSize", new Vector4(width, height, 1f / (float)width, 1f / (float)height));
			bool flag = false;
			int num = 0;
			for (int i = 0; i < m_ActivePostProcessRenderers.Count; i++)
			{
				int index = m_ActivePostProcessRenderers[i];
				CompoundRenderer compoundRenderer = m_PostProcessRenderers[index];
				RenderTargetIdentifier source;
				RenderTargetIdentifier destination;
				if (i == 0)
				{
					source = m_Source;
					if (m_ActivePostProcessRenderers.Count == 1)
					{
						if (m_Source == m_Destination)
						{
							destination = GetIntermediate(commandBuffer, 0);
							flag = true;
						}
						else
						{
							destination = m_Destination;
						}
					}
					else
					{
						destination = GetIntermediate(commandBuffer, num);
					}
				}
				else
				{
					source = GetIntermediate(commandBuffer, num);
					if (i == m_ActivePostProcessRenderers.Count - 1)
					{
						destination = m_Destination;
					}
					else
					{
						num = 1 - num;
						destination = GetIntermediate(commandBuffer, num);
					}
				}
				using (new ProfilingScope(commandBuffer, m_ProfilingSamplers[index]))
				{
					if (!compoundRenderer.Initialized)
					{
						compoundRenderer.InitializeInternal();
					}
					compoundRenderer.Render(commandBuffer, source, destination, ref renderingData, _injectionPoint);
				}
			}
			if (flag)
			{
				Blit(commandBuffer, m_Intermediate[0].Identifier(), m_Destination);
			}
			CleanupIntermediate(commandBuffer);
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);
		}
	}
}
