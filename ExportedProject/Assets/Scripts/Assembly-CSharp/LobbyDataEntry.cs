using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDataEntry : MonoBehaviour
{
	[SerializeField]
	private CSteamID lobbyID;

	[SerializeField]
	private string lobbyName;

	[SerializeField]
	private Text lobbyNameText;

	[SerializeField]
	private GameObject lobbyPingIconParent;

	[SerializeField]
	private Text joinable;

	[SerializeField]
	private Text pingIndicator;

	[SerializeField]
	private Text playersInLobby;

	public CSteamID LobbyID
	{
		get
		{
			return lobbyID;
		}
		set
		{
			lobbyID = value;
		}
	}

	public string LobbyName
	{
		get
		{
			return lobbyName;
		}
		set
		{
			lobbyName = value;
		}
	}

	public Text Joinable
	{
		get
		{
			return joinable;
		}
		set
		{
			joinable = value;
		}
	}

	public Text PlayersInLobby
	{
		get
		{
			return playersInLobby;
		}
		set
		{
			playersInLobby = value;
		}
	}

	public Text PingIndicator
	{
		get
		{
			return pingIndicator;
		}
		set
		{
			pingIndicator = value;
		}
	}

	public void SetLobbyData()
	{
		if (LobbyName == "")
		{
			lobbyNameText.text = "lobby";
		}
		else
		{
			lobbyNameText.text = LobbyName;
		}
	}

	public void SetPingDisplay(int ping)
	{
		pingIndicator.text = ping.ToString();
		if (ping <= -1)
		{
			lobbyPingIconParent.transform.GetChild(0).gameObject.SetActive(value: true);
		}
		else if (ping < 100)
		{
			lobbyPingIconParent.transform.GetChild(1).gameObject.SetActive(value: true);
		}
		else if (ping < 200)
		{
			lobbyPingIconParent.transform.GetChild(2).gameObject.SetActive(value: true);
		}
		else
		{
			lobbyPingIconParent.transform.GetChild(3).gameObject.SetActive(value: true);
		}
	}

	public void JoinLobby()
	{
		Object.FindObjectOfType<SteamLobby>().JoinLobby(LobbyID);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
