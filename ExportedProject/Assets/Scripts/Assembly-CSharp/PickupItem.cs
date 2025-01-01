using UnityEngine;

public class PickupItem : Item
{
	[SerializeField]
	private int sightNumber;

	[SerializeField]
	private int currentAmmo;

	[SerializeField]
	private int sightAttachment;

	[SerializeField]
	private int barrelAttachment;

	[SerializeField]
	private int gripAttachment;

	[SerializeField]
	private GameObject sightParent;

	[SerializeField]
	private GameObject barrelParent;

	[SerializeField]
	private GameObject gripParent;

	public int SightAttachment
	{
		get
		{
			return sightAttachment;
		}
		set
		{
			sightAttachment = value;
		}
	}

	public int BarrelAttachment
	{
		get
		{
			return barrelAttachment;
		}
		set
		{
			barrelAttachment = value;
		}
	}

	public int GripAttachment
	{
		get
		{
			return gripAttachment;
		}
		set
		{
			gripAttachment = value;
		}
	}

	private void Start()
	{
		if ((bool)sightParent && sightParent.transform.childCount > sightAttachment)
		{
			sightParent.transform.GetChild(SightAttachment).gameObject.SetActive(value: true);
		}
		if ((bool)barrelParent && barrelParent.transform.childCount > BarrelAttachment)
		{
			barrelParent.transform.GetChild(BarrelAttachment).gameObject.SetActive(value: true);
		}
		if ((bool)gripParent && gripParent.transform.childCount > gripAttachment)
		{
			gripParent.transform.GetChild(GripAttachment).gameObject.SetActive(value: true);
		}
	}

	public void ResetAttachments()
	{
		if ((bool)sightParent)
		{
			foreach (Transform componentInChild in sightParent.transform.GetComponentInChildren<Transform>())
			{
				componentInChild.gameObject.SetActive(value: false);
			}
			if (sightParent.transform.childCount > sightAttachment)
			{
				sightParent.transform.GetChild(SightAttachment).gameObject.SetActive(value: true);
			}
		}
		if ((bool)barrelParent)
		{
			foreach (Transform componentInChild2 in barrelParent.transform.GetComponentInChildren<Transform>())
			{
				componentInChild2.gameObject.SetActive(value: false);
			}
			if (barrelParent.transform.childCount > BarrelAttachment)
			{
				barrelParent.transform.GetChild(BarrelAttachment).gameObject.SetActive(value: true);
			}
		}
		if (!gripParent)
		{
			return;
		}
		foreach (Transform componentInChild3 in gripParent.transform.GetComponentInChildren<Transform>())
		{
			componentInChild3.gameObject.SetActive(value: false);
		}
		if (gripParent.transform.childCount > gripAttachment)
		{
			gripParent.transform.GetChild(GripAttachment).gameObject.SetActive(value: true);
		}
	}
}
