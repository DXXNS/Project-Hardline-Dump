using TrickShotAssets.CustomInputManager;
using UnityEngine;

public class KeyBindingsManager : MonoBehaviour
{
	private InputManagerStorage inputManager;

	public void Start()
	{
		inputManager = InputManagerStorage.Instance;
	}

	public void ResetBinds()
	{
		inputManager.ResetAllKeyMappings();
	}
}
