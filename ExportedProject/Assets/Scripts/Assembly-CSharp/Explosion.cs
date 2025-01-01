using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
	[SerializeField]
	private float baseDamage;

	[SerializeField]
	private AnimationCurve damageMultiplierDropOff;

	[SerializeField]
	private float maxRange;

	[SerializeField]
	private float lifeTime;

	[SerializeField]
	private bool ignoreObstructions;

	private static int playerMask = 4096;

	private static int characterMask = 64;

	private static int grenadeMask = 8192;

	private Human myCauser;

	public Human MyCauser
	{
		get
		{
			return myCauser;
		}
		set
		{
			myCauser = value;
		}
	}

	private void Start()
	{
		Invoke("Remove", lifeTime);
		if (base.isServer)
		{
			DealExplosionDamage();
		}
	}

	private void DealExplosionDamage()
	{
		List<float> list = new List<float>();
		List<Human> list2 = new List<Human>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, maxRange);
		foreach (Collider collider in array)
		{
			if (ignoreObstructions && (bool)collider.GetComponent<Hitbox>())
			{
				if (collider.GetComponent<Human>().Health > 0f)
				{
					float num = damageMultiplierDropOff.Evaluate(Vector3.Distance(base.transform.position, collider.transform.position)) * baseDamage;
					if (list2.Contains(collider.GetComponent<Hitbox>().Player))
					{
						list[list2.IndexOf(collider.GetComponent<Hitbox>().Player)] += num;
						continue;
					}
					list.Add(num);
					list2.Add(collider.GetComponent<Hitbox>().Player);
				}
			}
			else
			{
				if (!collider.GetComponent<Hitbox>())
				{
					continue;
				}
				int layerMask = ~(playerMask | characterMask | grenadeMask);
				Vector3 normalized = (collider.transform.position - base.transform.position).normalized;
				if (!Physics.Raycast(base.transform.position, normalized, out var hitInfo, maxRange, layerMask))
				{
					continue;
				}
				Debug.DrawRay(base.transform.position, normalized * maxRange, Color.red, 20f);
				if ((bool)hitInfo.transform.GetComponent<Hitbox>())
				{
					float num2 = damageMultiplierDropOff.Evaluate(Vector3.Distance(base.transform.position, collider.transform.position)) * baseDamage;
					if (list2.Contains(collider.GetComponent<Hitbox>().Player))
					{
						list[list2.IndexOf(collider.GetComponent<Hitbox>().Player)] += num2;
						continue;
					}
					list.Add(num2);
					list2.Add(collider.GetComponent<Hitbox>().Player);
				}
			}
		}
		for (int j = 0; j <= list2.Count - 1; j++)
		{
			MonoBehaviour.print("deal damage explosion " + list[j] + " " + list2[j].Health);
			Object.FindObjectOfType<HardlineGameManager>().CallHitAnotherPlayer(MyCauser, list2[j], list2[j].transform.position, Vector3.zero, list[j]);
		}
	}

	public void Remove()
	{
		if (base.isServer)
		{
			NetworkServer.Destroy(base.gameObject);
		}
	}

	private void MirrorProcessed()
	{
	}
}
