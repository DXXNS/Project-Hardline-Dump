using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
	private void Start()
	{
		if (!SteamManager.Initialized)
		{
			MonoBehaviour.print("steam works closed");
			return;
		}
		string personaName = SteamFriends.GetPersonaName();
		MonoBehaviour.print("steam name " + personaName);
	}

	private void Update()
	{
	}
}
