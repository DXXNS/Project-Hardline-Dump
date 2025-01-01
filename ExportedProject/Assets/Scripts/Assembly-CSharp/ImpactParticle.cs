using UnityEngine;

public class ImpactParticle : MonoBehaviour
{
	[SerializeField]
	protected bool dontStickToImpact;

	protected float timePast;

	[SerializeField]
	protected float delayStick;

	[SerializeField]
	protected bool destroyIfFailStick;

	[SerializeField]
	protected bool stickToPlayers;

	protected static float CAST_RADIUS = 0.3f;

	protected static float CAST_LENGTH = 1f;

	protected int layerMask;

	protected bool hasStuck;

	public virtual void Start()
	{
		if (!dontStickToImpact && delayStick <= 0f)
		{
			Stick();
		}
	}

	public virtual void Update()
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

	public virtual void Stick()
	{
		if (stickToPlayers)
		{
			layerMask = ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Character")));
		}
		else
		{
			layerMask = ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Character")) | (1 << LayerMask.NameToLayer("Hitbox")));
		}
		if (Physics.Raycast(base.transform.position + base.transform.forward * 0.1f, -base.transform.forward, out var hitInfo, CAST_LENGTH, layerMask))
		{
			hasStuck = true;
			base.transform.SetParent(hitInfo.transform);
		}
	}
}
