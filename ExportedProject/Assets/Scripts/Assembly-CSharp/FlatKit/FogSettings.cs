using UnityEngine;

namespace FlatKit
{
	[CreateAssetMenu(fileName = "FogSettings", menuName = "FlatKit/Fog Settings")]
	public class FogSettings : ScriptableObject
	{
		[Header("Distance Fog")]
		public bool useDistance = true;

		public Gradient distanceGradient;

		public float near;

		public float far = 100f;

		[Range(0f, 1f)]
		public float distanceFogIntensity = 1f;

		public bool useDistanceFogOnSky;

		[Header("Height Fog")]
		[Space]
		public bool useHeight;

		public Gradient heightGradient;

		public float low;

		public float high = 10f;

		[Range(0f, 1f)]
		public float heightFogIntensity = 1f;

		public bool useHeightFogOnSky;

		[Header("Blending")]
		[Space]
		[Range(0f, 1f)]
		public float distanceHeightBlend = 0.5f;

		private void OnValidate()
		{
			if (low > high)
			{
				Debug.LogWarning("[FlatKit] Fog Height configuration error: 'Low' must not be greater than 'High'");
			}
			if (near > far)
			{
				Debug.LogWarning("[FlatKit] Fog Distance configuration error: 'Near' must not be greater than 'Far'");
			}
		}
	}
}
