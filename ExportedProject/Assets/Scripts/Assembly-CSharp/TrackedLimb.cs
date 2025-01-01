using UnityEngine;

public class TrackedLimb : MonoBehaviour
{
	[SerializeField]
	private Transform targetLimb;

	[SerializeField]
	private ConfigurableJoint configurableJoint;

	private Quaternion targetInitialRotation;

	private void Start()
	{
		configurableJoint = GetComponent<ConfigurableJoint>();
		targetInitialRotation = targetLimb.transform.localRotation;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		configurableJoint.targetRotation = CopyRotation();
	}

	private Quaternion CopyRotation()
	{
		return Quaternion.Inverse(targetLimb.localRotation) * targetInitialRotation;
	}
}
