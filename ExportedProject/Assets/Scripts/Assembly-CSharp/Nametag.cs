using UnityEngine;
using UnityEngine.UI;

public class Nametag : MonoBehaviour
{
	[SerializeField]
	private Text nameTag;

	[SerializeField]
	private GameObject namePlate;

	public Text NameTag
	{
		get
		{
			return nameTag;
		}
		set
		{
			nameTag = value;
		}
	}

	public void SetNameTag(string name)
	{
		NameTag.text = name;
	}

	public void SetNameTagVisibility(bool visible)
	{
		NameTag.enabled = visible;
		namePlate.SetActive(visible);
	}
}
