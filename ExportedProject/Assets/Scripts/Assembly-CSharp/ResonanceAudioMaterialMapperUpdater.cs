using UnityEngine;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class ResonanceAudioMaterialMapperUpdater : MonoBehaviour
{
	public delegate void RefreshMaterialMapperDelegate();

	public RefreshMaterialMapperDelegate RefreshMaterialMapper;

	private void Update()
	{
		if (Application.isEditor && !Application.isPlaying && RefreshMaterialMapper != null)
		{
			RefreshMaterialMapper();
		}
	}
}
