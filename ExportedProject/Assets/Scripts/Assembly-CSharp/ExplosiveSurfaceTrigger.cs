using UnityEngine;

public class ExplosiveSurfaceTrigger : MonoBehaviour
{
	[SerializeField]
	private Explosive explosive;

	public void OnTriggerEnter(Collider other)
	{
		explosive.HitSurface();
	}
}
