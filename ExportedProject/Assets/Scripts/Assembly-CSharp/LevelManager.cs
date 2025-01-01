using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	private static bool showDisconnectedPanelFlag;

	public static bool ShowDisconnectedPanelFlag
	{
		get
		{
			return showDisconnectedPanelFlag;
		}
		set
		{
			showDisconnectedPanelFlag = value;
		}
	}

	public void Start()
	{
		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Video_Graphics", 2));
	}

	public void Update()
	{
		AudioListener.volume = PlayerPrefs.GetFloat("Audio_Volume", 0.5f);
		if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Lobby")
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		if (ShowDisconnectedPanelFlag)
		{
			_ = (bool)Object.FindObjectOfType<DisconnectedPanel>(includeInactive: true);
			Object.FindObjectOfType<DisconnectedPanel>(includeInactive: true).gameObject.SetActive(value: true);
			ShowDisconnectedPanelFlag = false;
		}
	}

	public void LoadShootingRange()
	{
		SceneManager.LoadScene("Main");
	}

	public void LoadSettings()
	{
		SceneManager.LoadScene("Settings");
	}

	public void LoadKeyBindings()
	{
		SceneManager.LoadScene("KeyBindings");
	}

	public void LoadCustomization()
	{
		SceneManager.LoadScene("LoadoutSelect");
	}

	public void LoadHowToPlay()
	{
		SceneManager.LoadScene("How to Play");
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}

	public void LoadLobby()
	{
		SceneManager.LoadScene("Lobby");
	}

	public void LoadCredits()
	{
		SceneManager.LoadScene("Credits");
	}

	public void DisconnectFromGame()
	{
		MonoBehaviour.print("disconnect from game");
		Object.FindObjectOfType<HardlineNetworkManager>().LeaveToMenuAndDisconnect();
	}

	public void LeaveToMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
