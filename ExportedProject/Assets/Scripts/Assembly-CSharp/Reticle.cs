using UnityEngine;

public class Reticle : MonoBehaviour
{
	[SerializeField]
	private bool circularReticle;

	[SerializeField]
	private GameObject[] reticleLines;

	[SerializeField]
	private float reticleSizeConstant;

	[SerializeField]
	private float zeroPoint;

	[SerializeField]
	private PlayerItem item;

	private void Start()
	{
		item = GetComponentInParent<PlayerItem>();
	}

	private void Update()
	{
		if (item is PlayerFirearm)
		{
			if (circularReticle && reticleLines.Length == 1)
			{
				reticleLines[0].transform.localScale = new Vector3(zeroPoint + reticleSizeConstant * (item as PlayerFirearm).CurrentSpread, zeroPoint + reticleSizeConstant * (item as PlayerFirearm).CurrentSpread, 0f);
			}
			else if (!circularReticle && reticleLines.Length == 4)
			{
				reticleLines[0].transform.localPosition = new Vector3(0f, zeroPoint + reticleSizeConstant * (item as PlayerFirearm).CurrentSpread, 0f);
				reticleLines[1].transform.localPosition = new Vector3(zeroPoint + reticleSizeConstant * (item as PlayerFirearm).CurrentSpread, 0f, 0f);
				reticleLines[2].transform.localPosition = new Vector3(0f, 0f - zeroPoint - reticleSizeConstant * (item as PlayerFirearm).CurrentSpread, 0f);
				reticleLines[3].transform.localPosition = new Vector3(0f - zeroPoint - reticleSizeConstant * (item as PlayerFirearm).CurrentSpread, 0f, 0f);
			}
		}
	}
}
