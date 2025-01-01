using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
	[SerializeField]
	private int cost;

	[SerializeField]
	private string attachmentName;

	[SerializeField]
	protected Texture itemIcon;

	[SerializeField]
	private Vector3 aimRecoilMultiplier = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private Vector3 gunRecoilMultiplier = new Vector3(1f, 1f, 1f);

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

	public Vector3 AimRecoilMultiplier
	{
		get
		{
			return aimRecoilMultiplier;
		}
		set
		{
			aimRecoilMultiplier = value;
		}
	}

	public Vector3 GunRecoilMultiplier
	{
		get
		{
			return gunRecoilMultiplier;
		}
		set
		{
			gunRecoilMultiplier = value;
		}
	}

	public string AttachmentName
	{
		get
		{
			return attachmentName;
		}
		set
		{
			attachmentName = value;
		}
	}

	public int Cost
	{
		get
		{
			return cost;
		}
		set
		{
			cost = value;
		}
	}
}
