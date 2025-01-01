using System;

namespace UnityEngine.Rendering.Universal.PostProcessing
{
	[Flags]
	public enum InjectionPoint
	{
		AfterOpaqueAndSky = 1,
		BeforePostProcess = 2,
		AfterPostProcess = 4
	}
}
