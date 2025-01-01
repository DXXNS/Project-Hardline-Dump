using UnityEngine;

public class Dummy : Human
{
	public override void Start()
	{
		base.Start();
		LoadItem(Resources.Load("Weapon_Glock") as GameObject, localPlayer: false);
		UpdateTeamAppearance();
	}

	protected override void Death()
	{
		SpawnRagdoll(base.transform);
		base.Death();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateImpactForce();
	}

	public void LateUpdate()
	{
		UpdateMoveStateAnimationsValues();
		UpdateAnimations();
	}

	private void MirrorProcessed()
	{
	}
}
