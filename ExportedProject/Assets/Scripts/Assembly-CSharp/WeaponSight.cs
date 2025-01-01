using UnityEngine;

public class WeaponSight : WeaponAttachment
{
	[SerializeField]
	private float useFOV;

	[SerializeField]
	private Vector3 positionalOffset;

	[SerializeField]
	private float sensitivityMultiplier;

	[SerializeField]
	private GameObject localObjects;

	[SerializeField]
	private float focusWeight;

	public float UseFOV
	{
		get
		{
			return useFOV;
		}
		set
		{
			useFOV = value;
		}
	}

	public Vector3 PositionalOffset
	{
		get
		{
			return positionalOffset;
		}
		set
		{
			positionalOffset = value;
		}
	}

	public float SensitivityMultiplier
	{
		get
		{
			return sensitivityMultiplier;
		}
		set
		{
			sensitivityMultiplier = value;
		}
	}

	public float FocusWeight
	{
		get
		{
			return focusWeight;
		}
		set
		{
			focusWeight = value;
		}
	}

	private void Awake()
	{
		DisableCamera();
	}

	public void EnableCamera()
	{
		if ((bool)localObjects)
		{
			localObjects.SetActive(value: true);
		}
	}

	public void DisableCamera()
	{
		if ((bool)localObjects)
		{
			localObjects.SetActive(value: false);
		}
	}
}
