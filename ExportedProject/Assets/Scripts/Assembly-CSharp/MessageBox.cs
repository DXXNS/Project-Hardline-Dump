using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text messageText;

	[SerializeField]
	private int team;

	[SerializeField]
	private int channel;

	[SerializeField]
	private Color team1Color;

	[SerializeField]
	private Color team2Color;

	public Text NameText
	{
		get
		{
			return nameText;
		}
		set
		{
			nameText = value;
		}
	}

	public Text MessageText
	{
		get
		{
			return messageText;
		}
		set
		{
			messageText = value;
		}
	}

	public void SetMessage(string player, int team, int channel, string message)
	{
		nameText.text = player;
		this.team = team;
		messageText.text = message;
		this.channel = channel;
		switch (team)
		{
		case 1:
			nameText.color = team1Color;
			break;
		case 2:
			nameText.color = team2Color;
			break;
		}
	}
}
