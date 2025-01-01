using UnityEngine;

public class Casing : MonoBehaviour
{
	public static float maxRenderDistance = 10f;

	[SerializeField]
	private float minForce;

	[SerializeField]
	private float maxForce;

	[SerializeField]
	private float lifetime;

	[SerializeField]
	private float directionRandomization;

	private void Start()
	{
		base.transform.Rotate(Random.Range(0f - directionRandomization, directionRandomization), Random.Range(0f - directionRandomization, directionRandomization), 0f);
		GetComponent<Rigidbody>().AddForce(Random.Range(minForce, maxForce) * base.transform.forward);
		Object.Destroy(base.gameObject, lifetime);
	}
}
