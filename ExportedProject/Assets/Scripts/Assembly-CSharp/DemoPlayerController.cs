using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DemoPlayerController : MonoBehaviour
{
	public Camera mainCamera;

	private CharacterController characterController;

	private float movementSpeed = 5f;

	private float rotationX;

	private float rotationY;

	private const float clampAngleDegrees = 80f;

	private const float sensitivity = 2f;

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
		Vector3 eulerAngles = mainCamera.transform.localRotation.eulerAngles;
		rotationX = eulerAngles.x;
		rotationY = eulerAngles.y;
	}

	private void LateUpdate()
	{
		float num = Input.GetAxis("Mouse X");
		float num2 = 0f - Input.GetAxis("Mouse Y");
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			num = 0f;
			num2 = 0f;
		}
		rotationX += 2f * num2;
		rotationY += 2f * num;
		rotationX = Mathf.Clamp(rotationX, -80f, 80f);
		mainCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		Vector3 vector = new Vector3(axis, 0f, axis2);
		vector = mainCamera.transform.localRotation * vector;
		vector.y = 0f;
		characterController.SimpleMove(movementSpeed * vector);
	}

	private void SetCursorLock(bool lockCursor)
	{
		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
