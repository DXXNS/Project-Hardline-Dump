using UnityEngine;
using UnityEngine.UI;

public class Hitmarker : MonoBehaviour
{
	public RawImage[] lines;

	private float transparency;

	private float size;

	[SerializeField]
	private float maxSize;

	[SerializeField]
	private float minSize;

	[SerializeField]
	private float fadeDelay;

	private bool startFade;

	[SerializeField]
	private float fadeSpeed;

	[SerializeField]
	private float shrinkSpeed;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		UpdateHitmarkerAppearance();
	}

	private void UpdateHitmarkerAppearance()
	{
		if (transparency > fadeSpeed)
		{
			if (startFade)
			{
				transparency -= fadeSpeed;
			}
		}
		else
		{
			transparency = 0f;
		}
		if (size - minSize > shrinkSpeed)
		{
			size -= shrinkSpeed;
		}
		else
		{
			size = minSize;
		}
		int num = 0;
		RawImage[] array = lines;
		foreach (RawImage rawImage in array)
		{
			rawImage.color = new Vector4(rawImage.color.r, rawImage.color.g, rawImage.color.b, transparency);
			switch (num)
			{
			case 0:
				rawImage.transform.localPosition = new Vector3(size, size, 0f);
				break;
			case 1:
				rawImage.transform.localPosition = new Vector3(0f - size, size, 0f);
				break;
			case 2:
				rawImage.transform.localPosition = new Vector3(0f - size, 0f - size, 0f);
				break;
			case 3:
				rawImage.transform.localPosition = new Vector3(size, 0f - size, 0f);
				break;
			}
			num++;
		}
	}

	public void HitAnimation()
	{
		transparency = 1f;
		size = maxSize;
		startFade = false;
		Invoke("StartFade", fadeDelay);
	}

	private void StartFade()
	{
		startFade = true;
	}
}
