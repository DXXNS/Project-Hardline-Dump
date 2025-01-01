using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Text>().text = "v" + Application.version.ToString();
	}

	private void Update()
	{
	}
}
