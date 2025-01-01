using UnityEngine;

public class PlayerManualFirearm : PlayerFirearm
{
	private bool lastInput;

	private bool currentlyRecycling;

	[SerializeField]
	private float casingDelay;

	[SerializeField]
	private GameObject manualSound;

	public bool CurrentlyRecycling
	{
		get
		{
			return currentlyRecycling;
		}
		set
		{
			currentlyRecycling = value;
		}
	}

	public override bool PrimaryUseItem(bool input)
	{
		if (input)
		{
			base.StopRepetitiveReload = true;
		}
		if (input && base.Chambered && !base.Sprinting && !base.WeaponFireObstruction && itemEnabled)
		{
			if (base.AimedDownSights)
			{
				Shoot(base.SpreadAmountADS);
			}
			else
			{
				Shoot(base.SpreadAmountHip);
			}
			base.Chambered = false;
			return true;
		}
		if (!input && !CurrentlyRecycling && !base.Reloading && !base.Sprinting && !base.Chambered && itemEnabled)
		{
			RecycleFirearm();
		}
		lastInput = input;
		return false;
	}

	public override void SecondaryUseItem(bool input)
	{
		if (!base.Sprinting && !base.WeaponADSObstruction)
		{
			base.AimedDownSights = input;
		}
		else
		{
			base.AimedDownSights = false;
		}
	}

	private void RecycleFirearm()
	{
		if (base.Ammo > 0)
		{
			CurrentlyRecycling = true;
			if ((bool)manualSound)
			{
				Object.Instantiate(manualSound, base.transform.position, base.transform.rotation).transform.SetParent(base.transform);
			}
			Invoke("ChamberWeapon", base.Cooldown);
			Invoke("ProduceCasing", casingDelay);
			base.UseAnimationTrigger = true;
		}
	}

	public override void CallReload()
	{
		if (!CurrentlyRecycling && !base.Reloading)
		{
			base.CallReload();
		}
	}

	public override void Shoot(float spreadAmount)
	{
		accumulatedHipSpread += hipSpreadIncreasePerShot;
		Vector3 activeBarrelPos = GetActiveBarrelPos();
		Quaternion activeBarrelRot = GetActiveBarrelRot();
		base.User.ReplicatePrimaryUseItem();
		for (int i = 0; i < base.NumberOfProjectiles; i++)
		{
			Projectile projectile = ((!activeBarrel.ReplacementBarrelPoint) ? Object.Instantiate(base.Projectile, base.BarrelPoint.position, base.BarrelPoint.transform.rotation) : Object.Instantiate(base.Projectile, activeBarrelPos, activeBarrelRot));
			projectile.SetProjectileStats(base.ProjectileSpeed, base.ProjectileDamage, base.ProjectileGravity, base.DeformationSize, base.Penetration, base.DeformationChance, base.DamageDropOff, base.PenetrationDropOff, useLimbMultipliers, suppressStrength);
			projectile.Item = this;
			projectile.transform.Rotate(new Vector3(Random.Range((0f - spreadAmount) / 2f, spreadAmount / 2f), Random.Range((0f - spreadAmount) / 2f, spreadAmount / 2f), 0f));
			if ((bool)GameObject.Find("Temp"))
			{
				projectile.transform.parent = GameObject.Find("Temp").transform;
			}
		}
		ApplyRecoil();
		if ((bool)base.UseAudio)
		{
			GameObject gameObject = ((!activeBarrel.ReplacementUseAudio) ? Object.Instantiate(base.UseAudio, activeBarrelPos, activeBarrelRot) : Object.Instantiate(activeBarrel.ReplacementUseAudio, activeBarrelPos, activeBarrelRot));
			if ((bool)GameObject.Find("Temp"))
			{
				gameObject.transform.parent = GameObject.Find("Temp").transform;
			}
		}
		if ((bool)base.UseEffect)
		{
			ProduceMuzzleFlash();
		}
		DepleteAmmo(1);
	}

	public override void Shoot()
	{
		Vector3 activeBarrelPos = GetActiveBarrelPos();
		Quaternion activeBarrelRot = GetActiveBarrelRot();
		base.User.ReplicatePrimaryUseItem();
		for (int i = 0; i < base.NumberOfProjectiles; i++)
		{
			Projectile projectile = ((!activeBarrel.ReplacementBarrelPoint) ? Object.Instantiate(base.Projectile, base.BarrelPoint.position, base.BarrelPoint.transform.rotation) : Object.Instantiate(base.Projectile, activeBarrel.ReplacementBarrelPoint.transform.position, activeBarrel.ReplacementBarrelPoint.transform.rotation));
			projectile.SetProjectileStats(base.ProjectileSpeed, base.ProjectileDamage, base.ProjectileGravity, base.DeformationSize, base.Penetration, base.DeformationChance, base.DamageDropOff, base.PenetrationDropOff, useLimbMultipliers, suppressStrength);
			projectile.Item = this;
			if ((bool)GameObject.Find("Temp"))
			{
				projectile.transform.parent = GameObject.Find("Temp").transform;
			}
		}
		ApplyRecoil();
		if ((bool)base.UseAudio)
		{
			GameObject gameObject = ((!activeBarrel.ReplacementUseAudio) ? Object.Instantiate(base.UseAudio, activeBarrelPos, activeBarrelRot) : Object.Instantiate(activeBarrel.ReplacementUseAudio, activeBarrelPos, activeBarrelRot));
			if ((bool)GameObject.Find("Temp"))
			{
				gameObject.transform.parent = GameObject.Find("Temp").transform;
			}
		}
		if ((bool)base.UseEffect)
		{
			ProduceMuzzleFlash();
		}
		DepleteAmmo(1);
	}

	public override void ChamberWeapon()
	{
		base.ChamberWeapon();
		CurrentlyRecycling = false;
	}
}
