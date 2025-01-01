using UnityEngine;
using UnityEngine.UI;

public class PracticeModeUIManager : MonoBehaviour
{
	[SerializeField]
	private RawImage offlineMapIcon;

	[SerializeField]
	private Dropdown offlineMapSelect;

	[SerializeField]
	private Texture[] possibleMapIcons;

	private void Start()
	{
	}

	private void Update()
	{
		offlineMapIcon.texture = possibleMapIcons[offlineMapSelect.value];
	}
}
