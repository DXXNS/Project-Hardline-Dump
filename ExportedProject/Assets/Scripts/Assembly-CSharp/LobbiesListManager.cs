using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesListManager : MonoBehaviour
{
	public static LobbiesListManager instance;

	private float baseContentHeight = 200f;

	[SerializeField]
	private float entryPaddingSpace;

	[SerializeField]
	private float entryStartYPos;

	[SerializeField]
	private GameObject lobbiesMenu;

	[SerializeField]
	private GameObject lobbyDataItemPrefab;

	[SerializeField]
	private GameObject lobbyListContent;

	[SerializeField]
	private InputField serverSearchInput;

	[SerializeField]
	private Text loadingServerBrowserText;

	[SerializeField]
	private List<GameObject> listOfLobbies = new List<GameObject>();

	private List<CSteamID> lobbyIDs;

	private LobbyDataUpdate_t result;

	private bool relayNetworkAccessInit;

	private float relayCheckFrequency = 0.5f;

	public List<GameObject> ListOfLobbies
	{
		get
		{
			return listOfLobbies;
		}
		set
		{
			listOfLobbies = value;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Start()
	{
		SteamNetworkingUtils.InitRelayNetworkAccess();
		WaitForRelay();
	}

	public void OnRelayNetworkAccessInit()
	{
	}

	public void DestroyLobbies()
	{
		foreach (GameObject listOfLobby in ListOfLobbies)
		{
			Object.Destroy(listOfLobby);
		}
		ListOfLobbies.Clear();
	}

	public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
	{
		this.lobbyIDs = lobbyIDs;
		this.result = result;
		SteamNetworkingUtils.GetRelayNetworkStatus(out var _);
		for (int i = 0; i < lobbyIDs.Count; i++)
		{
			if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
			{
				GameObject gameObject = Object.Instantiate(lobbyDataItemPrefab);
				gameObject.GetComponent<LobbyDataEntry>().LobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
				SteamNetworkingUtils.ParsePingLocationString(SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "location"), out var remoteLocation);
				int pingDisplay = SteamNetworkingUtils.EstimatePingTimeFromLocalHost(ref remoteLocation);
				gameObject.GetComponent<LobbyDataEntry>().LobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
				gameObject.GetComponent<LobbyDataEntry>().PlayersInLobby.text = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDs[i].m_SteamID) + " / " + SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDs[i].m_SteamID);
				gameObject.GetComponent<LobbyDataEntry>().SetLobbyData();
				gameObject.GetComponent<LobbyDataEntry>().SetPingDisplay(pingDisplay);
				gameObject.GetComponent<LobbyDataEntry>().PingIndicator.text = pingDisplay.ToString();
				gameObject.transform.SetParent(lobbyListContent.transform);
				gameObject.transform.localScale = Vector3.one;
				ListOfLobbies.Add(gameObject);
				lobbyListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(lobbyListContent.GetComponent<RectTransform>().sizeDelta.x, baseContentHeight + Mathf.Abs((float)i * entryPaddingSpace));
				gameObject.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, entryStartYPos + entryPaddingSpace * (float)i, 0f);
				gameObject.SetActive(value: true);
			}
		}
	}

	public void WaitForRelay()
	{
		if (CheckRelayInit())
		{
			relayNetworkAccessInit = true;
			loadingServerBrowserText.gameObject.SetActive(value: false);
			UpdateServerSearch();
		}
		else
		{
			loadingServerBrowserText.gameObject.SetActive(value: true);
			Invoke("WaitForRelay", relayCheckFrequency);
		}
	}

	public bool CheckRelayInit()
	{
		SteamNetworkingUtils.GetRelayNetworkStatus(out var pDetails);
		if (pDetails.m_eAvail == ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current)
		{
			return true;
		}
		return false;
	}

	public void GetListOfLobbies()
	{
		Object.FindObjectOfType<SteamLobby>().GetLobbiesList();
	}

	public void GetListOfLobbies(string lobbyName)
	{
		Object.FindObjectOfType<SteamLobby>().GetLobbiesList(lobbyName);
	}

	public void UpdateServerSearch()
	{
		if (relayNetworkAccessInit)
		{
			string text = serverSearchInput.text;
			if (text == "")
			{
				GetListOfLobbies();
			}
			else
			{
				GetListOfLobbies(text);
			}
		}
	}

	private void Update()
	{
	}
}
