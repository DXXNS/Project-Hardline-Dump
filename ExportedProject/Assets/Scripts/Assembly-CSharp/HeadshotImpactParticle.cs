using UnityEngine;

public class HeadshotImpactParticle : ImpactParticle
{
	private new static float CAST_RADIUS = 3f;

	public override void Start()
	{
		if (!dontStickToImpact && delayStick <= 0f)
		{
			Stick();
		}
	}

	public override void Update()
	{
		if (!dontStickToImpact)
		{
			if (timePast <= delayStick && !hasStuck)
			{
				Stick();
			}
			timePast += Time.deltaTime;
		}
		if (timePast > delayStick && destroyIfFailStick && !hasStuck)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public override void Stick()
	{
		bool flag = false;
		MonoBehaviour.print("check mask");
		Collider[] array = Physics.OverlapSphere(base.transform.position, CAST_RADIUS, LayerMask.GetMask("SpecialRaycast"));
		int num = 0;
		float num2 = CAST_RADIUS;
		for (int i = 0; i < array.Length; i++)
		{
			if (Vector3.Distance(array[i].transform.position, base.transform.position) < num2 && !array[i].transform.GetComponentInParent<Ragdoll>().PastInitPhase)
			{
				flag = true;
				num2 = Vector3.Distance(array[i].transform.position, base.transform.position);
				num = i;
			}
		}
		MonoBehaviour.print("find index: " + flag);
		if (flag)
		{
			hasStuck = true;
			base.transform.SetParent(array[num].transform);
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].Play();
			}
		}
	}
}
