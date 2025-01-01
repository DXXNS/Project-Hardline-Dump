using System;
using UnityEngine;

public class SlaveController : MonoBehaviour
{
	public RagdollState state;

	[SerializeField]
	[Tooltip("Determines how fast ragdoll loses strength when in contact.")]
	private float looseStrengthLerp = 1f;

	[SerializeField]
	[Tooltip("Determines how fast ragdoll gains strength after being freed from contact.")]
	private float gainStrengthLerp = 0.05f;

	[SerializeField]
	[Tooltip("Minimum force strength during collision.")]
	private float minContactForce = 0.1f;

	[SerializeField]
	[Tooltip("Minimum torque strength during collision.")]
	private float minContactTorque = 0.1f;

	[SerializeField]
	[Tooltip("Time of being dead expressed in seconds passed in sumulation.")]
	private float deadTime = 4f;

	private AnimationFollowing animFollow;

	private float maxTorqueCoefficient;

	private float maxForceCoefficient;

	private float currentDeadStep;

	private float currentStrength;

	[NonSerialized]
	public int currentNumberOfCollisions;

	private void Start()
	{
		HumanoidSetUp componentInParent = GetComponentInParent<HumanoidSetUp>();
		animFollow = componentInParent.animFollow;
		maxForceCoefficient = animFollow.forceCoefficient;
		maxTorqueCoefficient = animFollow.torqueCoefficient;
		currentNumberOfCollisions = 0;
		currentDeadStep = deadTime;
		currentStrength = 1f;
	}

	private void FixedUpdate()
	{
		animFollow.FollowAnimation();
		state = GetRagdollState();
		switch (state)
		{
		case RagdollState.DEAD:
			currentDeadStep += Time.fixedDeltaTime;
			if (currentDeadStep >= deadTime)
			{
				ComeAlive();
			}
			break;
		case RagdollState.LOOSING_STRENGTH:
			LooseStrength();
			break;
		case RagdollState.GAINING_STRENGTH:
			GainStrength();
			break;
		case RagdollState.FOLLOWING_ANIMATION:
			break;
		}
	}

	private RagdollState GetRagdollState()
	{
		if (!animFollow.isAlive)
		{
			return RagdollState.DEAD;
		}
		if (currentNumberOfCollisions != 0)
		{
			return RagdollState.LOOSING_STRENGTH;
		}
		if (currentStrength < 1f)
		{
			return RagdollState.GAINING_STRENGTH;
		}
		return RagdollState.FOLLOWING_ANIMATION;
	}

	private void LooseStrength()
	{
		currentStrength -= looseStrengthLerp * Time.fixedDeltaTime;
		currentStrength = Mathf.Clamp(currentStrength, 0f, 1f);
		InterpolateStrength(currentStrength);
	}

	private void GainStrength()
	{
		currentStrength += gainStrengthLerp * Time.fixedDeltaTime;
		currentStrength = Mathf.Clamp(currentStrength, 0f, 1f);
		InterpolateStrength(currentStrength);
	}

	private void InterpolateStrength(float ratio)
	{
		animFollow.forceCoefficient = Mathf.Lerp(minContactForce, maxForceCoefficient, ratio);
		animFollow.torqueCoefficient = Mathf.Lerp(minContactTorque, maxTorqueCoefficient, ratio);
	}

	[ContextMenu("Die")]
	private void Die()
	{
		animFollow.isAlive = false;
		currentDeadStep = 0f;
		ResetForces();
	}

	[ContextMenu("Come alive")]
	private void ComeAlive()
	{
		animFollow.isAlive = true;
	}

	[ContextMenu("Reset forces")]
	private void ResetForces()
	{
		animFollow.forceCoefficient = 0f;
		animFollow.torqueCoefficient = 0f;
		currentStrength = 0f;
	}
}
