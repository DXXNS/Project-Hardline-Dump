using UnityEngine;

public class EnemyPatrolLocations : MonoBehaviour
{
	public Transform GetRandomPatrolLocation()
	{
		return base.transform.GetChild(Random.Range(0, base.transform.childCount - 1)).transform;
	}
}
