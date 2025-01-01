using UnityEngine;

public class FirearmEntry : MonoBehaviour
{
	private string itemName;

	private int sight;

	private int grip;

	private int barrel;

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

	public int Sight
	{
		get
		{
			return sight;
		}
		set
		{
			sight = value;
		}
	}

	public int Grip
	{
		get
		{
			return grip;
		}
		set
		{
			grip = value;
		}
	}

	public int Barrel
	{
		get
		{
			return barrel;
		}
		set
		{
			barrel = value;
		}
	}

	public FirearmEntry(string name)
	{
		itemName = name;
	}

	public FirearmEntry(string name, int sight, int barrel, int grip)
	{
		base.name = name;
		this.sight = sight;
		this.barrel = barrel;
		this.grip = grip;
	}
}
