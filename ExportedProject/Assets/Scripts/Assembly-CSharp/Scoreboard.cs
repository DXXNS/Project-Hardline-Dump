using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
	[SerializeField]
	private ScoreboardEntry[] team1ScoreTabs;

	[SerializeField]
	private ScoreboardEntry[] team2ScoreTabs;

	private List<Player> playerList = new List<Player>();

	private List<Player> playerTeam1List = new List<Player>();

	private List<Player> playerTeam2List = new List<Player>();

	private string localPlayerName;

	public string LocalPlayerName
	{
		get
		{
			return localPlayerName;
		}
		set
		{
			localPlayerName = value;
		}
	}

	private void Start()
	{
		EstablishPlayers();
	}

	private void Update()
	{
	}

	public void EstablishPlayers()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			playerList.Add(player);
			if (player.Team == 1)
			{
				playerTeam1List.Add(player);
			}
			else if (player.Team == 2)
			{
				playerTeam2List.Add(player);
			}
		}
	}

	public void UpdateScoreboard()
	{
		SortPlayersByScore();
		for (int i = 0; i <= team1ScoreTabs.Length - 1; i++)
		{
			if (i < playerTeam1List.Count)
			{
				Player player = playerTeam1List[i];
				team1ScoreTabs[i].UpdateEntry(player.HumanName, player.Score.ToString(), player.Kills.ToString(), player.Assists.ToString(), player.Deaths.ToString(), player.HumanName == localPlayerName);
			}
			else
			{
				team1ScoreTabs[i].UpdateEntry("___", "___", "___", "___", "___", isPlayer: false);
			}
		}
		for (int j = 0; j <= team2ScoreTabs.Length - 1; j++)
		{
			if (j < playerTeam2List.Count)
			{
				Player player2 = playerTeam2List[j];
				team2ScoreTabs[j].UpdateEntry(player2.HumanName, player2.Score.ToString(), player2.Kills.ToString(), player2.Assists.ToString(), player2.Deaths.ToString(), player2.HumanName == localPlayerName);
			}
			else
			{
				team2ScoreTabs[j].UpdateEntry("___", "___", "___", "___", "___", isPlayer: false);
			}
		}
	}

	private void SortPlayersByScore()
	{
		playerTeam1List.Sort((Player p2, Player p1) => p1.Score.CompareTo(p2.Score));
		playerTeam2List.Sort((Player p2, Player p1) => p1.Score.CompareTo(p2.Score));
		playerList.Sort((Player p2, Player p1) => p1.Score.CompareTo(p2.Score));
	}
}
