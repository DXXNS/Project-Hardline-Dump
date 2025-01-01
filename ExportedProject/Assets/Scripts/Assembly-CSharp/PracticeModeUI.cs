using UnityEngine;
using UnityEngine.UI;

public class PracticeModeUI : UserInterface
{
	private static float loadoutHiddenPosition = 500f;

	[SerializeField]
	private GameObject standardKits;

	[SerializeField]
	private GameObject specialistKits;

	[SerializeField]
	private LoadoutSelectable[] loadoutSelectables;

	[SerializeField]
	private LoadoutSelectable[] specialistLoadoutSelectables;

	[SerializeField]
	private Text team1Left;

	[SerializeField]
	private Text team2Left;

	[SerializeField]
	private Text timerText;

	[SerializeField]
	private Text gameWinStateText;

	[SerializeField]
	private Text gameEndElapsedText;

	private bool gameOverScreen;

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

	public bool GameOverScreen
	{
		get
		{
			return gameOverScreen;
		}
		set
		{
			gameOverScreen = value;
		}
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateTeamLeft();
		if (humans.Count <= 0)
		{
			GetPlayers();
		}
	}

	public void ResetKitsScreen()
	{
		specialistKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
		standardKits.GetComponent<RectTransform>().localPosition = new Vector3(0f, loadoutHiddenPosition, 0f);
	}

	public void ToggleGameOverScreen(bool state, bool win)
	{
		GameOverScreen = state;
		animator.SetBool("GameOver Screen", state);
		if (state && IsInvoking("IncrementTimer"))
		{
			CancelInvoke("IncrementTimer");
		}
		if (win)
		{
			gameWinStateText.text = "Scenario Completed";
		}
		else
		{
			gameWinStateText.text = "Scenario Failed";
		}
		gameEndElapsedText.text = timerText.text;
		if ((bool)Object.FindObjectOfType<Player>() && state)
		{
			Object.FindObjectOfType<Player>().MenuOpened = true;
		}
	}

	public void DisableGameOverScreen()
	{
		animator.SetBool("GameOver Screen", value: false);
		GameOverScreen = false;
	}

	public void RestartGame()
	{
		Object.FindObjectOfType<PracticeModeGameManager>().RestartGame();
		Object.FindObjectOfType<PracticeModeGameManager>().AllPlayersLoaded();
	}

	private void UpdateTeamLeft()
	{
		int num = 0;
		int num2 = 0;
		foreach (Human human in humans)
		{
			if (human.Team == 1 && human.Health > 0f)
			{
				num++;
			}
			else if (human.Team == 2 && human.Health > 0f)
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

	public void SetTimer(int time)
	{
		int num = Mathf.FloorToInt(time / 60);
		int num2 = time % 60;
		string text = ((num < 10) ? ("0" + num) : num.ToString());
		string text2 = ((num2 < 10) ? ("0" + num2) : num2.ToString());
		timerText.text = text + ":" + text2;
	}
}
