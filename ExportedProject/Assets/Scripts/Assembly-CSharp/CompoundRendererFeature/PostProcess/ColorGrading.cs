using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace CompoundRendererFeature.PostProcess
{
	[Serializable]
	[VolumeComponentMenu("Quibli/Stylized Color Grading")]
	public class ColorGrading : VolumeComponent
	{
		[Tooltip("Controls the amount to which image colors are modified.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f, overrideState: true);

		[Space]
		public ClampedFloatParameter blueShadows = new ClampedFloatParameter(0f, 0f, 1f, overrideState: true);

		public ClampedFloatParameter greenShadows = new ClampedFloatParameter(0f, 0f, 1f, overrideState: true);

		public ClampedFloatParameter redHighlights = new ClampedFloatParameter(0f, 0f, 1f, overrideState: true);

		public ClampedFloatParameter contrast = new ClampedFloatParameter(0f, 0f, 1f, overrideState: true);

		[Space]
		public ClampedFloatParameter vibrance = new ClampedFloatParameter(0f, 0f, 1f, overrideState: true);

		public ClampedFloatParameter saturation = new ClampedFloatParameter(0f, 0f, 1f, overrideState: true);
	}
}
