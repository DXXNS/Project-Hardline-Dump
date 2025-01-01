using UnityEngine;

public class ClothingWithExtras : MonoBehaviour
{
	[SerializeField]
	private GameObject[] extraObjects;

	private void Start()
	{
		GameObject[] array = extraObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}

	public void ShowExtras()
	{
		GameObject[] array = extraObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}

	public void HideExtras()
	{
		GameObject[] array = extraObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
	}
}
