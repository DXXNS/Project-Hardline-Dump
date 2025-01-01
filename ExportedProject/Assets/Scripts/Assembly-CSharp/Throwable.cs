using UnityEngine;

public class Throwable : Consumable
{
	[SerializeField]
	private GameObject spawnItem;

	[SerializeField]
	private GameObject spawnPoint;

	[SerializeField]
	private float throwStrength;

	public override void UseItem()
	{
		Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObjectForward(spawnItem.name, spawnPoint.transform.position, spawnPoint.transform.eulerAngles, throwStrength, base.User);
	}
}
