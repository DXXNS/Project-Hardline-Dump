using UnityEngine;

public class Item : MonoBehaviour
{
	[SerializeField]
	private string itemName;

	[SerializeField]
	private string displayName;

	[SerializeField]
	private Texture itemIcon;

	public string ItemName
	{
		get
		{
			return itemName;
		}
		set
		{
			itemName = value;
		}
	}

	public Texture ItemIcon
	{
		get
		{
			return itemIcon;
		}
		set
		{
			itemIcon = value;
		}
	}

	public string GetDisplayName()
	{
		if (displayName != "")
		{
			return displayName;
		}
		return itemName;
	}
}
