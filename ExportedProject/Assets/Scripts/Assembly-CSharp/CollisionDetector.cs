using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
	private SlaveController slaveController;

	private LayerMask layerMask;

	private void Start()
	{
		HumanoidSetUp componentInParent = GetComponentInParent<HumanoidSetUp>();
		slaveController = componentInParent.slaveController;
		layerMask = componentInParent.dontLooseStrengthLayerMask;
	}

	private bool CheckIfLayerIsInLayerMask(int layer)
	{
		return (int)layerMask == ((int)layerMask | (1 << layer));
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!CheckIfLayerIsInLayerMask(collision.gameObject.layer))
		{
			slaveController.currentNumberOfCollisions++;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (!CheckIfLayerIsInLayerMask(collision.gameObject.layer))
		{
			slaveController.currentNumberOfCollisions--;
		}
	}
}
