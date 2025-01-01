using System;
using UnityEngine;

namespace TrickShotAssets.CustomInputManager
{
	[Serializable]
	public class KeyMapping
	{
		public string inputName;

		public KeyCode key;

		public KeyCode defaultKey;
	}
}
