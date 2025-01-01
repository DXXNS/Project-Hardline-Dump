using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
	protected Callback<LobbyCreated_t> lobbyCreated;

	protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;

	protected Callback<LobbyEnter_t> lobbyEntered;

	protected Callback<LobbyMatchList_t> lobbyList;

	protected Callback<LobbyDataUpdate_t> lobbyDataUpdated;

	private List<CSteamID> lobbyIDs = new List<CSteamID>();

	private CSteamID lobbyCreatedID;

	public const string HostAddressKey = "HostAddress";

	private static bool callBacksInitialized;

	[SerializeField]
	private HardlineNetworkManager networkManager;

	public void HostLobby()
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
	}

	private void Start()
	{
		networkManager = GetComponent<HardlineNetworkManager>();
		if (!SteamManager.Initialized)
		{
			MonoBehaviour.print("steam closed");
		}
		else if (!callBacksInitialized)
		{
			lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
			lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
			lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
			lobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
			callBacksInitialized = true;
		}
	}

	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult == EResult.k_EResultOK)
		{
			if ((bool)networkManager)
			{
				networkManager.StartHost();
			}
			lobbyCreatedID = new CSteamID(callback.m_ulSteamIDLobby);
			SetJoinable(joinable: true);
			int num = 0;
			if ((bool)Object.FindObjectOfType<LobbyUIManager>())
			{
				num = Object.FindObjectOfType<LobbyUIManager>().LobbyType;
			}
			switch (num)
			{
			case 0:
				MonoBehaviour.print("lobby public");
				SteamMatchmaking.SetLobbyType(new CSteamID(callback.m_ulSteamIDLobby), ELobbyType.k_ELobbyTypePublic);
				break;
			case 1:
				MonoBehaviour.print("lobby private");
				SteamMatchmaking.SetLobbyType(new CSteamID(callback.m_ulSteamIDLobby), ELobbyType.k_ELobbyTypePrivate);
				break;
			case 2:
				MonoBehaviour.print("lobby friends only");
				SteamMatchmaking.SetLobbyType(new CSteamID(callback.m_ulSteamIDLobby), ELobbyType.k_ELobbyTypeFriendsOnly);
				break;
			}
			HardlineGameManager.currentLobby = new CSteamID(callback.m_ulSteamIDLobby);
			SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
			SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", Object.FindObjectOfType<LocalLobbyUIHandler>().GetHostServerName());
			SteamNetworkingUtils.GetLocalPingLocation(out var result);
			SteamNetworkingUtils.ConvertPingLocationToString(ref result, out var pszBuf, 250);
			SteamNetworkingUtils.ParsePingLocationString(pszBuf, out var result2);
			MonoBehaviour.print("estimate ping: " + SteamNetworkingUtils.EstimatePingTimeFromLocalHost(ref result2));
			MonoBehaviour.print(pszBuf);
			SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "location", pszBuf);
		}
	}

	private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		Object.FindObjectOfType<LocalLobbyUIHandler>().SetConnectingPanel(active: true);
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}

	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		Object.FindObjectOfType<LocalLobbyUIHandler>().SetConnectingPanel(active: false);
		if (NetworkServer.active)
		{
			MonoBehaviour.print("server not active");
			return;
		}
		Object.FindObjectOfType<LocalLobbyUIHandler>().LoadMenu("game lobby");
		string lobbyData = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");
		networkManager.networkAddress = lobbyData;
		networkManager.StartClient();
	}

	public void JoinLobby(CSteamID lobbyID)
	{
		Object.FindObjectOfType<LocalLobbyUIHandler>().SetConnectingPanel(active: true);
		CSteamID cSteamID = lobbyID;
		MonoBehaviour.print("join lobby " + cSteamID.ToString());
		HardlineGameManager.currentLobby = lobbyID;
		SteamMatchmaking.JoinLobby(lobbyID);
	}

	public void GetLobbiesList()
	{
		Object.FindObjectOfType<LobbiesListManager>().DestroyLobbies();
		if (lobbyIDs.Count > 0)
		{
			lobbyIDs.Clear();
		}
		SteamMatchmaking.AddRequestLobbyListResultCountFilter(120);
		SteamMatchmaking.RequestLobbyList();
	}

	public void GetLobbiesList(string name)
	{
		Object.FindObjectOfType<LobbiesListManager>().DestroyLobbies();
		if (lobbyIDs.Count > 0)
		{
			lobbyIDs.Clear();
		}
		MonoBehaviour.print("Get lobbies list " + name);
		SteamMatchmaking.AddRequestLobbyListStringFilter("name", name, ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.RequestLobbyList();
	}

	private void OnGetLobbyList(LobbyMatchList_t result)
	{
		if (LobbiesListManager.instance.ListOfLobbies.Count > 0)
		{
			LobbiesListManager.instance.DestroyLobbies();
		}
		for (int i = 0; i < result.m_nLobbiesMatching; i++)
		{
			CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
			lobbyIDs.Add(lobbyByIndex);
			SteamMatchmaking.RequestLobbyData(lobbyByIndex);
		}
	}

	private void OnGetLobbyData(LobbyDataUpdate_t result)
	{
		Object.FindObjectOfType<LobbiesListManager>().DisplayLobbies(lobbyIDs, result);
	}

	public void SetJoinable(bool joinable)
	{
		_ = lobbyCreatedID;
		SteamMatchmaking.SetLobbyJoinable(lobbyCreatedID, joinable);
	}
}
