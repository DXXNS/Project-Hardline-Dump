using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestRangeNetworkManager : HardlineNetworkManager
{
	private bool changeableFramerate = true;

	private bool runLowFrameRate;

	public new static event Action OnClientConnected;

	public new static event Action OnClientDisconnected;

	public override void Start()
	{
		base.Start();
		StartHost();
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		MonoBehaviour.print("on client disconnect");
		base.OnClientDisconnect(conn);
		TestRangeNetworkManager.OnClientDisconnected?.Invoke();
		NetworkManager.Shutdown();
	}

	protected override void Update()
	{
		if (SceneManager.GetActiveScene().name == "Main Menu")
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (changeableFramerate)
		{
			if (Input.GetKeyDown(KeyCode.P))
			{
				runLowFrameRate = !runLowFrameRate;
			}
			if (runLowFrameRate)
			{
				Application.targetFrameRate = 30;
			}
			else
			{
				Application.targetFrameRate = 120;
			}
		}
	}

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		base.OnServerAddPlayer(conn);
		MonoBehaviour.print("init server");
		Player[] array = UnityEngine.Object.FindObjectsOfType<Player>();
		foreach (Player obj in array)
		{
			obj.Init();
			obj.InputEnabled = true;
		}
	}

	public new void DamageAnotherPlayer(Human target, float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		MonoBehaviour.print("deal damage");
		target.ReplicateDealDamage(value, causer, hitPos, hitRot);
	}
}
