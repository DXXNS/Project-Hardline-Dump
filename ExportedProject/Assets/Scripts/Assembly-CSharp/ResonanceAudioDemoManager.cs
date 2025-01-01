using UnityEngine;

public class ResonanceAudioDemoManager : MonoBehaviour
{
	public Camera mainCamera;

	public ResonanceAudioDemoCubeController cube;

	private void Start()
	{
		Screen.sleepTimeout = -1;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(mainCamera.ViewportPointToRay(0.5f * Vector2.one), out hitInfo) && hitInfo.transform == cube.transform;
		cube.SetGazedAt(flag);
		if (flag && ((Input.touchCount == 0 && Input.GetMouseButtonDown(0)) || (Input.touchCount > 0 && Input.GetTouch(0).tapCount > 1 && Input.GetTouch(0).phase == TouchPhase.Began)))
		{
			cube.TeleportRandomly();
		}
	}
}
