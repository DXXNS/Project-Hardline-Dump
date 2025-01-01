using UnityEngine;

[ExecuteInEditMode]
public class TeamSpawn : MonoBehaviour
{
	[SerializeField]
	private float spawnRadius;

	public float SpawnRadius
	{
		get
		{
			return spawnRadius;
		}
		set
		{
			spawnRadius = value;
		}
	}

	private void Update()
	{
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, SpawnRadius);
	}
}
