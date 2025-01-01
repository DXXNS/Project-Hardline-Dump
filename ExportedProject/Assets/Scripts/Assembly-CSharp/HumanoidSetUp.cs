using System;
using UnityEngine;

public class HumanoidSetUp : MonoBehaviour
{
	[Tooltip("Static animator hips.")]
	public Transform masterRoot;

	[Tooltip("Ragdoll hips.")]
	public Transform slaveRoot;

	[Tooltip("Camera following the character.")]
	public Camera characterCamera;

	[Tooltip("Ragdoll looses strength when colliding with other objects except for objects with layers contained in this mask.")]
	public LayerMask dontLooseStrengthLayerMask;

	[NonSerialized]
	public MasterController masterController;

	[NonSerialized]
	public SlaveController slaveController;

	[NonSerialized]
	public AnimationFollowing animFollow;

	[NonSerialized]
	public Animator anim;

	private void Awake()
	{
		if (masterRoot == null)
		{
			Debug.LogError("masterRoot not assigned.");
		}
		if (slaveRoot == null)
		{
			Debug.LogError("slaveRoot not assigned.");
		}
		if (characterCamera == null)
		{
			Debug.LogError("characterCamera not assigned.");
		}
		masterController = GetComponentInChildren<MasterController>();
		if (masterController == null)
		{
			Debug.LogError("MasterControler not found.");
		}
		slaveController = GetComponentInChildren<SlaveController>();
		if (slaveController == null)
		{
			Debug.LogError("SlaveController not found.");
		}
		animFollow = GetComponentInChildren<AnimationFollowing>();
		if (animFollow == null)
		{
			Debug.LogError("AnimationFollowing not found.");
		}
		anim = GetComponentInChildren<Animator>();
		if (anim == null)
		{
			Debug.LogError("Animator not found.");
		}
	}
}
