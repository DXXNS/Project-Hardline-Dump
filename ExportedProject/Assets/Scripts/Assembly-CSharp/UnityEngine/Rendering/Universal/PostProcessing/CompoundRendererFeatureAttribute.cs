using System;

namespace UnityEngine.Rendering.Universal.PostProcessing
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CompoundRendererFeatureAttribute : Attribute
	{
		private readonly string name;

		private readonly InjectionPoint injectionPoint;

		private readonly bool shareInstance;

		public string Name => name;

		public InjectionPoint InjectionPoint => injectionPoint;

		public bool ShareInstance => shareInstance;

		public CompoundRendererFeatureAttribute(string name, InjectionPoint injectionPoint, bool shareInstance = false)
		{
			this.name = name;
			this.injectionPoint = injectionPoint;
			this.shareInstance = shareInstance;
		}

		public static CompoundRendererFeatureAttribute GetAttribute(Type type)
		{
			if (type == null)
			{
				return null;
			}
			object[] customAttributes = type.GetCustomAttributes(typeof(CompoundRendererFeatureAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			return customAttributes[0] as CompoundRendererFeatureAttribute;
		}
	}
}
