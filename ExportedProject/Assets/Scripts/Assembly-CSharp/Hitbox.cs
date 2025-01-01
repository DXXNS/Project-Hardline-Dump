using UnityEngine;

public class Hitbox : MonoBehaviour
{
	[SerializeField]
	private Human player;

	[SerializeField]
	private float damageMultiplier;

	public Human Player
	{
		get
		{
			return player;
		}
		set
		{
			player = value;
		}
	}

	public float DamageMultiplier
	{
		get
		{
			return damageMultiplier;
		}
		set
		{
			damageMultiplier = value;
		}
	}

	public float getDamage(float rawDamage)
	{
		return rawDamage * DamageMultiplier;
	}
}
