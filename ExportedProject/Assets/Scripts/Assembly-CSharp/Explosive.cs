using Mirror;
using UnityEngine;

public class Explosive : NetworkBehaviour
{
	[SerializeField]
	private GameObject explosion;

	[SerializeField]
	private float fuzeTime;

	[SerializeField]
	private bool removeOnExplode;

	[SerializeField]
	private GameObject groundHitSound;

	private Human myCauser;

	private bool hasHitSurface;

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
		if (base.isServer)
		{
			Invoke("Explode", fuzeTime);
		}
		if (!base.isServer)
		{
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	private void Update()
	{
	}

	public void Explode()
	{
		Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject(explosion.name, base.transform.position, Vector3.forward, MyCauser);
		if (removeOnExplode)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void HitSurface()
	{
		if (!hasHitSurface)
		{
			if ((bool)groundHitSound)
			{
				Object.Instantiate(groundHitSound, base.transform.position, base.transform.rotation);
			}
			hasHitSurface = true;
		}
	}

	private void MirrorProcessed()
	{
	}
}
