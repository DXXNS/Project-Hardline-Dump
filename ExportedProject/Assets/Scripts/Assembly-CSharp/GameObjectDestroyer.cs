using UnityEngine;

public class GameObjectDestroyer : MonoBehaviour
{
	[SerializeField]
	private float time;

	public float Time
	{
		get
		{
			return time;
		}
		set
		{
			time = value;
		}
	}

	private void Start()
	{
		if (time != 0f)
		{
			Object.Destroy(base.gameObject, Time);
		}
	}

	public void InitTimedDestroy()
	{
		Object.Destroy(base.gameObject, Time);
	}
}
