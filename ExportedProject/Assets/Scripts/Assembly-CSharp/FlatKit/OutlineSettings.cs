using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FlatKit
{
	[CreateAssetMenu(fileName = "OutlineSettings", menuName = "FlatKit/Outline Settings")]
	public class OutlineSettings : ScriptableObject
	{
		public Color edgeColor = Color.white;

		[Range(0f, 5f)]
		public int thickness = 1;

		[Space]
		public bool useDepth = true;

		public bool useNormals;

		public bool useColor;

		[Header("Advanced settings")]
		[Space(25f)]
		public float minDepthThreshold;

		public float maxDepthThreshold = 0.25f;

		[Space]
		public float minNormalsThreshold;

		public float maxNormalsThreshold = 0.25f;

		[Space]
		public float minColorThreshold;

		public float maxColorThreshold = 0.25f;

		[Space]
		public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingTransparents;

		[Space(20f)]
		public bool outlineOnly;

		private void OnValidate()
		{
			if (minDepthThreshold > maxDepthThreshold)
			{
				Debug.LogWarning("[FlatKit] Outline configuration error: 'Min Depth Threshold' must not be greater than 'Max Depth Threshold'");
			}
			if (minNormalsThreshold > maxNormalsThreshold)
			{
				Debug.LogWarning("[FlatKit] Outline configuration error: 'Min Normals Threshold' must not be greater than 'Max Normals Threshold'");
			}
			if (minColorThreshold > maxColorThreshold)
			{
				Debug.LogWarning("[FlatKit] Outline configuration error: 'Min Color Threshold' must not be greater than 'Max Color Threshold'");
			}
		}
	}
}
