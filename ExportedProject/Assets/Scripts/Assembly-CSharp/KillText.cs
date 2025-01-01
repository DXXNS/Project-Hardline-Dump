using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillText : MonoBehaviour
{
	private static float baseYPosition = -80f;

	private static float yDisplacement = -30f;

	private static float textDuration = 3f;

	[SerializeField]
	private GameObject killTextSample;

	[SerializeField]
	private GameObject assistTextSample;

	private List<GameObject> killTexts = new List<GameObject>();

	public void Update()
	{
		if (killTexts.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < killTexts.Count; i++)
		{
			if ((bool)killTexts[i])
			{
				killTexts[i].transform.localPosition = new Vector3(0f, baseYPosition + yDisplacement * (float)(killTexts.Count - i), 0f);
				killTexts[i].SetActive(value: true);
			}
			else
			{
				killTexts.RemoveAt(i);
			}
		}
	}

	public void CreateNewKillText(string playerName)
	{
		MonoBehaviour.print("kill text");
		GameObject gameObject = Object.Instantiate(killTextSample, base.transform.position, base.transform.rotation);
		gameObject.transform.GetChild(1).GetComponent<Text>().text = playerName;
		gameObject.transform.SetParent(base.transform);
		killTexts.Add(gameObject);
		Object.Destroy(gameObject, textDuration);
	}

	public void CreateNewAssistText(string playerName)
	{
		MonoBehaviour.print("assist text");
		GameObject gameObject = Object.Instantiate(assistTextSample, base.transform.position, base.transform.rotation);
		gameObject.transform.GetChild(1).GetComponent<Text>().text = playerName;
		gameObject.transform.parent = base.transform;
		killTexts.Add(gameObject);
		Object.Destroy(gameObject, textDuration);
	}

	public void RemoveText(GameObject text)
	{
	}
}
