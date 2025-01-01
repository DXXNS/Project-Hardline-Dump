using System.Collections.Generic;
using TrickShotAssets.CustomInputManager;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
	private static float notificationDisableDelay = 0.31f;

	private static float notificationDuration = 3.5f;

	private static float gameMenuHiddenDelay = 0.5f;

	private static float settingsHiddenDelay = 0.25f;

	private static float scoreboardHiddenDelay = 0.3f;

	private static float playerKilledByPanelDuration = 4f;

	private static float hideLatencyAfter = 3f;

	private bool scoreboardLastState;

	[SerializeField]
	private Text[] loadedAmmoTexts;

	[SerializeField]
	private Text[] availableAmmoTexts;

	[SerializeField]
	private Text eventNotification;

	[SerializeField]
	private GameObject primaryEvent;

	[SerializeField]
	private Text secondaryNotification;

	[SerializeField]
	private GameObject secondaryEvent;

	[SerializeField]
	private Text itemNameBoard;

	[SerializeField]
	private RawImage itemIconBoard;

	[SerializeField]
	protected GameObject settings;

	[SerializeField]
	protected GameObject keyBindings;

	private bool keyBindingsOpen;

	private SettingsManager settingsManager;

	[SerializeField]
	protected Scoreboard scoreboard;

	protected bool scoreboardOpen;

	[SerializeField]
	private Text healthText;

	[SerializeField]
	private Slider healthSlider;

	[SerializeField]
	private Slider healthTraceSlider;

	[SerializeField]
	private GameObject interactPrompt;

	[SerializeField]
	private Text interactionText;

	[SerializeField]
	private Text interactionTypeText;

	[SerializeField]
	private Text playerKilledByText;

	[SerializeField]
	protected GameObject waitingForPlayersScreen;

	protected bool allPlayersLoaded;

	protected Animator animator;

	private float healthTraceSlider_kP = 0.2f;

	private int lastAmmo;

	private int lastAvailableAmmo;

	[SerializeField]
	private Text spectatedPlayerText;

	[SerializeField]
	private Text latencyText;

	[SerializeField]
	private GameObject latencyIcons;

	private float lastLatencyCheck;

	private int latency;

	private float stableLatencyTime;

	[SerializeField]
	private GameObject latencyPanel;

	private bool alwaysShowLatency;

	private Human localPlayer;

	[SerializeField]
	protected List<Human> humans = new List<Human>();

	public bool AllPlayersLoaded
	{
		get
		{
			return allPlayersLoaded;
		}
		set
		{
			allPlayersLoaded = value;
		}
	}

	public bool ScoreboardOpen
	{
		get
		{
			return scoreboardOpen;
		}
		set
		{
			scoreboardOpen = value;
		}
	}

	public bool KeyBindingsOpen
	{
		get
		{
			return keyBindingsOpen;
		}
		set
		{
			keyBindingsOpen = value;
		}
	}

	protected virtual void Start()
	{
		stableLatencyTime = hideLatencyAfter;
		KeyBindingsOpen = false;
		settingsManager = Object.FindObjectOfType<SettingsManager>();
		animator = GetComponent<Animator>();
	}

	public void GetPlayers()
	{
		humans.Clear();
		Human[] array = Object.FindObjectsOfType<Human>();
		foreach (Human item in array)
		{
			humans.Add(item);
		}
	}

	private string GetLocalPlayerName()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.isLocalPlayer)
			{
				return player.HumanName;
			}
		}
		return null;
	}

	private Human GetLocalPlayer()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.isLocalPlayer)
			{
				return player;
			}
		}
		return null;
	}

	protected virtual void FixedUpdate()
	{
		if (scoreboard.LocalPlayerName == null)
		{
			scoreboard.LocalPlayerName = GetLocalPlayerName();
		}
		if (localPlayer == null)
		{
			localPlayer = GetLocalPlayer();
		}
		if ((bool)healthTraceSlider)
		{
			healthTraceSlider.value += (healthSlider.value - healthTraceSlider.value) * healthTraceSlider_kP;
		}
		UpdateGameUI();
	}

	public void SetAmmoTexts(int loadedAmmo, int availableAmmo)
	{
		Text[] array = loadedAmmoTexts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = loadedAmmo.ToString();
			if (loadedAmmo != lastAmmo)
			{
				GlitchAmmoDisplayEffect();
			}
		}
		array = availableAmmoTexts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = availableAmmo.ToString();
			if (availableAmmo != lastAvailableAmmo)
			{
				GlitchAmmoDisplayEffect();
			}
		}
		lastAmmo = loadedAmmo;
		lastAvailableAmmo = availableAmmo;
	}

	public void GlitchAmmoDisplayEffect()
	{
	}

	public void ToggleSettings(bool opened)
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MenuOpened = opened;
		}
		if (opened)
		{
			settings.SetActive(value: true);
			animator.SetBool("Settings Active", value: true);
		}
		else
		{
			Invoke("DisableSettings", settingsHiddenDelay);
			animator.SetBool("Settings Active", value: false);
			CloseKeyBindings();
		}
	}

	public void KilledByNotification(string playerKilledBy)
	{
		playerKilledByText.text = playerKilledBy;
		animator.SetBool("Show KilledByPanel", value: true);
		Invoke("CloseKilledByPanel", playerKilledByPanelDuration);
	}

	public void CloseKilledByPanel()
	{
		animator.SetBool("Show KilledByPanel", value: false);
	}

	public void DisableSettings()
	{
		settings.SetActive(value: false);
	}

	public virtual void UpdateGameUI()
	{
		if ((bool)localPlayer)
		{
			bool isSpectating = (localPlayer as Player).IsSpectating;
			animator.SetBool("Spectating", isSpectating);
			bool flag = true;
			if ((localPlayer as Player).Health <= 0f)
			{
				flag = false;
			}
			if ((settings.activeInHierarchy | scoreboard.gameObject.activeInHierarchy) || (!flag && !isSpectating))
			{
				animator.SetBool("GameUI Active", value: false);
				animator.SetBool("Timer Maximized", value: false);
			}
			else
			{
				animator.SetBool("Timer Maximized", value: true);
				animator.SetBool("GameUI Active", value: true);
			}
		}
	}

	public void OpenKeyBindings()
	{
		KeyBindingsOpen = true;
		keyBindings.SetActive(value: true);
	}

	public void CloseKeyBindings()
	{
		KeyBindingsOpen = false;
		keyBindings.SetActive(value: false);
	}

	public void EnableCursor()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void DisableCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void ShowNotification(string notification)
	{
		CancelInvoke("HideNotification");
		eventNotification.text = notification;
		primaryEvent.SetActive(value: true);
		animator.SetTrigger("Show Notification");
		Invoke("HideNotification", notificationDuration);
	}

	private void HideNotification()
	{
		animator.SetTrigger("Hide Notification");
		Invoke("DisableNotificationPlate", notificationDisableDelay);
	}

	private void DisableNotificationPlate()
	{
		primaryEvent.SetActive(value: false);
	}

	public void ShowSecondaryNotification()
	{
		secondaryEvent.SetActive(value: true);
	}

	public void SetSecondaryNotificationText(string text)
	{
		secondaryNotification.text = text;
	}

	public void HideSecondaryNotification()
	{
		secondaryEvent.SetActive(value: false);
	}

	public void UpdateHealthDisplay(float health)
	{
		healthText.text = "Health: " + Mathf.Round(health);
		if ((bool)healthSlider)
		{
			healthSlider.value = health;
		}
	}

	public void LeaveGame()
	{
		Object.FindObjectOfType<HardlineNetworkManager>().LeaveToMenuAndDisconnect();
	}

	public void SetScoreBoardState(bool state)
	{
		if ((bool)scoreboard)
		{
			if (state)
			{
				scoreboardOpen = true;
				scoreboard.gameObject.SetActive(value: true);
				animator.SetBool("Scoreboard Active", value: true);
				scoreboard.UpdateScoreboard();
			}
			else
			{
				scoreboardOpen = false;
				animator.SetBool("Scoreboard Active", value: false);
				if (scoreboardLastState && !IsInvoking("DisableScoreboard"))
				{
					Invoke("DisableScoreboard", scoreboardHiddenDelay);
				}
			}
		}
		scoreboardLastState = state;
	}

	public void DisableScoreboard()
	{
		CancelInvoke("DisableScoreboard");
		scoreboard.gameObject.SetActive(value: false);
	}

	public void UpdateInteractPrompt(bool enabled, string interactionType)
	{
		interactionText.text = InputManagerStorage.Instance.keyMappings[InputManagerStorage.Instance.GetIndexByName("Interact")].key.ToString();
		interactionTypeText.text = interactionType;
		if (enabled)
		{
			interactPrompt.SetActive(value: true);
		}
		else
		{
			interactPrompt.SetActive(value: false);
		}
	}

	public void DisableMapLoadingScreen()
	{
		waitingForPlayersScreen.SetActive(value: false);
	}

	public void UpdateSpectateText(string playerName)
	{
		spectatedPlayerText.text = playerName;
	}

	public void UpdateItemDisplay(string itemName, Texture itemIcon)
	{
		if ((bool)itemNameBoard)
		{
			itemNameBoard.text = itemName;
			if (itemIcon != null)
			{
				itemIconBoard.texture = itemIcon;
				itemIconBoard.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				itemIconBoard.color = new Color(1f, 1f, 1f, 0f);
			}
		}
	}

	public void UpdateLatency(int latency)
	{
		if (!latencyPanel)
		{
			return;
		}
		for (int i = 0; i < latencyIcons.transform.childCount; i++)
		{
			latencyIcons.transform.GetChild(i).gameObject.SetActive(value: false);
		}
		this.latency = latency;
		latencyText.text = latency.ToString();
		if (latency <= -1)
		{
			latencyIcons.transform.GetChild(0).gameObject.SetActive(value: true);
		}
		else if (latency < 100)
		{
			if (stableLatencyTime < hideLatencyAfter)
			{
				stableLatencyTime += Time.time - lastLatencyCheck;
				lastLatencyCheck = Time.time;
			}
			latencyIcons.transform.GetChild(1).gameObject.SetActive(value: true);
		}
		else if (latency < 200)
		{
			stableLatencyTime = 0f;
			latencyIcons.transform.GetChild(2).gameObject.SetActive(value: true);
		}
		else
		{
			stableLatencyTime = 0f;
			latencyIcons.transform.GetChild(3).gameObject.SetActive(value: true);
		}
		if (stableLatencyTime >= hideLatencyAfter && PlayerPrefs.GetInt("Display_AlwaysShowLatency", 0) == 0 && !alwaysShowLatency)
		{
			latencyPanel.SetActive(value: false);
		}
		else
		{
			latencyPanel.SetActive(value: true);
		}
	}
}
