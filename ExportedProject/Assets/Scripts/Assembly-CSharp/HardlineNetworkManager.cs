using System;
using System.Net;
using System.Net.Sockets;
using Mirror;
using UnityEngine;

public class HardlineNetworkManager : NetworkManager
{
	private static bool gracefulDisconnectFlag;

	[SerializeField]
	protected string menuScene;

	[SerializeField]
	protected NetworkRoomPlayer roomPlayerPrefab;

	[SerializeField]
	protected GameObject gamePlayer;

	[SerializeField]
	protected bool isServer;

	public static bool GracefulDisconnectFlag
	{
		get
		{
			return gracefulDisconnectFlag;
		}
		set
		{
			gracefulDisconnectFlag = value;
		}
	}

	public static event Action OnClientConnected;

	public static event Action OnClientDisconnected;

	public new virtual void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		HardlineNetworkManager[] array = UnityEngine.Object.FindObjectsOfType<HardlineNetworkManager>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		GracefulDisconnectFlag = false;
	}

	public override void OnStartServer()
	{
		networkAddress = GetLocalIPAddress();
		isServer = true;
	}

	public override void OnStartClient()
	{
	}

	protected virtual void Update()
	{
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);
		HardlineNetworkManager.OnClientConnected?.Invoke();
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		if ((bool)UnityEngine.Object.FindObjectOfType<LocalLobbyUIHandler>())
		{
			UnityEngine.Object.FindObjectOfType<LocalLobbyUIHandler>().LoadMenu("selection");
		}
		base.OnClientDisconnect(conn);
		HardlineNetworkManager.OnClientDisconnected?.Invoke();
		NetworkManager.Shutdown();
		if ((bool)UnityEngine.Object.FindObjectOfType<HardlineGameManager>())
		{
			UnityEngine.Object.FindObjectOfType<HardlineGameManager>().DescreasePlayersLoaded();
		}
		if (!GracefulDisconnectFlag)
		{
			LevelManager.ShowDisconnectedPanelFlag = true;
		}
		else
		{
			GracefulDisconnectFlag = false;
		}
	}

	public override void OnServerConnect(NetworkConnection conn)
	{
		if (base.numPlayers >= maxConnections)
		{
			conn.Disconnect();
		}
	}

	public override void ServerChangeScene(string newSceneName)
	{
		base.ServerChangeScene(newSceneName);
	}

	public void DamageAnotherPlayer(Human target, float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		target.ReplicateDealDamage(value, causer, hitPos, hitRot);
	}

	public void LeaveToMenuAndDisconnect()
	{
		GracefulDisconnectFlag = true;
		NetworkManager.Shutdown();
	}

	public void LeaveToMainMenuAndDisconnect()
	{
		GracefulDisconnectFlag = true;
		UnityEngine.Object.FindObjectOfType<LevelManager>().LoadMainMenu();
		NetworkManager.Shutdown();
	}

	public void LeaveToLobbyAndDisconnect()
	{
		NetworkManager.Shutdown();
		GracefulDisconnectFlag = true;
		UnityEngine.Object.FindObjectOfType<LocalLobbyUIHandler>().LoadMenu("selection");
		if (isServer)
		{
			if ((bool)UnityEngine.Object.FindObjectOfType<SteamLobby>())
			{
				UnityEngine.Object.FindObjectOfType<SteamLobby>().SetJoinable(joinable: false);
			}
			if ((bool)UnityEngine.Object.FindObjectOfType<HardlineGameManager>())
			{
				UnityEngine.Object.FindObjectOfType<HardlineGameManager>().RpcServerClose();
			}
		}
	}

	public static string GetLocalIPAddress()
	{
		IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
		foreach (IPAddress iPAddress in addressList)
		{
			if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return iPAddress.ToString();
			}
		}
		throw new Exception("No network adapters with an IPv4 address in the system!");
	}
}
