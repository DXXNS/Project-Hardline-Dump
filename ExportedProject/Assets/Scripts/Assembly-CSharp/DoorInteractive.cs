using UnityEngine;

public class DoorInteractive : MonoBehaviour
{
	[SerializeField]
	private Door door;

	public void Interact(Vector3 interactPosition)
	{
		float dot = Vector3.Dot(interactPosition - base.transform.position, base.transform.forward);
		door.Interact(dot);
	}

	public void SwingOtherWay(Vector3 interactPosition)
	{
		float dot = Vector3.Dot(interactPosition - base.transform.position, base.transform.forward);
		door.Open(dot);
	}

	public bool CheckOpenedCorrectly(Vector3 interactPosition)
	{
		float dot = Vector3.Dot(interactPosition - base.transform.position, base.transform.forward);
		return door.CheckOpenedCorrectly(dot);
	}

	public bool DoorOpen()
	{
		return door.IsOpen();
	}
}
