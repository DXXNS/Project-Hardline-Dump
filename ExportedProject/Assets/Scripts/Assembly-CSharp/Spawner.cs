using Mirror;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
	[SerializeField]
	private GameObject spawnedObject;

	[SerializeField]
	private float spawnDelay;

	[SerializeField]
	private bool initSpawn;

	private bool spawnFlag;

	private GameObject currentObject;

	[SerializeField]
	private bool respawn;

	public override void OnStartServer()
	{
		if (base.isServer)
		{
			if (initSpawn)
			{
				SpawnDesignatedObject();
			}
			else if (base.transform.childCount > 0)
			{
				currentObject = base.transform.GetChild(0).gameObject;
			}
		}
	}

	private void Update()
	{
		if (currentObject == null && !spawnFlag && respawn)
		{
			Invoke("SpawnDesignatedObject", spawnDelay);
			spawnFlag = true;
		}
	}

	public void SpawnDesignatedObject()
	{
		if (base.isServer)
		{
			currentObject = Object.Instantiate(spawnedObject, base.transform.position, base.transform.rotation);
			NetworkServer.Spawn(currentObject);
			currentObject.transform.position = base.transform.position;
			spawnFlag = false;
		}
	}

	private void MirrorProcessed()
	{
	}
}
