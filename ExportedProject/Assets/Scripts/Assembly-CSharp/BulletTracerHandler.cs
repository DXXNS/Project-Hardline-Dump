using UnityEngine;

public class BulletTracerHandler : MonoBehaviour
{
	private TrailRenderer trailRenderer;

	[SerializeField]
	private Color startColor;

	private Color currentColor;

	[SerializeField]
	private float fadeDelay;

	[SerializeField]
	private float fadeSpeed;

	private bool fading;

	private void Start()
	{
		trailRenderer = GetComponent<TrailRenderer>();
		fading = false;
		trailRenderer.startColor = startColor;
		trailRenderer.endColor = startColor;
		currentColor = startColor;
		Invoke("StartFade", fadeDelay);
	}

	public void StartFade()
	{
		fading = true;
	}

	private void FixedUpdate()
	{
		if (fading)
		{
			currentColor = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a - fadeSpeed);
		}
		trailRenderer.startColor = currentColor;
		trailRenderer.endColor = currentColor;
	}
}
