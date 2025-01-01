using UnityEngine;

public class HandIKTarget : MonoBehaviour
{
	[SerializeField]
	private Vector3 startPosition = new Vector3(0f, -0.031f, 0f);

	[SerializeField]
	private Human player;

	[SerializeField]
	private Transform startRelateVector;

	[SerializeField]
	private Transform playerPivotPoint;

	private void Update()
	{
		UpdateTransforms();
	}

	public void UpdateTransforms()
	{
		base.transform.localPosition = startPosition;
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		if ((bool)player.Item)
		{
			PlayerItem item = player.Item;
			base.transform.RotateAround(player.ItemAlignment.transform.position, player.ItemAlignment.transform.right, item.transform.localEulerAngles.x);
			base.transform.RotateAround(player.ItemAlignment.transform.position, player.ItemAlignment.transform.forward, item.transform.localEulerAngles.z);
			base.transform.RotateAround(player.ItemAlignment.transform.position, player.ItemAlignment.transform.up, item.transform.localEulerAngles.y);
			base.transform.position += CalculateIKTargetPositionFromItem();
		}
	}

	private Vector3 CalculateIKTargetPositionFromItem()
	{
		PlayerItem item = player.Item;
		if ((bool)item && (bool)item.BaseTransform)
		{
			_ = startRelateVector.transform.TransformPoint(startPosition) + item.transform.TransformPoint(item.transform.localPosition) - item.transform.TransformPoint(0f, 0f, 0f);
			return item.BaseTransform.position - item.GetZeroedBaseTransformPosition();
		}
		return new Vector3(0f, 0f, 0f);
	}
}
