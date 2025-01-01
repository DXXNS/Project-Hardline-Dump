using UnityEngine;
using UnityEngine.UI;

public class ScoreboardEntry : MonoBehaviour
{
	[SerializeField]
	private Text playerNameText;

	[SerializeField]
	private Text playerKillsText;

	[SerializeField]
	private Text playerAssistsText;

	[SerializeField]
	private Text playerDeathsText;

	[SerializeField]
	private Text playerScoreText;

	private RawImage box;

	[SerializeField]
	private Color baseColor;

	[SerializeField]
	private Color playerColor;

	public Text PlayerNameText
	{
		get
		{
			return playerNameText;
		}
		set
		{
			playerNameText = value;
		}
	}

	public Text PlayerKillsText
	{
		get
		{
			return playerKillsText;
		}
		set
		{
			playerKillsText = value;
		}
	}

	public Text PlayerAssistsText
	{
		get
		{
			return playerAssistsText;
		}
		set
		{
			playerAssistsText = value;
		}
	}

	public Text PlayerDeathsText
	{
		get
		{
			return playerDeathsText;
		}
		set
		{
			playerDeathsText = value;
		}
	}

	public Text PlayerScoreText
	{
		get
		{
			return playerScoreText;
		}
		set
		{
			playerScoreText = value;
		}
	}

	private void Awake()
	{
		box = GetComponent<RawImage>();
	}

	public void UpdateEntry(string name, string score, string kills, string assists, string deaths, bool isPlayer)
	{
		PlayerNameText.text = name;
		PlayerScoreText.text = score;
		PlayerKillsText.text = kills;
		PlayerAssistsText.text = assists;
		PlayerDeathsText.text = deaths;
		if (isPlayer)
		{
			box.color = playerColor;
		}
		else
		{
			box.color = baseColor;
		}
	}
}
