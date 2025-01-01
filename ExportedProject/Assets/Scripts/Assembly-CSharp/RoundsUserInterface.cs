using UnityEngine;
using UnityEngine.UI;

public class RoundsUserInterface : UserInterface
{
	private static float loadoutHiddenPosition = 500f;

	private static float disableGameStartTimerDelay = 0.25f;

	private static float loadingScreenHiddenDelay = 0.5f;

	[SerializeField]
	private LoadoutSelectable[] loadoutSelectables;

	[SerializeField]
	private LoadoutSelectable[] specialistLoadoutSelectables;

	[SerializeField]
	private GameObject itemsMenu;

	[SerializeField]
	private GameObject gameStartTimer;

	[SerializeField]
	private Text gameStartTimerText;

	[SerializeField]
	private GameObject timer;

	[SerializeField]
	private Text timerText;

	[SerializeField]
	private WinDisplay winDisplay;

	[SerializeField]
	private Text team1Left;

	[SerializeField]
	private Text team2Left;

	[SerializeField]
	private GameObject standardKits;

	[SerializeField]
	private GameObject specialistKits;

	[SerializeField]
	private Text supplyPointsText;

	[SerializeField]
	private GameObject timerClick;

	private bool showSpecialistKits;

	private HardlineGameManager gameManager;

	private int lastTeam1Left;

	private int lastTeam2Left;

	public LoadoutSelectable[] LoadoutSelectables
	{
		get
		{
			return loadoutSelectables;
		}
		set
		{
			loadoutSelectables = value;
		}
	}

	public LoadoutSelectable[] SpecialistLoadoutSelectables
	{
		get
		{
			return specialistLoadoutSelectables;
		}
		set
		{
			specialistLoadoutSelectables = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		UpdateLoadingPlayerScreen();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateTeamLeft();
		if (humans.Count <= 0)
		{
			GetPlayers();
		}
		if (!gameManager)
		{
			gameManager = Object.FindObjectOfType<HardlineGameManager>();
		}
	}

	private void UpdateTeamLeft()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < humans.Count; i++)
		{
			if (humans[i] == null)
			{
				humans.RemoveAt(i);
			}
			if (humans[i].Team == 1 && humans[i].Health > 0f)
			{
				num++;
			}
			else if (humans[i].Team == 2 && humans[i].Health > 0f)
			{
				num2++;
			}
		}
		if ((bool)team1Left && (bool)team2Left)
		{
			if (num != lastTeam1Left)
			{
				lastTeam1Left = num;
				animator.SetTrigger("Team1LeftFlick");
			}
			if (num2 != lastTeam2Left)
			{
				lastTeam2Left = num2;
				animator.SetTrigger("Team2LeftFlick");
			}
			team1Left.text = num.ToString();
			team2Left.text = num2.ToString();
		}
	}

	public override void UpdateGameUI()
	{
		if ((bool)gameManager && (bool)gameManager.LocalPlayer && (bool)gameManager.LocalPlayer)
		{
			bool isSpectating = gameManager.LocalPlayer.IsSpectating;
			animator.SetBool("Spectating", isSpectating);
			bool flag = true;
			if (gameManager.LocalPlayer.Health <= 0f)
			{
				flag = false;
			}
			if ((settings.activeInHierarchy | itemsMenu.activeInHierarchy | scoreboard.gameObject.activeInHierarchy) || (!flag && !isSpectating))
			{
				animator.SetBool("GameUI Active", value: false);
				animator.SetBool("Timer Maximized", value: false);
			}
			else
			{
				animator.SetBool("Timer Maximized", value: true);
				animator.SetBool("GameUI Active", value: true);
			}
		}
	}

	public void SetTimer(int time)
	{
		int num = Mathf.FloorToInt(time / 60);
		int num2 = time % 60;
		string text = ((num < 10) ? ("0" + num) : num.ToString());
		string text2 = ((num2 < 10) ? ("0" + num2) : num2.ToString());
		timerText.text = text + ":" + text2;
	}

	public void SetGameStartTimer(int time)
	{
		Object.Instantiate(timerClick, base.transform.position, base.transform.rotation);
		gameStartTimerText.text = time.ToString();
		if (time != 0)
		{
			animator.SetTrigger("Timer Tick");
		}
	}

	public void UpdateWinsDisplay(int team1Wins, int team1RoundsWon, int team2Wins, int team2RoundsWon)
	{
		string text = "team 1 wins: " + team1Wins;
		if (team1RoundsWon == 1)
		{
			text += ".";
		}
		string text2 = "team 2 wins: " + team2Wins;
		if (team2RoundsWon == 1)
		{
			text2 += ".";
		}
		winDisplay.UpdateWinDisplay(team1Wins, team1RoundsWon, team2Wins, team2RoundsWon);
	}

	public void UpdateSupplyPointsText(int value)
	{
		supplyPointsText.text = value.ToString();
	}

	public void ShowGameStartTimer()
	{
		gameStartTimer.SetActive(value: true);
	}

	public void HideGameStartTimer()
	{
		gameStartTimerText.text = "0";
		animator.SetTrigger("Timer Vanish");
		Invoke("DisableGameStartTimer", disableGameStartTimerDelay);
	}

	public void DisableGameStartTimer()
	{
		gameStartTimer.SetActive(value: false);
	}

	public void ToggleSpecialistKits()
	{
		specialistKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, loadoutHiddenPosition, 0f);
		standardKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, loadoutHiddenPosition, 0f);
		showSpecialistKits = !showSpecialistKits;
		itemsMenu.GetComponent<Animator>().SetBool("SpecialistSelect", showSpecialistKits);
		if (showSpecialistKits)
		{
			specialistKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
		}
		else
		{
			standardKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	public void ResetKitsScreen()
	{
		specialistKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, loadoutHiddenPosition, 0f);
		standardKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
	}

	public void UpdateLoadingPlayerScreen()
	{
		if (!base.AllPlayersLoaded)
		{
			waitingForPlayersScreen.SetActive(value: true);
			animator.SetBool("Show Loading Screen", value: true);
		}
		else
		{
			animator.SetBool("Show Loading Screen", value: false);
			Invoke("DisableMapLoadingScreen", loadingScreenHiddenDelay);
		}
	}
}
