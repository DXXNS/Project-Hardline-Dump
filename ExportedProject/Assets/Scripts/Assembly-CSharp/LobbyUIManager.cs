using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : NetworkBehaviour
{
	[SerializeField]
	private bool useDeveloperTestMode;

	[SerializeField]
	private bool allowOnePlayerGame;

	private bool playersOnEachTeam;

	[SerializeField]
	private InputField ipAddressInput;

	[SerializeField]
	private GameObject selectionMenu;

	[SerializeField]
	private GameObject gameLobbyMenu;

	[SerializeField]
	private GameObject serverBrowserMenu;

	[SerializeField]
	private Button startGameButton;

	[SerializeField]
	private GameObject noPlayerOnOneTeamText;

	[SerializeField]
	private MultiplayerNetworkManager networkManager;

	[SerializeField]
	private SteamLobby steamLobby;

	[SerializeField]
	private List<Text> playerNameBoardTeam1 = new List<Text>();

	[SerializeField]
	private List<Text> playerNameBoardTeam2 = new List<Text>();

	[SerializeField]
	private Dropdown mapSelect;

	[SerializeField]
	private Dropdown offlineMapSelect;

	[SerializeField]
	private Text invalidNameText;

	[SerializeField]
	private RawImage mapIcon;

	[SerializeField]
	private Texture[] possibleMapIcons;

	[SerializeField]
	private GameObject levelLoadingPanel;

	[SerializeField]
	private GameObject connectingToServerPanel;

	[SerializeField]
	private Slider sceneLoadSlider;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Button startButton;

	[SerializeField]
	private Dropdown lobbyTypeDropdown;

	[SerializeField]
	private Dropdown difficultyDropdown;

	[SyncVar]
	private int mapSelected;

	private static int maxNameLength;

	private LocalLobbyUIHandler localUI;

	private int lobbyType;

	private bool queueStartShootingRange;

	private bool queueStartOfflineLevel;

	[SerializeField]
	private Toggle reconModeToggle;

	public bool PlayersOnEachTeam
	{
		get
		{
			return playersOnEachTeam;
		}
		set
		{
			playersOnEachTeam = value;
		}
	}

	public int LobbyType
	{
		get
		{
			return lobbyType;
		}
		set
		{
			lobbyType = value;
		}
	}

	public int NetworkmapSelected
	{
		get
		{
			return mapSelected;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref mapSelected))
			{
				int num = mapSelected;
				SetSyncVar(value, ref mapSelected, 1uL);
			}
		}
	}

	[ClientRpc]
	public void RpcSetMapSelected(int map)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(map);
		SendRPCInternal(typeof(LobbyUIManager), "RpcSetMapSelected", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void Start()
	{
		localUI = Object.FindObjectOfType<LocalLobbyUIHandler>();
		networkManager = Object.FindObjectOfType<MultiplayerNetworkManager>();
		steamLobby = Object.FindObjectOfType<SteamLobby>();
		SetDifficulty();
	}

	private void OnEnable()
	{
		HardlineNetworkManager.OnClientConnected += HandleClientConnected;
		HardlineNetworkManager.OnClientDisconnected += HandleClientDisconnected;
	}

	private void OnDisable()
	{
		HardlineNetworkManager.OnClientConnected -= HandleClientConnected;
		HardlineNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
	}

	public void CallLeaveToMenu()
	{
		Object.FindObjectOfType<HardlineNetworkManager>().LeaveToMenuAndDisconnect();
	}

	public void CallLeaveToMainMenu()
	{
		Object.FindObjectOfType<HardlineNetworkManager>().LeaveToMainMenuAndDisconnect();
	}

	public void CallLeaveGame()
	{
		Object.FindObjectOfType<HardlineNetworkManager>().LeaveToLobbyAndDisconnect();
	}

	public void CallSwapTeam()
	{
		Object.FindObjectOfType<MultiplayerNetworkManager>().SwapLocalPlayerTeam();
	}

	public void Update()
	{
		if (!Object.FindObjectOfType<NetworkRoomPlayer>())
		{
			return;
		}
		if (Object.FindObjectOfType<NetworkRoomPlayer>().isServer)
		{
			if (networkManager.Team1.Count + networkManager.Team2.Count == 1)
			{
				reconModeToggle.gameObject.SetActive(value: true);
			}
			else
			{
				reconModeToggle.gameObject.SetActive(value: false);
			}
			allowOnePlayerGame = reconModeToggle.isOn;
			if (playersOnEachTeam | allowOnePlayerGame | useDeveloperTestMode)
			{
				noPlayerOnOneTeamText.SetActive(value: false);
				startGameButton.interactable = true;
			}
			else
			{
				noPlayerOnOneTeamText.SetActive(value: true);
				startGameButton.interactable = false;
			}
			mapSelect.interactable = true;
			mapIcon.texture = possibleMapIcons[mapSelect.value];
			RpcSetMapSelected(mapSelect.value);
		}
		else
		{
			reconModeToggle.gameObject.SetActive(value: false);
			noPlayerOnOneTeamText.SetActive(value: false);
			startGameButton.interactable = false;
			mapSelect.interactable = false;
			mapIcon.texture = possibleMapIcons[mapSelect.value];
		}
	}

	public void DisableAllMenus()
	{
		selectionMenu.gameObject.SetActive(value: false);
	}

	public void StartSteamHost()
	{
		Object.FindObjectOfType<LocalLobbyUIHandler>().LoadMenu("empty");
		steamLobby.HostLobby();
	}

	public void StartHost()
	{
		networkManager.StartHost();
	}

	public void JoinLobby()
	{
		string text = ipAddressInput.text;
		if (!(text == "") && text != null && (ValidateIPv4(text) || !(text != "localhost")))
		{
			networkManager.networkAddress = text;
			networkManager.StartClient();
			localUI.LoadMenu("");
			localUI.LoadMenu("game lobby");
		}
	}

	private void HandleClientConnected()
	{
		if (queueStartShootingRange)
		{
			queueStartShootingRange = false;
			LoadShootingRange();
		}
		else if (queueStartOfflineLevel)
		{
			queueStartOfflineLevel = false;
			LoadOfflineLevel();
		}
		else
		{
			localUI.LoadMenu("game lobby");
		}
	}

	private void HandleClientDisconnected()
	{
		localUI.LoadMenu("join lobby");
		networkManager.Team1.Clear();
		networkManager.Team2.Clear();
		UpdateTeamBoard();
	}

	public void StartGame()
	{
		string scene = "Level" + (mapSelect.value + 1);
		Object.FindObjectOfType<LobbyUIManager>().StartLoadMapPanel();
		networkManager.StartGame(scene);
		Object.FindObjectOfType<SteamLobby>().SetJoinable(joinable: false);
	}

	public void StartShootingRange()
	{
		queueStartShootingRange = true;
		lobbyType = 1;
		StartSteamHost();
	}

	public void StartOfflineLevel()
	{
		queueStartOfflineLevel = true;
		lobbyType = 1;
		StartSteamHost();
	}

	public void LoadShootingRange()
	{
		string scene = "Shooting Range";
		Object.FindObjectOfType<LobbyUIManager>().StartLoadMapPanel();
		networkManager.StartGame(scene);
		Object.FindObjectOfType<SteamLobby>().SetJoinable(joinable: false);
	}

	public void LoadOfflineLevel()
	{
		string scene = "Level_Offline" + (offlineMapSelect.value + 1);
		Object.FindObjectOfType<LobbyUIManager>().StartLoadMapPanel();
		networkManager.StartGame(scene);
		Object.FindObjectOfType<SteamLobby>().SetJoinable(joinable: false);
	}

	public static string GetSteamName()
	{
		return SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID());
	}

	public void LeaveGame()
	{
		networkManager.LeaveToMenuAndDisconnect();
	}

	public void SetDifficulty()
	{
		if (difficultyDropdown.value == 0)
		{
			PracticeModeGameManager.difficulty = EnemyAI.EnemyDifficulties.EASY;
		}
		else if (difficultyDropdown.value == 1)
		{
			PracticeModeGameManager.difficulty = EnemyAI.EnemyDifficulties.MEDIUM;
		}
		else
		{
			PracticeModeGameManager.difficulty = EnemyAI.EnemyDifficulties.HARD;
		}
	}

	public void UpdateTeamBoard()
	{
		if (networkManager == null)
		{
			return;
		}
		for (int i = 0; i < playerNameBoardTeam1.Count; i++)
		{
			if (networkManager.Team1.Count >= i + 1)
			{
				playerNameBoardTeam1[i].text = networkManager.Team1[i].PlayerName;
			}
			else
			{
				playerNameBoardTeam1[i].text = "";
			}
		}
		for (int j = 0; j < playerNameBoardTeam2.Count; j++)
		{
			if (networkManager.Team2.Count >= j + 1)
			{
				playerNameBoardTeam2[j].text = networkManager.Team2[j].PlayerName;
			}
			else
			{
				playerNameBoardTeam2[j].text = "";
			}
		}
	}

	public bool ValidateIPv4(string ipString)
	{
		if (string.IsNullOrEmpty(ipString))
		{
			return false;
		}
		string[] array = ipString.Split('.');
		if (array.Length != 4)
		{
			return false;
		}
		byte tempForParsing;
		return array.All((string r) => byte.TryParse(r, out tempForParsing));
	}

	public void UpdateLoadingPanel(bool enabled, float value)
	{
		if ((bool)levelLoadingPanel && (bool)sceneLoadSlider)
		{
			levelLoadingPanel.SetActive(enabled);
			sceneLoadSlider.value = value;
		}
	}

	public void StartLoadMapPanel()
	{
		if (base.isServer)
		{
			UpdateLoadingPanel(enabled: true, 0f);
			RpcLoadMap();
		}
	}

	[ClientRpc]
	public void RpcLoadMap()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(LobbyUIManager), "RpcLoadMap", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void UpdateLobbyTypeDropdown()
	{
		lobbyType = lobbyTypeDropdown.value;
	}

	public void OpenPracticeMode()
	{
		Object.FindObjectOfType<LocalLobbyUIHandler>().LoadMenu("practice menu");
	}

	public void OpenSelection()
	{
		Object.FindObjectOfType<LocalLobbyUIHandler>().LoadMenu("join lobby");
	}

	static LobbyUIManager()
	{
		maxNameLength = 20;
		RemoteCallHelper.RegisterRpcDelegate(typeof(LobbyUIManager), "RpcSetMapSelected", InvokeUserCode_RpcSetMapSelected);
		RemoteCallHelper.RegisterRpcDelegate(typeof(LobbyUIManager), "RpcLoadMap", InvokeUserCode_RpcLoadMap);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_RpcSetMapSelected(int map)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			NetworkmapSelected = map;
			mapSelect.value = map;
		}
	}

	protected static void InvokeUserCode_RpcSetMapSelected(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetMapSelected called on server.");
		}
		else
		{
			((LobbyUIManager)obj).UserCode_RpcSetMapSelected(reader.ReadInt());
		}
	}

	protected void UserCode_RpcLoadMap()
	{
		if (!base.isServer)
		{
			UpdateLoadingPanel(enabled: true, 0f);
		}
	}

	protected static void InvokeUserCode_RpcLoadMap(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcLoadMap called on server.");
		}
		else
		{
			((LobbyUIManager)obj).UserCode_RpcLoadMap();
		}
	}

	public override bool SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		bool result = base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(mapSelected);
			return true;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(mapSelected);
			result = true;
		}
		return result;
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			int num = mapSelected;
			NetworkmapSelected = reader.ReadInt();
			return;
		}
		long num2 = (long)reader.ReadULong();
		if ((num2 & 1L) != 0L)
		{
			int num3 = mapSelected;
			NetworkmapSelected = reader.ReadInt();
		}
	}
}
