using UnityEngine;
using UnityEngine.UI;

public class LocalLobbyUIHandler : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private GameObject selectionMenu;

	[SerializeField]
	private GameObject gameLobbyMenu;

	[SerializeField]
	private InputField ipAddressInput;

	[SerializeField]
	private GameObject connectingToServerPanel;

	[SerializeField]
	private InputField serverNameHostInput;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public string GetHostServerName()
	{
		if (serverNameHostInput.text == "")
		{
			return "Server" + Random.Range(0, 9999);
		}
		return serverNameHostInput.text;
	}

	public void LoadMenu(string menu)
	{
		MonoBehaviour.print("menu " + menu);
		switch (menu)
		{
		case "selection":
			SetConnectingPanel(active: false);
			animator.SetInteger("Lobby State", 0);
			ipAddressInput.text = "";
			break;
		case "join lobby":
			animator.SetInteger("Lobby State", 0);
			ipAddressInput.text = "";
			break;
		case "game lobby":
			animator.SetInteger("Lobby State", 1);
			break;
		case "server browser":
			animator.SetInteger("Lobby State", 2);
			break;
		case "practice menu":
			animator.SetInteger("Lobby State", 3);
			break;
		case "empty":
			animator.SetInteger("Lobby State", -1);
			break;
		default:
			animator.SetInteger("Lobby State", -1);
			break;
		}
	}

	public void SetConnectingPanel(bool active)
	{
		connectingToServerPanel.SetActive(active);
	}
}
