using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerNetworkManager : HardlineNetworkManager
{
	private List<NetworkRoomPlayer> roomPlayers = new List<NetworkRoomPlayer>();

	[SerializeField]
	private List<NetworkRoomPlayer> team1 = new List<NetworkRoomPlayer>();

	[SerializeField]
	private List<NetworkRoomPlayer> team2 = new List<NetworkRoomPlayer>();

	private int teamSize = 5;

	public List<Player> GamePlayers { get; } = new List<Player>();


	public List<NetworkRoomPlayer> Team1
	{
		get
		{
			return team1;
		}
		set
		{
			team1 = value;
		}
	}

	public List<NetworkRoomPlayer> Team2
	{
		get
		{
			return team2;
		}
		set
		{
			team2 = value;
		}
	}

	public new static event Action OnClientConnected;

	public new static event Action OnClientDisconnected;

	public static event Action OnServerDisconnected;

	protected override void Update()
	{
		base.Update();
		if (SceneManager.GetActiveScene().name == menuScene)
		{
			UpdateTeams();
		}
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);
		MultiplayerNetworkManager.OnClientDisconnected?.Invoke();
		Team1.Clear();
		Team2.Clear();
		UpdateTeams();
		NetworkManager.Shutdown();
	}

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		base.OnServerDisconnect(conn);
		MultiplayerNetworkManager.OnServerDisconnected?.Invoke();
		MonoBehaviour.print("client disconnect A");
	}

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		if (SceneManager.GetActiveScene().name == menuScene)
		{
			NetworkRoomPlayer networkRoomPlayer = UnityEngine.Object.Instantiate(roomPlayerPrefab);
			NetworkServer.AddPlayerForConnection(conn, networkRoomPlayer.gameObject);
		}
		else
		{
			conn.Disconnect();
		}
	}

	public void UpdateTeams()
	{
		if (!UnityEngine.Object.FindObjectOfType<LobbyUIManager>())
		{
			return;
		}
		Team1.Clear();
		Team2.Clear();
		NetworkRoomPlayer[] array = UnityEngine.Object.FindObjectsOfType<NetworkRoomPlayer>();
		foreach (NetworkRoomPlayer networkRoomPlayer in array)
		{
			if (networkRoomPlayer.Team == 1)
			{
				Team1.Add(networkRoomPlayer);
			}
			else if (networkRoomPlayer.Team == 2)
			{
				Team2.Add(networkRoomPlayer);
			}
		}
		if ((bool)UnityEngine.Object.FindObjectOfType<LobbyUIManager>())
		{
			if (team1.Count > 0 && team2.Count > 0)
			{
				UnityEngine.Object.FindObjectOfType<LobbyUIManager>().PlayersOnEachTeam = true;
			}
			else
			{
				UnityEngine.Object.FindObjectOfType<LobbyUIManager>().PlayersOnEachTeam = false;
			}
			if ((bool)UnityEngine.Object.FindObjectOfType<LobbyUIManager>())
			{
				UnityEngine.Object.FindObjectOfType<LobbyUIManager>().UpdateTeamBoard();
			}
		}
	}

	public void StartGame(string scene)
	{
		ServerChangeScene(scene);
	}

	public void AllPlayersLoaded()
	{
		if (isServer)
		{
			roomPlayers = new List<NetworkRoomPlayer>(UnityEngine.Object.FindObjectsOfType<NetworkRoomPlayer>());
			for (int num = roomPlayers.Count - 1; num >= 0; num--)
			{
				NetworkConnection connectionToClient = roomPlayers[num].connectionToClient;
				GameObject gameObject = UnityEngine.Object.Instantiate(gamePlayer);
				NetworkServer.Destroy(connectionToClient.identity.gameObject);
				NetworkServer.ReplacePlayerForConnection(connectionToClient, gameObject.gameObject);
				gameObject.GetComponent<Player>().Team = roomPlayers[num].GetComponent<NetworkRoomPlayer>().Team;
				gameObject.GetComponent<Player>().ReplicateTeam(roomPlayers[num].GetComponent<NetworkRoomPlayer>().Team);
				gameObject.GetComponent<Player>().HumanName = roomPlayers[num].GetComponent<NetworkRoomPlayer>().PlayerName;
				gameObject.GetComponent<Human>().ReplicateHumanName(roomPlayers[num].GetComponent<NetworkRoomPlayer>().PlayerName);
			}
		}
		UnityEngine.Object.FindObjectOfType<HardlineGameManager>().InitPlayersAndFindLocalPlayer();
		UnityEngine.Object.FindObjectOfType<HardlineGameManager>().AllPlayersLoaded();
	}

	public void SwapLocalPlayerTeam()
	{
		NetworkRoomPlayer[] array = UnityEngine.Object.FindObjectsOfType<NetworkRoomPlayer>();
		foreach (NetworkRoomPlayer networkRoomPlayer in array)
		{
			if (networkRoomPlayer.hasAuthority)
			{
				networkRoomPlayer.SwapTeam();
			}
		}
	}
}
