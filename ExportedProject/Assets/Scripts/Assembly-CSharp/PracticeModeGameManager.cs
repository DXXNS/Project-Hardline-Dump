using System.Collections.Generic;
using UnityEngine;

public class PracticeModeGameManager : HardlineGameManager
{
	private static int SHOW_REMAINING_ENEMIES_WHEN_ALIVE = 2;

	private static float WIN_GAME_UI_DELAY = 1f;

	private static float LOSE_GAME_UI_DELAY = 2f;

	private PracticeModeUI gameUI;

	[SerializeField]
	private GameObject loadOutSelect;

	private List<LoadoutSelectable> loadOutSelectables = new List<LoadoutSelectable>();

	private List<LoadoutSelectable> specialistLoadoutSelectables = new List<LoadoutSelectable>();

	public static EnemyAI.EnemyDifficulties difficulty;

	private static float disableDisplayTime = 0.4f;

	[SerializeField]
	private int enemiesAlive = -1;

	private int elapsedTime;

	private bool hardgameFlag;

	private bool endgameFlag;

	public void Awake()
	{
		enemiesAlive = -1;
		gameUI = Object.FindObjectOfType<PracticeModeUI>();
	}

	protected override void Start()
	{
		loadOutSelectables = new List<LoadoutSelectable>(gameUI.LoadoutSelectables);
		specialistLoadoutSelectables = new List<LoadoutSelectable>(gameUI.SpecialistLoadoutSelectables);
		Object.FindObjectOfType<MultiplayerNetworkManager>().AllPlayersLoaded();
		base.Start();
	}

	public override void AllPlayersLoaded()
	{
		hardgameFlag = false;
		endgameFlag = false;
		base.AllPlayersLoaded();
		AsssignSpecialistLoadouts();
		OpenLoadoutSelect();
		InitSceneObjects();
	}

	public void PlayerSelectedLoadout()
	{
		gameUI.ShowNotification("Eliminate all enemies to win");
		Spawner[] array = Object.FindObjectsOfType<Spawner>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnDesignatedObject();
		}
		gameUI.GetPlayers();
		elapsedTime = 0;
		FindPlayers();
		if (IsInvoking("IncrementTimer"))
		{
			CancelInvoke("IncrementTimer");
		}
		InvokeRepeating("IncrementTimer", 1f, 1f);
	}

	public void IncrementTimer()
	{
		elapsedTime++;
		if ((bool)gameUI)
		{
			gameUI.SetTimer(elapsedTime);
		}
	}

	protected override void Update()
	{
		base.Update();
		UpdateCursorState();
		UpdatePlayerLockState();
		UpdateGameState();
	}

	public void UpdateGameState()
	{
		if (!endgameFlag)
		{
			if (base.LocalPlayer.Health <= 0f)
			{
				CancelInvoke("IncrementTimer");
				Invoke("UILoseGame", LOSE_GAME_UI_DELAY);
				endgameFlag = true;
			}
			if (enemiesAlive == 0)
			{
				CancelInvoke("IncrementTimer");
				Invoke("UIWinGame", WIN_GAME_UI_DELAY);
				enemiesAlive = -1;
				endgameFlag = true;
			}
		}
	}

	public void UILoseGame()
	{
		gameUI.ToggleGameOverScreen(state: true, win: false);
	}

	public void UIWinGame()
	{
		gameUI.ToggleGameOverScreen(state: true, win: true);
	}

	public void FindEnemiesAlive()
	{
		enemiesAlive = 0;
		foreach (Human human in humans)
		{
			if (human.Health > 0f && human is EnemyAI)
			{
				enemiesAlive++;
			}
		}
		if (enemiesAlive <= SHOW_REMAINING_ENEMIES_WHEN_ALIVE && !hardgameFlag)
		{
			hardgameFlag = true;
			SetEndGameMode();
		}
	}

	private void SetEndGameMode()
	{
		gameUI.ShowNotification("Enemies and Player have been revealed!");
		EnemyAI[] array = Object.FindObjectsOfType<EnemyAI>();
		foreach (EnemyAI obj in array)
		{
			obj.SetNameTagVisibility(visible: true);
			obj.SetAggressiveMode();
		}
	}

	public void OpenLoadoutSelect()
	{
		if ((bool)base.LocalPlayer)
		{
			base.LocalPlayer.LoadoutSelectOpened = true;
			base.LocalPlayer.InputEnabled = false;
			gameUI.ResetKitsScreen();
			loadOutSelect.SetActive(value: true);
			loadOutSelect.GetComponent<Animator>().SetBool("SpecialistSelect", value: true);
			loadOutSelect.GetComponent<Animator>().SetTrigger("Show");
		}
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

	public void CloseLoadOutSelect()
	{
		loadOutSelect.GetComponent<Animator>().SetTrigger("Hide");
		Invoke("DisableLoadoutSelect", disableDisplayTime);
	}

	public void DisableLoadoutSelect()
	{
		loadOutSelect.SetActive(value: false);
		base.LocalPlayer.LoadoutSelectOpened = false;
	}

	public override void UpdateCursorState()
	{
		if (!(!loadOutSelect | !base.LocalPlayer))
		{
			if (base.LocalPlayer.MenuOpened | loadOutSelect.activeSelf | gameUI.GameOverScreen)
			{
				Object.FindObjectOfType<UserInterface>().EnableCursor();
			}
			else
			{
				Object.FindObjectOfType<UserInterface>().DisableCursor();
			}
		}
	}

	public override void UpdatePlayerLockState()
	{
		if (!(!base.LocalPlayer | !loadOutSelect))
		{
			if (base.LocalPlayer.MenuOpened | loadOutSelect.activeSelf | gameUI.GameOverScreen)
			{
				base.LocalPlayer.InputEnabled = false;
				base.LocalPlayer.LookEnabled = false;
			}
			else
			{
				base.LocalPlayer.LookEnabled = true;
				base.LocalPlayer.InputEnabled = true;
			}
		}
	}

	public void ShutdownGame()
	{
		Object.FindObjectOfType<HardlineNetworkManager>().LeaveToMenuAndDisconnect();
	}

	public void RestartGame()
	{
		base.LocalPlayer.ResetCharacter();
		EnemyAI[] array = Object.FindObjectsOfType<EnemyAI>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
		InitSceneObjects();
		UplinkStation[] array2 = Object.FindObjectsOfType<UplinkStation>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].ResetUplink();
		}
		SpectatorCamera[] array3 = Object.FindObjectsOfType<SpectatorCamera>();
		for (int i = 0; i < array3.Length; i++)
		{
			Object.Destroy(array3[i].gameObject);
		}
	}

	private void MirrorProcessed()
	{
	}
}
