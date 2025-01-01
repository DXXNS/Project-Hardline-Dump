using UnityEngine;

public class Consumable : PlayerItem
{
	[SerializeField]
	protected float useTime;

	[SerializeField]
	protected float useAfterDelay;

	[SerializeField]
	protected float healthRecovery;

	[SerializeField]
	protected float ammoRecovery;

	[SerializeField]
	protected bool removeAfterUse;

	[SerializeField]
	private GameObject useSoundEffect;

	private bool usingItem;

	public bool UsingItem
	{
		get
		{
			return usingItem;
		}
		set
		{
			usingItem = value;
		}
	}

	public override void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}

	public override bool PrimaryUseItem(bool input)
	{
		if (!UsingItem && input && itemEnabled)
		{
			base.User.ReplicatePrimaryUseItem();
			useAnimationTrigger = true;
			UsingItem = true;
			Invoke("UseItem", useTime);
			if (removeAfterUse)
			{
				Invoke("FinishUse", useTime + useAfterDelay);
			}
			if ((bool)useSoundEffect)
			{
				Object.Instantiate(useSoundEffect, base.transform.position, base.transform.rotation).transform.parent = base.transform;
			}
			return true;
		}
		return false;
	}

	public override void SecondaryUseItem(bool input)
	{
	}

	public virtual void UseItem()
	{
		base.User.ReplicateDealDamage(0f - healthRecovery, base.User.HumanName, base.transform.position, Vector3.zero);
	}

	public void FinishUse()
	{
		if (removeAfterUse)
		{
			UsingItem = false;
			Unequip();
		}
	}

	public void Unequip()
	{
		base.User.LoadItem("Unarmed", localPlayer: true);
	}
}
