using UnityEngine;

public class WinDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject[] team1WinCircles;

	[SerializeField]
	private GameObject[] team2WinCircles;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void UpdateWinDisplay(int team1Wins, int team1SubWins, int team2Wins, int team2SubWins)
	{
		GameObject[] array = team1WinCircles;
		foreach (GameObject obj in array)
		{
			obj.SetActive(value: false);
			obj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		}
		array = team2WinCircles;
		foreach (GameObject obj2 in array)
		{
			obj2.SetActive(value: false);
			obj2.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		}
		int j = 0;
		int k = 0;
		for (; j < team1Wins + team1SubWins; j++)
		{
			if (team1WinCircles.Length > j)
			{
				team1WinCircles[j].SetActive(value: true);
			}
			if (team1SubWins == 1 && j == team1Wins)
			{
				team1WinCircles[j].GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 0.4f);
			}
		}
		for (; k < team2Wins + team2SubWins; k++)
		{
			if (team2WinCircles.Length > k)
			{
				team2WinCircles[k].SetActive(value: true);
			}
			if (team2SubWins == 1 && k == team2Wins)
			{
				team2WinCircles[k].GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 0.4f);
			}
		}
	}
}
