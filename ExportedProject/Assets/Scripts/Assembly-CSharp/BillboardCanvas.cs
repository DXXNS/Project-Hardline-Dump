using UnityEngine;
using UnityEngine.UI;

public class BillboardCanvas : MonoBehaviour
{
	private static float baseFOV = 60f;

	[SerializeField]
	private bool scaleWithDistance;

	[SerializeField]
	private bool shrinkWhenLookedAt;

	[SerializeField]
	private float transparencyEffectWhenLookedAt = 0.3f;

	[SerializeField]
	private float shrinkAmountWhenLookedAt = 0.8f;

	[SerializeField]
	private Image[] images;

	[SerializeField]
	private RawImage[] rawImages;

	private float shrink_kp = 0.3f;

	private float visionCone = 8f;

	private float currentTransparency = 1f;

	private float currentShrinkSize = 1f;

	private Transform cameraTransform;

	[SerializeField]
	private float objectScale = 1f;

	[SerializeField]
	private float FOVScaleFactor;

	private Vector3 initialScale;

	[SerializeField]
	private float yPositionOffset;

	[SerializeField]
	private float distanceScaleFactor;

	[SerializeField]
	private float baseDistanceScale;

	private Vector3 basePosition;

	private void Start()
	{
		initialScale = base.transform.localScale;
		basePosition = base.transform.localPosition;
	}

	public void Update()
	{
		bool flag = false;
		Camera[] array = Object.FindObjectsOfType<Camera>();
		foreach (Camera camera in array)
		{
			if (camera.gameObject.activeSelf && camera.transform.tag == "SpectatorCamera")
			{
				flag = true;
				cameraTransform = camera.transform;
			}
		}
		if (!flag)
		{
			array = Object.FindObjectsOfType<Camera>();
			foreach (Camera camera2 in array)
			{
				if (camera2.gameObject.activeSelf && camera2.transform.tag == "PlayerCamera")
				{
					cameraTransform = camera2.transform;
				}
			}
		}
		if (!cameraTransform)
		{
			return;
		}
		if (shrinkWhenLookedAt)
		{
			UpdateLookShrink();
			Image[] array2 = images;
			foreach (Image image in array2)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, currentTransparency);
			}
			RawImage[] array3 = rawImages;
			foreach (RawImage rawImage in array3)
			{
				rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, currentTransparency);
			}
		}
		base.transform.LookAt(base.transform.position + cameraTransform.transform.rotation * Vector3.forward, cameraTransform.transform.rotation * Vector3.up);
		base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f);
		if (scaleWithDistance)
		{
			float distanceToPoint = new Plane(cameraTransform.transform.forward, cameraTransform.transform.position).GetDistanceToPoint(base.transform.position);
			base.transform.localScale = initialScale * (baseDistanceScale + distanceToPoint * objectScale * distanceScaleFactor * (1f - (cameraTransform.gameObject.GetComponent<Camera>().fieldOfView - baseFOV) / baseFOV * FOVScaleFactor)) * currentShrinkSize;
			base.transform.localPosition = new Vector3(basePosition.x, basePosition.y + distanceToPoint * yPositionOffset, basePosition.z);
		}
	}

	public void UpdateLookShrink()
	{
		if (Mathf.Abs(Quaternion.Dot(cameraTransform.rotation, Quaternion.LookRotation((cameraTransform.position - base.transform.position).normalized)) * 180f) < visionCone)
		{
			currentShrinkSize += (shrinkAmountWhenLookedAt - currentShrinkSize) * shrink_kp;
			currentTransparency += (transparencyEffectWhenLookedAt - currentTransparency) * shrink_kp;
		}
		else
		{
			currentShrinkSize += (1f - currentShrinkSize) * shrink_kp;
			currentTransparency += (1f - currentTransparency) * shrink_kp;
		}
	}
}
