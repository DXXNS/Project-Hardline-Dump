using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ResonanceAudioDemoCubeController : MonoBehaviour
{
	private Material material;

	private void Start()
	{
		material = GetComponent<Renderer>().material;
		SetGazedAt(gazedAt: false);
	}

	public void SetGazedAt(bool gazedAt)
	{
		material.color = (gazedAt ? Color.green : Color.red);
	}

	public void TeleportRandomly()
	{
		Vector3 onUnitSphere = Random.onUnitSphere;
		onUnitSphere.y = Mathf.Clamp(onUnitSphere.y, 0.5f, 1f);
		float num = 2f * Random.value + 1.5f;
		base.transform.localPosition = num * onUnitSphere;
	}
}
