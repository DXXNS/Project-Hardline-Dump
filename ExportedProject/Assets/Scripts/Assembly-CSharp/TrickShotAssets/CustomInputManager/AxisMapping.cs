using System;
using UnityEngine;

namespace TrickShotAssets.CustomInputManager
{
	[Serializable]
	public class AxisMapping
	{
		public string inputName;

		public KeyCode key;

		public KeyCode defaultKey;
	}
}
