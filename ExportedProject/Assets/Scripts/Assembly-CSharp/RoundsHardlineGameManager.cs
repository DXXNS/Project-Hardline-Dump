using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using UnityEngine;

public class RoundsHardlineGameManager : HardlineGameManager
{
	private static bool useDebugMode;

	[SerializeField]
	private int openUplinkStationsTime = 50;

	[SerializeField]
	private int revealUplinkStationsTime = 20;

	private static int winSupplyReward;

	private static int killSupplyReward;

	private static int roundFinishSupplyReward;

	[SerializeField]
	private float uplinkProgressRate = 5f;

	private static float disableDisplayTime;

	private GameObject uplinkStations;

	[SyncVar]
	private int uplinkNumber;

	protected bool roundEndFlag;

	protected bool gameToResetFlag;

	[SerializeField]
	private int team1Wins;

	[SerializeField]
	private int team2Wins;

	[SerializeField]
	private int team1SubWins;

	[SerializeField]
	private int team2SubWins;

	private float gameEndDelay = 4f;

	private float serverCloseOnGameEndDelay = 8f;

	private float clientLeaveOnGameEndDelay = 6f;

	[SerializeField]
	private int roundsToWin;

	[SerializeField]
	private GameObject loadOutSelect;

	private List<LoadoutSelectable> loadOutSelectables = new List<LoadoutSelectable>();

	private List<LoadoutSelectable> specialistLoadoutSelectables = new List<LoadoutSelectable>();

	[SerializeField]
	private List<List<PlayerItem>> possiblePrimaryItems = new List<List<PlayerItem>>();

	[SerializeField]
	private List<List<PlayerItem>> possibleSecondaryItems = new List<List<PlayerItem>>();

	[SerializeField]
	private List<List<PlayerItem>> possibleEquipment1 = new List<List<PlayerItem>>();

	[SerializeField]
	private List<List<PlayerItem>> possibleEquipment2 = new List<List<PlayerItem>>();

	[SerializeField]
	private List<List<PlayerItem>> possibleMeleeWeapons = new List<List<PlayerItem>>();

	private RoundsUserInterface gameUI;

	[SyncVar]
	[SerializeField]
	private int gameStartTimer;

	[SerializeField]
	private int countdownTime;

	[SyncVar]
	[SerializeField]
	private int timer;

	private int team1Alive;

	private int team2Alive;

	private bool uplinkRevealFlag;

	private bool uplinkOpenFlag;

	[SerializeField]
	private ChatHandler chatHandler;

	private MultiplayerNetworkManager networkManager;

	public GameObject LoadOutSelect
	{
		get
		{
			return loadOutSelect;
		}
		set
		{
			loadOutSelect = value;
		}
	}

	public int Team1Alive
	{
		get
		{
			return team1Alive;
		}
		set
		{
			team1Alive = value;
		}
	}

	public int Team2Alive
	{
		get
		{
			return team2Alive;
		}
		set
		{
			team2Alive = value;
		}
	}

	public GameObject UplinkStations
	{
		get
		{
			return uplinkStations;
		}
		set
		{
			uplinkStations = value;
		}
	}

	public int Team1Wins
	{
		get
		{
			return team1Wins;
		}
		set
		{
			team1Wins = value;
		}
	}

	public int Team2Wins
	{
		get
		{
			return team2Wins;
		}
		set
		{
			team2Wins = value;
		}
	}

	public float UplinkProgressRate
	{
		get
		{
			return uplinkProgressRate;
		}
		set
		{
			uplinkProgressRate = value;
		}
	}

	public bool RoundEndFlag
	{
		get
		{
			return roundEndFlag;
		}
		set
		{
			roundEndFlag = value;
		}
	}

	public int NetworkuplinkNumber
	{
		get
		{
			return uplinkNumber;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref uplinkNumber))
			{
				int num = uplinkNumber;
				SetSyncVar(value, ref uplinkNumber, 2uL);
			}
		}
	}

	public int NetworkgameStartTimer
	{
		get
		{
			return gameStartTimer;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref gameStartTimer))
			{
				int num = gameStartTimer;
				SetSyncVar(value, ref gameStartTimer, 4uL);
			}
		}
	}

	public int Networktimer
	{
		get
		{
			return timer;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref timer))
			{
				int num = timer;
				SetSyncVar(value, ref timer, 8uL);
			}
		}
	}

	public void GeneratePossibleItems()
	{
		PlayerItem playerItem = GenerateItem("Weapon_Unarmed");
		PlayerItem playerItem2 = GenerateItem("Weapon_StimPen");
		PlayerItem playerItem3 = GenerateItem("Weapon_FragGrenade");
		PlayerItem playerItem4 = GenerateItem("Weapon_Glock", 0, 0, 0);
		PlayerItem playerItem5 = GenerateItem("Weapon_Glock", 0, 1, 0);
		PlayerItem playerItem6 = GenerateItem("Weapon_Glock", 1, 1, 0);
		PlayerItem playerItem7 = GenerateItem("Weapon_M45", 0, 0, 0);
		PlayerItem playerItem8 = GenerateItem("Weapon_M45", 1, 0, 0);
		PlayerItem playerItem9 = GenerateItem("Weapon_M45", 2, 0, 0);
		PlayerItem playerItem10 = GenerateItem("Weapon_M45", 0, 1, 0);
		PlayerItem playerItem11 = GenerateItem("Weapon_M416", 0, 0, 0);
		PlayerItem playerItem12 = GenerateItem("Weapon_M416", 0, 0, 1);
		PlayerItem playerItem13 = GenerateItem("Weapon_M416", 1, 1, 1);
		PlayerItem playerItem14 = GenerateItem("Weapon_M416", 2, 2, 1);
		PlayerItem playerItem15 = GenerateItem("Weapon_M416", 2, 2, 2);
		PlayerItem playerItem16 = GenerateItem("Weapon_M416", 3, 3, 2);
		PlayerItem playerItem17 = GenerateItem("Weapon_M416", 4, 3, 1);
		PlayerItem playerItem18 = GenerateItem("Weapon_M416", 4, 3, 1);
		PlayerItem playerItem19 = GenerateItem("Weapon_M416", 3, 2, 2);
		PlayerItem playerItem20 = GenerateItem("Weapon_M416", 5, 3, 2);
		PlayerItem playerItem21 = GenerateItem("Weapon_M416", 6, 3, 2);
		PlayerItem playerItem22 = GenerateItem("Weapon_AK15", 0, 0, 0);
		PlayerItem playerItem23 = GenerateItem("Weapon_AK15", 0, 0, 1);
		PlayerItem playerItem24 = GenerateItem("Weapon_AK15", 2, 2, 1);
		PlayerItem playerItem25 = GenerateItem("Weapon_AK15", 3, 3, 2);
		PlayerItem playerItem26 = GenerateItem("Weapon_AK15", 4, 3, 1);
		PlayerItem playerItem27 = GenerateItem("Weapon_AK15", 6, 3, 2);
		PlayerItem playerItem28 = GenerateItem("Weapon_M249", 0, 0, 0);
		PlayerItem playerItem29 = GenerateItem("Weapon_M249", 0, 0, 1);
		PlayerItem playerItem30 = GenerateItem("Weapon_M249", 3, 3, 1);
		PlayerItem playerItem31 = GenerateItem("Weapon_M249", 4, 0, 0);
		PlayerItem playerItem32 = GenerateItem("Weapon_M249", 6, 3, 2);
		PlayerItem playerItem33 = GenerateItem("Weapon_P90", 0, 0, 0);
		PlayerItem playerItem34 = GenerateItem("Weapon_P90", 0, 3, 0);
		PlayerItem playerItem35 = GenerateItem("Weapon_P90", 1, 2, 0);
		PlayerItem playerItem36 = GenerateItem("Weapon_P90", 2, 3, 0);
		PlayerItem playerItem37 = GenerateItem("Weapon_P90", 5, 3, 0);
		PlayerItem playerItem38 = GenerateItem("Weapon_P90", 3, 3, 0);
		PlayerItem playerItem39 = GenerateItem("Weapon_M24", 0, 0, 0);
		GenerateItem("Weapon_M24", 1, 1, 0);
		PlayerItem playerItem40 = GenerateItem("Weapon_M24", 4, 3, 0);
		PlayerItem playerItem41 = GenerateItem("Weapon_M24", 5, 1, 0);
		PlayerItem playerItem42 = GenerateItem("Weapon_M24", 5, 3, 0);
		PlayerItem playerItem43 = GenerateItem("Weapon_Scout", 0, 0, 0);
		PlayerItem playerItem44 = GenerateItem("Weapon_Scout", 2, 1, 0);
		GenerateItem("Weapon_Scout", 1, 1, 0);
		PlayerItem playerItem45 = GenerateItem("Weapon_Scout", 4, 3, 0);
		PlayerItem playerItem46 = GenerateItem("Weapon_Scout", 5, 1, 0);
		PlayerItem playerItem47 = GenerateItem("Weapon_Scout", 5, 3, 0);
		PlayerItem playerItem48 = GenerateItem("Weapon_M870", 0, 0, 0);
		PlayerItem playerItem49 = GenerateItem("Weapon_M870", 2, 2, 0);
		PlayerItem playerItem50 = GenerateItem("Weapon_M870", 3, 1, 0);
		PlayerItem playerItem51 = GenerateItem("Weapon_SA58", 0, 0, 0);
		PlayerItem playerItem52 = GenerateItem("Weapon_SA58", 2, 2, 2);
		PlayerItem playerItem53 = GenerateItem("Weapon_SA58", 4, 2, 2);
		PlayerItem playerItem54 = GenerateItem("Weapon_SA58", 4, 3, 2);
		PlayerItem playerItem55 = GenerateItem("Weapon_SA58", 5, 1, 2);
		PlayerItem playerItem56 = GenerateItem("Weapon_SA58", 5, 3, 1);
		PlayerItem playerItem57 = GenerateItem("Weapon_SA58", 6, 3, 1);
		PlayerItem playerItem58 = GenerateItem("Weapon_X22", 0, 0, 0);
		PlayerItem playerItem59 = GenerateItem("Weapon_X22", 0, 3, 0);
		PlayerItem playerItem60 = GenerateItem("Weapon_X22", 2, 3, 1);
		PlayerItem playerItem61 = GenerateItem("Weapon_X22", 4, 3, 0);
		PlayerItem playerItem62 = GenerateItem("Weapon_Knife");
		PlayerItem[] collection = new PlayerItem[1] { playerItem };
		PlayerItem[] collection2 = new PlayerItem[7] { playerItem5, playerItem4, playerItem7, playerItem10, playerItem58, playerItem9, playerItem6 };
		PlayerItem[] collection3 = new PlayerItem[3] { playerItem, playerItem, playerItem2 };
		PlayerItem[] collection4 = new PlayerItem[1] { playerItem };
		PlayerItem[] collection5 = new PlayerItem[1] { playerItem62 };
		possiblePrimaryItems.Add(new List<PlayerItem>(collection));
		possibleSecondaryItems.Add(new List<PlayerItem>(collection2));
		possibleEquipment1.Add(new List<PlayerItem>(collection3));
		possibleEquipment2.Add(new List<PlayerItem>(collection4));
		possibleMeleeWeapons.Add(new List<PlayerItem>(collection5));
		PlayerItem[] collection6 = new PlayerItem[3] { playerItem33, playerItem48, playerItem43 };
		PlayerItem[] collection7 = new PlayerItem[6] { playerItem5, playerItem4, playerItem7, playerItem10, playerItem58, playerItem6 };
		possiblePrimaryItems.Add(new List<PlayerItem>(collection6));
		possibleSecondaryItems.Add(new List<PlayerItem>(collection7));
		possibleEquipment1.Add(new List<PlayerItem>(collection3));
		possibleEquipment2.Add(new List<PlayerItem>(collection4));
		possibleMeleeWeapons.Add(new List<PlayerItem>(collection5));
		PlayerItem[] collection8 = new PlayerItem[11]
		{
			playerItem35, playerItem38, playerItem49, playerItem50, playerItem11, playerItem44, playerItem46, playerItem39, playerItem51, playerItem28,
			playerItem22
		};
		PlayerItem[] collection9 = new PlayerItem[8] { playerItem5, playerItem4, playerItem7, playerItem10, playerItem59, playerItem60, playerItem8, playerItem9 };
		PlayerItem[] collection10 = new PlayerItem[2] { playerItem, playerItem2 };
		PlayerItem[] collection11 = new PlayerItem[3] { playerItem, playerItem3, playerItem };
		possiblePrimaryItems.Add(new List<PlayerItem>(collection8));
		possibleSecondaryItems.Add(new List<PlayerItem>(collection9));
		possibleEquipment1.Add(new List<PlayerItem>(collection10));
		possibleEquipment2.Add(new List<PlayerItem>(collection11));
		possibleMeleeWeapons.Add(new List<PlayerItem>(collection5));
		PlayerItem[] collection12 = new PlayerItem[22]
		{
			playerItem36, playerItem34, playerItem35, playerItem49, playerItem12, playerItem19, playerItem13, playerItem17, playerItem41, playerItem45,
			playerItem53, playerItem55, playerItem31, playerItem29, playerItem30, playerItem22, playerItem23, playerItem24, playerItem24, playerItem26,
			playerItem46, playerItem47
		};
		PlayerItem[] collection13 = new PlayerItem[7] { playerItem8, playerItem10, playerItem60, playerItem59, playerItem6, playerItem8, playerItem9 };
		possiblePrimaryItems.Add(new List<PlayerItem>(collection12));
		possibleSecondaryItems.Add(new List<PlayerItem>(collection13));
		possibleEquipment1.Add(new List<PlayerItem>(collection10));
		possibleEquipment2.Add(new List<PlayerItem>(collection11));
		possibleMeleeWeapons.Add(new List<PlayerItem>(collection5));
		PlayerItem[] collection14 = new PlayerItem[30]
		{
			playerItem18, playerItem17, playerItem14, playerItem15, playerItem19, playerItem20, playerItem16, playerItem16, playerItem36, playerItem34,
			playerItem49, playerItem50, playerItem42, playerItem40, playerItem56, playerItem54, playerItem52, playerItem21, playerItem57, playerItem37,
			playerItem31, playerItem29, playerItem30, playerItem32, playerItem26, playerItem25, playerItem27, playerItem24, playerItem47, playerItem45
		};
		PlayerItem[] collection15 = new PlayerItem[9] { playerItem5, playerItem10, playerItem61, playerItem59, playerItem60, playerItem58, playerItem6, playerItem9, playerItem8 };
		PlayerItem[] collection16 = new PlayerItem[1] { playerItem2 };
		PlayerItem[] collection17 = new PlayerItem[2] { playerItem3, playerItem2 };
		possiblePrimaryItems.Add(new List<PlayerItem>(collection14));
		possibleSecondaryItems.Add(new List<PlayerItem>(collection15));
		possibleEquipment1.Add(new List<PlayerItem>(collection16));
		possibleEquipment2.Add(new List<PlayerItem>(collection17));
		possibleMeleeWeapons.Add(new List<PlayerItem>(collection5));
	}

	public void AsssignSpecialistLoadouts()
	{
		for (int i = 0; i <= 2; i++)
		{
			PlayerItem primary = GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + (i + 1) + "_primary", "M416"), PlayerPrefs.GetInt("loadout_" + (i + 1) + "_primary_sight", 0), PlayerPrefs.GetInt("loadout_" + (i + 1) + "_primary_barrel", 0), PlayerPrefs.GetInt("loadout_" + (i + 1) + "_primary_grip", 0));
			PlayerItem secondary = GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + (i + 1) + "_secondary", "Glock"), PlayerPrefs.GetInt("loadout_" + (i + 1) + "_secondary_sight", 0), PlayerPrefs.GetInt("loadout_" + (i + 1) + "_secondary_barrel", 0), PlayerPrefs.GetInt("loadout_" + (i + 1) + "_secondary_grip", 0));
			PlayerItem equipment = GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + (i + 1) + "_equipment1", "FragGrenade"));
			PlayerItem equipment2 = GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + (i + 1) + "_equipment2", "StimPen"));
			PlayerItem meleeWeapon = GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + (i + 1) + "_meleeWeapon", "Knife"));
			specialistLoadoutSelectables[i].SetLoadout("Specialist Kit " + (i + 1), primary, secondary, equipment, equipment2, meleeWeapon);
		}
	}

	[ClientRpc]
	public void RpcSetTimer(int timer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(timer);
		SendRPCInternal(typeof(RoundsHardlineGameManager), "RpcSetTimer", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetGameStartTimer(int timer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(timer);
		SendRPCInternal(typeof(RoundsHardlineGameManager), "RpcSetGameStartTimer", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSyncSubGameWin(int winningTeam)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(winningTeam);
		SendRPCInternal(typeof(RoundsHardlineGameManager), "RpcSyncSubGameWin", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void SubGameWin(int winningTeam)
	{
		roundEndFlag = true;
		MonoBehaviour.print("round sub win B");
		switch (winningTeam)
		{
		case 1:
		{
			Player player2 = GetLocalPlayer();
			base.PlayerSupplyPoints += roundFinishSupplyReward;
			if (player2.Team == 1)
			{
				base.PlayerSupplyPoints += winSupplyReward;
			}
			team1SubWins++;
			if (team1SubWins >= 2)
			{
				Team1Wins++;
				team1SubWins = 0;
				team2SubWins = 0;
				gameUI.ShowNotification("team 1 wins round");
			}
			else
			{
				gameUI.ShowNotification("team 1 wins half");
			}
			break;
		}
		case 2:
		{
			Player player = GetLocalPlayer();
			base.PlayerSupplyPoints += roundFinishSupplyReward;
			if (player.Team == 2)
			{
				base.PlayerSupplyPoints += winSupplyReward;
			}
			team2SubWins++;
			if (team2SubWins >= 2)
			{
				Team2Wins++;
				team2SubWins = 0;
				team1SubWins = 0;
				gameUI.ShowNotification("team 2 wins round");
			}
			else
			{
				gameUI.ShowNotification("team 2 wins half");
			}
			break;
		}
		}
		gameUI.UpdateWinsDisplay(Team1Wins, team1SubWins, Team2Wins, team2SubWins);
		if (Team1Wins >= roundsToWin || Team2Wins >= roundsToWin)
		{
			if (Team1Wins >= roundsToWin)
			{
				EndGame(1);
			}
			else
			{
				EndGame(2);
			}
		}
		else
		{
			Invoke("RestartGameCharacter", gameEndDelay);
		}
	}

	public void SetTimer(int timer)
	{
		Networktimer = timer;
		gameUI.SetTimer(timer);
		if (this.timer <= revealUplinkStationsTime && !uplinkRevealFlag)
		{
			RevealUplinkStations();
		}
		if (this.timer <= 0)
		{
			if (!uplinkOpenFlag)
			{
				OpenUplinkStations();
			}
			CancelInvoke("CountdownTimer");
		}
	}

	[ClientRpc]
	public void RpcSetUplinkNumber(int number)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(number);
		SendRPCInternal(typeof(RoundsHardlineGameManager), "RpcSetUplinkNumber", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void SetUplinkNumber(int number)
	{
		uplinkStations = GameObject.Find("UplinkStations");
		NetworkuplinkNumber = number;
		for (int i = 0; i < UplinkStations.transform.childCount; i++)
		{
			UplinkStations.transform.GetChild(i).GetComponent<UplinkStation>().ResetUplink();
			if (i == uplinkNumber)
			{
				UplinkStations.transform.GetChild(i).gameObject.SetActive(value: true);
				UplinkStations.transform.GetChild(i).GetComponent<UplinkStation>().ReplicateActiveUplink(active: true);
			}
			else
			{
				UplinkStations.transform.GetChild(i).GetComponent<UplinkStation>().ReplicateActiveUplink(active: false);
				UplinkStations.transform.GetChild(i).gameObject.SetActive(value: true);
			}
		}
	}

	public void RevealUplinkStations()
	{
		uplinkStations = GameObject.Find("UplinkStations");
		uplinkRevealFlag = true;
		gameUI.ShowNotification("Uplink Station has been revealed");
		UplinkStation[] componentsInChildren = UplinkStations.GetComponentsInChildren<UplinkStation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Visible = true;
		}
	}

	public void OpenUplinkStations()
	{
		uplinkOpenFlag = true;
		gameUI.ShowNotification("Uplink Station has been opened, capture it!");
		UplinkStation[] componentsInChildren = UplinkStations.GetComponentsInChildren<UplinkStation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Interactable = true;
		}
	}

	public void DetermineRandomUplinkStation()
	{
		uplinkStations = GameObject.Find("UplinkStations");
		if (base.isServer)
		{
			int number = Random.Range(0, UplinkStations.transform.childCount - 1);
			SetUplinkNumber(number);
			RpcSetUplinkNumber(number);
		}
	}

	public void SetGameStartTimer(int gameTimer)
	{
		NetworkgameStartTimer = gameTimer;
		gameUI.SetGameStartTimer(gameTimer);
		if (gameStartTimer <= 0)
		{
			if (base.isServer)
			{
				roundEndFlag = false;
			}
			gameUI.HideGameStartTimer();
			CancelInvoke("CountdownGameStartTimer");
			gameUI.SetGameStartTimer(gameTimer);
			StartTimer(openUplinkStationsTime);
		}
		else
		{
			gameUI.ShowGameStartTimer();
		}
	}

	public void EndGame(int winningTeam)
	{
		switch (winningTeam)
		{
		case 1:
			gameUI.ShowNotification("team 1 has won the game");
			break;
		case 2:
			gameUI.ShowNotification("team 2 has won the game");
			break;
		}
		if (base.isServer)
		{
			Invoke("ShutdownGame", serverCloseOnGameEndDelay);
		}
		else
		{
			Invoke("ShutdownGame", clientLeaveOnGameEndDelay);
		}
	}

	public void ShutdownGame()
	{
		Object.FindObjectOfType<HardlineNetworkManager>().LeaveToMenuAndDisconnect();
	}

	public override void RestartGameCharacter()
	{
		base.RestartGameCharacter();
		OpenLoadoutSelect();
		StartGameStartTimer(countdownTime);
		RestartLevel();
		CancelInvoke("CountdownTimer");
		Networktimer = openUplinkStationsTime;
		base.LocalPlayer.ResetInputs();
		Ragdoll[] array = Object.FindObjectsOfType<Ragdoll>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	public void Awake()
	{
		gameUI = Object.FindObjectOfType<RoundsUserInterface>();
	}

	protected override void Start()
	{
		base.Start();
		roundEndFlag = false;
		loadOutSelectables = new List<LoadoutSelectable>(gameUI.LoadoutSelectables);
		specialistLoadoutSelectables = new List<LoadoutSelectable>(gameUI.SpecialistLoadoutSelectables);
		GeneratePossibleItems();
		chatHandler = Object.FindObjectOfType<ChatHandler>();
	}

	public override void AllPlayersLoaded()
	{
		base.AllPlayersLoaded();
		AsssignSpecialistLoadouts();
		OpenLoadoutSelect();
		DetermineRandomUplinkStation();
		gameUI.UpdateWinsDisplay(Team1Wins, team1SubWins, Team2Wins, team2SubWins);
	}

	protected override void Update()
	{
		if (base.LocalPlayer == null)
		{
			InitPlayersAndFindLocalPlayer();
		}
		if (humans.Count <= 0)
		{
			FindPlayers();
		}
		if (!networkManager)
		{
			networkManager = Object.FindObjectOfType<MultiplayerNetworkManager>();
		}
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(HardlineGameManager.currentLobby);
		if (!loadedAllPlayersFlag && base.PlayersLoaded >= numLobbyMembers)
		{
			networkManager.AllPlayersLoaded();
			gameUI.AllPlayersLoaded = true;
			gameUI.UpdateLoadingPlayerScreen();
			if (base.isServer)
			{
				StartGameStartTimer(countdownTime);
			}
			loadedAllPlayersFlag = true;
			InitSceneObjects();
		}
		UpdateCursorState();
		UpdatePlayerLockState();
		if ((bool)gameUI)
		{
			gameUI.UpdateSupplyPointsText(base.PlayerSupplyPoints);
		}
		if (base.isServer)
		{
			RoundMode();
		}
	}

	public void RoundMode()
	{
		if (humans.Count < 2)
		{
			return;
		}
		team1Alive = 0;
		team2Alive = 0;
		for (int i = 0; i < humans.Count; i++)
		{
			if (humans[i] == null)
			{
				humans.RemoveAt(i);
			}
			if (humans[i].Health >= 0f)
			{
				if (humans[i].Team == 1)
				{
					team1Alive++;
				}
				else if (humans[i].Team == 2)
				{
					team2Alive++;
				}
			}
		}
		if (!roundEndFlag && !useDebugMode && !roundEndFlag)
		{
			if (team1Alive <= 0)
			{
				ReplicateSubGameWin(2);
			}
			else if (team2Alive <= 0)
			{
				ReplicateSubGameWin(1);
			}
		}
	}

	public override void UpdateCursorState()
	{
		if (!(!loadOutSelect | !base.LocalPlayer))
		{
			if (base.LocalPlayer.MenuOpened | loadOutSelect.activeSelf)
			{
				gameUI.EnableCursor();
			}
			else
			{
				gameUI.DisableCursor();
			}
		}
	}

	public override void UpdatePlayerLockState()
	{
		if (!base.LocalPlayer | !loadOutSelect)
		{
			return;
		}
		if (base.LocalPlayer.MenuOpened | loadOutSelect.activeSelf | chatHandler.InputField.isFocused)
		{
			base.LocalPlayer.InputEnabled = false;
			base.LocalPlayer.LookEnabled = false;
			return;
		}
		base.LocalPlayer.LookEnabled = true;
		if (gameStartTimer > 0)
		{
			base.LocalPlayer.InputEnabled = false;
		}
		else
		{
			base.LocalPlayer.InputEnabled = true;
		}
	}

	public void ReplicateSubGameWin(int team)
	{
		if (base.isServer && !roundEndFlag)
		{
			SubGameWin(team);
			RpcSyncSubGameWin(team);
			roundEndFlag = true;
		}
	}

	public void RewardSupplyForKill()
	{
		base.PlayerSupplyPoints += killSupplyReward;
	}

	public void OpenLoadoutSelect()
	{
		if (!base.LocalPlayer)
		{
			return;
		}
		int tier = 0;
		switch (base.LocalPlayer.Team)
		{
		case 1:
			tier = Team2Wins;
			break;
		case 2:
			tier = Team1Wins;
			break;
		}
		base.LocalPlayer.LoadoutSelectOpened = true;
		base.LocalPlayer.InputEnabled = false;
		gameUI.ResetKitsScreen();
		foreach (LoadoutSelectable loadOutSelectable in loadOutSelectables)
		{
			loadOutSelectable.SetLoadout("Loadout", GenerateRandomLoadout(tier));
		}
		LoadOutSelect.SetActive(value: true);
		loadOutSelect.GetComponent<Animator>().SetTrigger("Show");
	}

	public void CloseLoadOutSelect()
	{
		loadOutSelect.GetComponent<Animator>().SetTrigger("Hide");
		Invoke("DisableLoadoutSelect", disableDisplayTime);
	}

	public void DisableLoadoutSelect()
	{
		LoadOutSelect.SetActive(value: false);
		base.LocalPlayer.LoadoutSelectOpened = false;
	}

	public void EnableLoadoutSelect()
	{
		LoadOutSelect.SetActive(value: true);
	}

	public Inventory GenerateRandomLoadout(int tier)
	{
		Inventory inventory = new Inventory();
		int index = Random.Range(0, possiblePrimaryItems[tier].Count);
		int index2 = Random.Range(0, possibleSecondaryItems[tier].Count);
		int num = Random.Range(0, possibleEquipment1[tier].Count);
		int index3 = Random.Range(0, possibleEquipment2[tier].Count);
		int index4 = Random.Range(0, possibleMeleeWeapons[tier].Count);
		inventory.PrimaryItem = possiblePrimaryItems[tier][index];
		inventory.SecondaryItem = possibleSecondaryItems[tier][index2];
		inventory.Equipment1 = possibleEquipment1[tier][num];
		inventory.MeleeWeapon = possibleMeleeWeapons[tier][index4];
		if (num != 0)
		{
			inventory.Equipment2 = possibleEquipment2[tier][index3];
		}
		else
		{
			inventory.Equipment2 = possibleEquipment2[tier][0];
			inventory.Equipment1 = possibleEquipment2[tier][index3];
		}
		return inventory;
	}

	public void StartGameStartTimer(int time)
	{
		if (base.isServer)
		{
			SetGameStartTimer(time);
			RpcSetGameStartTimer(time);
			InvokeRepeating("CountdownGameStartTimer", 1f, 1f);
		}
	}

	public void StartTimer(int time)
	{
		if (base.isServer)
		{
			SetTimer(time);
			RpcSetTimer(time);
			CancelInvoke("CountdownTimer");
			InvokeRepeating("CountdownTimer", 1f, 1f);
		}
	}

	public void CountdownGameStartTimer()
	{
		if (base.isServer)
		{
			SetGameStartTimer(gameStartTimer - 1);
			RpcSetGameStartTimer(gameStartTimer);
		}
	}

	public void CountdownTimer()
	{
		if (base.isServer)
		{
			SetTimer(timer - 1);
			RpcSetTimer(timer);
		}
	}

	public void RestartLevel()
	{
		uplinkOpenFlag = false;
		uplinkRevealFlag = false;
		ResetUplinks();
		DeleteSpectatorCamera();
		DetermineRandomUplinkStation();
		InitSceneObjects();
	}

	public void ResetUplinks()
	{
		UplinkStation[] array = Object.FindObjectsOfType<UplinkStation>(includeInactive: true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetUplink();
		}
	}

	public void DeleteSpectatorCamera()
	{
		if ((bool)Object.FindObjectOfType<SpectatorCamera>())
		{
			Object.Destroy(Object.FindObjectOfType<SpectatorCamera>().gameObject);
		}
	}

	public new Player GetLocalPlayer()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.isLocalPlayer)
			{
				return player;
			}
		}
		return null;
	}

	public void UpdateUplinkInteract(int controlState)
	{
		if (base.LocalPlayer.Team == controlState)
		{
			gameUI.ShowNotification("Your team is capturing the uplink station, help defend it!");
		}
		else
		{
			gameUI.ShowNotification("The opposing team is capturing the uplink station, stop them from completing the capture!");
		}
	}

	static RoundsHardlineGameManager()
	{
		useDebugMode = false;
		winSupplyReward = 1;
		killSupplyReward = 1;
		roundFinishSupplyReward = 3;
		disableDisplayTime = 0.4f;
		RemoteCallHelper.RegisterRpcDelegate(typeof(RoundsHardlineGameManager), "RpcSetTimer", InvokeUserCode_RpcSetTimer);
		RemoteCallHelper.RegisterRpcDelegate(typeof(RoundsHardlineGameManager), "RpcSetGameStartTimer", InvokeUserCode_RpcSetGameStartTimer);
		RemoteCallHelper.RegisterRpcDelegate(typeof(RoundsHardlineGameManager), "RpcSyncSubGameWin", InvokeUserCode_RpcSyncSubGameWin);
		RemoteCallHelper.RegisterRpcDelegate(typeof(RoundsHardlineGameManager), "RpcSetUplinkNumber", InvokeUserCode_RpcSetUplinkNumber);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_RpcSetTimer(int timer)
	{
		if (!base.isServer)
		{
			SetTimer(timer);
		}
	}

	protected static void InvokeUserCode_RpcSetTimer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetTimer called on server.");
		}
		else
		{
			((RoundsHardlineGameManager)obj).UserCode_RpcSetTimer(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetGameStartTimer(int timer)
	{
		if (!base.isServer)
		{
			SetGameStartTimer(timer);
		}
	}

	protected static void InvokeUserCode_RpcSetGameStartTimer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetGameStartTimer called on server.");
		}
		else
		{
			((RoundsHardlineGameManager)obj).UserCode_RpcSetGameStartTimer(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSyncSubGameWin(int winningTeam)
	{
		if (!base.isServer)
		{
			SubGameWin(winningTeam);
		}
	}

	protected static void InvokeUserCode_RpcSyncSubGameWin(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSyncSubGameWin called on server.");
		}
		else
		{
			((RoundsHardlineGameManager)obj).UserCode_RpcSyncSubGameWin(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetUplinkNumber(int number)
	{
		if (!base.isServer)
		{
			SetUplinkNumber(number);
		}
	}

	protected static void InvokeUserCode_RpcSetUplinkNumber(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetUplinkNumber called on server.");
		}
		else
		{
			((RoundsHardlineGameManager)obj).UserCode_RpcSetUplinkNumber(reader.ReadInt());
		}
	}

	public override bool SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		bool result = base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(uplinkNumber);
			writer.WriteInt(gameStartTimer);
			writer.WriteInt(timer);
			return true;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(uplinkNumber);
			result = true;
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(gameStartTimer);
			result = true;
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteInt(timer);
			result = true;
		}
		return result;
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			int num = uplinkNumber;
			NetworkuplinkNumber = reader.ReadInt();
			int num2 = gameStartTimer;
			NetworkgameStartTimer = reader.ReadInt();
			int num3 = timer;
			Networktimer = reader.ReadInt();
			return;
		}
		long num4 = (long)reader.ReadULong();
		if ((num4 & 2L) != 0L)
		{
			int num5 = uplinkNumber;
			NetworkuplinkNumber = reader.ReadInt();
		}
		if ((num4 & 4L) != 0L)
		{
			int num6 = gameStartTimer;
			NetworkgameStartTimer = reader.ReadInt();
		}
		if ((num4 & 8L) != 0L)
		{
			int num7 = timer;
			Networktimer = reader.ReadInt();
		}
	}
}
