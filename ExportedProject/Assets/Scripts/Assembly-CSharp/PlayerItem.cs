using System;
using UnityEngine;

public abstract class PlayerItem : Item
{
	[SerializeField]
	protected float useCooldown;

	[SerializeField]
	protected float drawTime;

	protected bool itemEnabled;

	[SerializeField]
	protected bool useAnimationTrigger;

	[SerializeField]
	private int itemCost;

	[SerializeField]
	protected GameObject drawSound;

	[SerializeField]
	protected GameObject initSound;

	[SerializeField]
	protected string itemDescription;

	private static float fixedUpdateCallRate = 0.02f;

	private static float itemSwayTransition_kP = 0.3f;

	private static float itemShift_kP = 0.15f;

	private static float itemShiftAmount_x = 0.02f;

	private static float itemShiftAmount_y = 0.2f;

	private static float itemShiftCap = 0.2f;

	private Vector3 rotationVector;

	private Vector3 positionOffset;

	private Vector3 rotationOffset;

	[SerializeField]
	private Transform baseTransform;

	[SerializeField]
	private Transform forwardTransform;

	[Header("Animations and Avatars")]
	[SerializeField]
	private RuntimeAnimatorController animatorController;

	[SerializeField]
	private Avatar avatar;

	[Header("Item Stats")]
	[SerializeField]
	private float itemSwingAmount;

	[SerializeField]
	private float itemRecovery_kP;

	private float itemSwing_x;

	private float itemSwing_y;

	[Header("Item Camera Effects")]
	[SerializeField]
	private float cameraShakeMagnitude;

	[SerializeField]
	private float cameraShakeRoughness;

	[SerializeField]
	private float cameraShakeFadeIn;

	[SerializeField]
	private float cameraShakeFadeOut;

	[SerializeField]
	private float itemSwayMultiplier;

	private Vector3 itemSway;

	private Vector3 targItemShift;

	private Vector3 itemShift;

	[SerializeField]
	private float itemStability = 1f;

	private float swayTime;

	private float itemSwaySpeed;

	private float itemSwayMagnitude;

	private float itemSwayMagnitudeTarget;

	private bool sprinting;

	[SerializeField]
	private Human user;

	public Vector3 RotationOffset
	{
		get
		{
			return rotationOffset;
		}
		set
		{
			rotationOffset = value;
		}
	}

	public Vector3 RotationVector
	{
		get
		{
			return rotationVector;
		}
		set
		{
			rotationVector = value;
		}
	}

	public Transform BaseTransform
	{
		get
		{
			return baseTransform;
		}
		set
		{
			baseTransform = value;
		}
	}

	public float ItemSwing_x
	{
		get
		{
			return itemSwing_x;
		}
		set
		{
			itemSwing_x = value;
		}
	}

	public float ItemSwing_y
	{
		get
		{
			return itemSwing_y;
		}
		set
		{
			itemSwing_y = value;
		}
	}

	public float ItemSwingAmount
	{
		get
		{
			return itemSwingAmount;
		}
		set
		{
			itemSwingAmount = value;
		}
	}

	public float ItemRecovery_kP
	{
		get
		{
			return itemRecovery_kP;
		}
		set
		{
			itemRecovery_kP = value;
		}
	}

	public Vector3 PositionOffset
	{
		get
		{
			return positionOffset;
		}
		set
		{
			positionOffset = value;
		}
	}

	public Transform ForwardTransform
	{
		get
		{
			return forwardTransform;
		}
		set
		{
			forwardTransform = value;
		}
	}

	public RuntimeAnimatorController AnimatorController
	{
		get
		{
			return animatorController;
		}
		set
		{
			animatorController = value;
		}
	}

	public Avatar Avatar
	{
		get
		{
			return avatar;
		}
		set
		{
			avatar = value;
		}
	}

	public Vector3 ItemSway
	{
		get
		{
			return itemSway;
		}
		set
		{
			itemSway = value;
		}
	}

	public Vector3 TargItemShift
	{
		get
		{
			return targItemShift;
		}
		set
		{
			targItemShift = value;
		}
	}

	public Vector3 ItemShift
	{
		get
		{
			return itemShift;
		}
		set
		{
			itemShift = value;
		}
	}

	public static float ItemSwayTransition_kP
	{
		get
		{
			return itemSwayTransition_kP;
		}
		set
		{
			itemSwayTransition_kP = value;
		}
	}

	public static float ItemShift_kP
	{
		get
		{
			return itemShift_kP;
		}
		set
		{
			itemShift_kP = value;
		}
	}

	public static float ItemShiftAmount_x
	{
		get
		{
			return itemShiftAmount_x;
		}
		set
		{
			itemShiftAmount_x = value;
		}
	}

	public static float ItemShiftAmount_y
	{
		get
		{
			return itemShiftAmount_y;
		}
		set
		{
			itemShiftAmount_y = value;
		}
	}

	public static float ItemShiftCap
	{
		get
		{
			return itemShiftCap;
		}
		set
		{
			itemShiftCap = value;
		}
	}

	public bool Sprinting
	{
		get
		{
			return sprinting;
		}
		set
		{
			sprinting = value;
		}
	}

	public Human User
	{
		get
		{
			return user;
		}
		set
		{
			user = value;
		}
	}

	public bool UseAnimationTrigger
	{
		get
		{
			return useAnimationTrigger;
		}
		set
		{
			useAnimationTrigger = value;
		}
	}

	public float CameraShakeMagnitude
	{
		get
		{
			return cameraShakeMagnitude;
		}
		set
		{
			cameraShakeMagnitude = value;
		}
	}

	public float CameraShakeRoughness
	{
		get
		{
			return cameraShakeRoughness;
		}
		set
		{
			cameraShakeRoughness = value;
		}
	}

	public float CameraShakeFadeIn
	{
		get
		{
			return cameraShakeFadeIn;
		}
		set
		{
			cameraShakeFadeIn = value;
		}
	}

	public float CameraShakeFadeOut
	{
		get
		{
			return cameraShakeFadeOut;
		}
		set
		{
			cameraShakeFadeOut = value;
		}
	}

	public float ItemSwayMultiplier
	{
		get
		{
			return itemSwayMultiplier;
		}
		set
		{
			itemSwayMultiplier = value;
		}
	}

	public int ItemCost
	{
		get
		{
			return itemCost;
		}
		set
		{
			itemCost = value;
		}
	}

	public string ItemDescription
	{
		get
		{
			return itemDescription;
		}
		set
		{
			itemDescription = value;
		}
	}

	public virtual void Start()
	{
		User = GetComponentInParent<Human>();
		itemEnabled = false;
		Invoke("EnableItem", drawTime);
	}

	public void DrawSound(bool init)
	{
		if (init)
		{
			if ((bool)initSound && (bool)User)
			{
				UnityEngine.Object.Instantiate(initSound, User.transform.position, User.transform.rotation);
			}
		}
		else if ((bool)drawSound && (bool)User)
		{
			UnityEngine.Object.Instantiate(drawSound, User.transform.position, User.transform.rotation);
		}
	}

	public void EnableItem()
	{
		itemEnabled = true;
	}

	public virtual void Update()
	{
		UpdateItemSway();
		UpdateTransforms();
	}

	public virtual void UpdateTransforms()
	{
		rotationOffset = new Vector3(ItemSwing_y + ItemSway.x, 0f, ItemSwing_x + ItemSway.y);
		ItemSwing_x += (0f - ItemSwing_x) * ItemRecovery_kP;
		ItemSwing_y += (0f - ItemSwing_y) * ItemRecovery_kP;
		positionOffset = new Vector3(ItemShift.x, ItemShift.y, 0f);
		base.transform.localPosition = ConvertVectorDirections(PositionOffset);
		base.transform.localEulerAngles = ConvertVectorDirections(RotationOffset);
	}

	public Vector3 ConvertVectorDirections(Vector3 baseVector)
	{
		return Quaternion.Euler(RotationVector) * baseVector;
	}

	public Vector3 GetZeroedBaseTransformPosition()
	{
		if ((bool)baseTransform)
		{
			Vector3 position = base.transform.position;
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			Vector3 result = baseTransform.TransformPoint(0f, 0f, 0f);
			base.transform.position = position;
			return result;
		}
		Vector3 position2 = base.transform.position;
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		Vector3 result2 = base.transform.TransformPoint(0f, 0f, 0f);
		base.transform.position = position2;
		return result2;
	}

	public virtual void HitAnotherTarget(bool killShot, Human target)
	{
		if (User.hasAuthority && User is Player && !User.PlayersKilled.Contains(target))
		{
			(User as Player).HitMarkerSound(killShot);
		}
		if (killShot)
		{
			if (target.Team != User.Team)
			{
				if (!User.PlayersKilled.Contains(target))
				{
					if ((bool)UnityEngine.Object.FindObjectOfType<KillText>())
					{
						UnityEngine.Object.FindObjectOfType<KillText>().CreateNewKillText(target.HumanName);
					}
					User.AddKill();
					User.AddScore(100);
					User.PlayersKilled.Add(target);
					(User as Player).KillHitMarker.GetComponent<Hitmarker>().HitAnimation();
				}
				MonoBehaviour.print("hit another target kill");
			}
			else
			{
				User.AddScore(-100);
			}
		}
		else if (target.Team != User.Team && User is Player)
		{
			(User as Player).BaseHitMarker.GetComponent<Hitmarker>().HitAnimation();
		}
	}

	public void AddItemSwing(float x, float y)
	{
		ItemSwing_x += x * ItemSwingAmount;
		ItemSwing_y += y * ItemSwingAmount;
	}

	public void UpdateItemSway()
	{
		swayTime += itemSwaySpeed * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant;
		if (swayTime > MathF.PI * 2f)
		{
			swayTime = MathF.PI / 180f * (swayTime - 360f);
		}
		itemSwayMagnitude += (itemSwayMagnitudeTarget - itemSwayMagnitude) * ItemSwayTransition_kP;
		ItemSway = new Vector3(itemSwayMagnitude * ItemSwayMultiplier * Mathf.Sin(swayTime * 2f), itemSwayMagnitude * ItemSwayMultiplier * Mathf.Sin(swayTime), 0f);
	}

	public virtual void UpdateItemShift(float x, float y)
	{
		TargItemShift = new Vector3(Mathf.Clamp(x * ItemShiftAmount_x, 0f - ItemShiftCap, ItemShiftCap), Mathf.Clamp(y * ItemShiftAmount_y, 0f - ItemShiftCap, ItemShiftCap), 0f);
		ItemShift += new Vector3((TargItemShift.x - ItemShift.x) * ItemShift_kP, (TargItemShift.y - ItemShift.y) * ItemShift_kP, 0f);
	}

	public void SetItemSwayValues(float speed, float magnitude)
	{
		itemSwaySpeed = speed;
		itemSwayMagnitudeTarget = magnitude * itemStability;
	}

	public abstract bool PrimaryUseItem(bool input);

	public abstract void SecondaryUseItem(bool input);
}
