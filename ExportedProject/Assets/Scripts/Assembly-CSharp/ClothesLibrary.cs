using UnityEngine;

public class ClothesLibrary : MonoBehaviour
{
	[SerializeField]
	protected GameObject shirtParent;

	[SerializeField]
	protected GameObject pantsParent;

	[SerializeField]
	protected GameObject vestsParent;

	[SerializeField]
	protected GameObject headwearParent;

	[SerializeField]
	protected GameObject hoodsParent;

	public GameObject GetHeadwear(string helmet)
	{
		GameObject result = null;
		if ((bool)GetChildWithName(headwearParent, helmet))
		{
			headwearParent.transform.Find(helmet).gameObject.SetActive(value: true);
			result = GetChildWithName(headwearParent, helmet);
		}
		return result;
	}

	public GameObject GetVests(string vest)
	{
		GameObject result = null;
		if ((bool)GetChildWithName(vestsParent, vest))
		{
			vestsParent.transform.Find(vest).gameObject.SetActive(value: true);
			result = GetChildWithName(vestsParent, vest);
		}
		return result;
	}

	public GameObject GetShirt(string shirt)
	{
		GameObject result = null;
		if ((bool)GetChildWithName(shirtParent, shirt))
		{
			shirtParent.transform.Find(shirt).gameObject.SetActive(value: true);
			result = GetChildWithName(shirtParent, shirt);
		}
		return result;
	}

	public GameObject GetPants(string pants)
	{
		GameObject result = null;
		if ((bool)GetChildWithName(pantsParent, pants))
		{
			pantsParent.transform.Find(pants).gameObject.SetActive(value: true);
			result = GetChildWithName(pantsParent, pants);
		}
		return result;
	}

	public GameObject GetHoods(string hoods)
	{
		GameObject result = null;
		if ((bool)GetChildWithName(hoodsParent, hoods))
		{
			hoodsParent.transform.Find(hoods).gameObject.SetActive(value: true);
			result = GetChildWithName(hoodsParent, hoods);
		}
		return result;
	}

	private GameObject GetChildWithName(GameObject obj, string name)
	{
		Transform transform = obj.transform.Find(name);
		if (transform != null)
		{
			return transform.gameObject;
		}
		return null;
	}
}
