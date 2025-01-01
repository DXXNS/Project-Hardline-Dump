using UnityEngine;

public class MeleeWeapon : PlayerItem
{
	[SerializeField]
	private GameObject useSoundEffect;

	[SerializeField]
	private GameObject attackObject;

	[SerializeField]
	private float swingDelay;

	[SerializeField]
	private float damage;

	[SerializeField]
	private float range;

	private bool usingItem;

	private bool readyToUse;

	public override void Start()
	{
		readyToUse = true;
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}

	public override bool PrimaryUseItem(bool input)
	{
		if (readyToUse && input && itemEnabled)
		{
			readyToUse = false;
			base.User.ReplicatePrimaryUseItem();
			useAnimationTrigger = true;
			usingItem = true;
			Invoke("SpawnAttack", swingDelay);
			Invoke("Cooldown", useCooldown);
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

	public virtual void SpawnAttack()
	{
		Transform headTransform = base.User.HeadTransform;
		GameObject obj = Object.Instantiate(attackObject, headTransform.transform.position, headTransform.transform.rotation);
		obj.GetComponent<MeleeAttack>().Item = this;
		obj.GetComponent<MeleeAttack>().SetAttackStats(damage, range);
		obj.GetComponent<MeleeAttack>().CheckDamage();
	}

	public void Cooldown()
	{
		readyToUse = true;
	}
}
