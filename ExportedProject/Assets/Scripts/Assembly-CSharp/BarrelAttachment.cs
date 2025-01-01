using UnityEngine;

public class BarrelAttachment : WeaponAttachment
{
	[SerializeField]
	private GameObject replacementBarrelPoint;

	[SerializeField]
	private GameObject replacementMuzzleFlash;

	[SerializeField]
	private GameObject replacementUseAudio;

	[SerializeField]
	private float length;

	public float Length
	{
		get
		{
			return length;
		}
		set
		{
			length = value;
		}
	}

	public GameObject ReplacementMuzzleFlash
	{
		get
		{
			return replacementMuzzleFlash;
		}
		set
		{
			replacementMuzzleFlash = value;
		}
	}

	public GameObject ReplacementUseAudio
	{
		get
		{
			return replacementUseAudio;
		}
		set
		{
			replacementUseAudio = value;
		}
	}

	public GameObject ReplacementBarrelPoint
	{
		get
		{
			return replacementBarrelPoint;
		}
		set
		{
			replacementBarrelPoint = value;
		}
	}
}
