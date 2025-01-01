using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	[SerializeField]
	private Slider sensitivitySlider;

	[SerializeField]
	private Text sensitivityText;

	[SerializeField]
	private Slider volumeSlider;

	[SerializeField]
	private Slider fovSlider;

	[SerializeField]
	private Text fovValue;

	[SerializeField]
	private Text volumeText;

	[SerializeField]
	private int settingResolutionIndex;

	private bool toggleLeanValue;

	private bool toggleChatValue;

	private Resolution[] resolutions;

	[SerializeField]
	private Dropdown resolutionDropdown;

	[SerializeField]
	private Toggle fullscreenToggle;

	[SerializeField]
	private Toggle toggleLean;

	[SerializeField]
	private Toggle toggleChat;

	[SerializeField]
	private Dropdown graphicsDropdown;

	private bool settingFullscreen;

	public int SettingResolutionIndex
	{
		get
		{
			return settingResolutionIndex;
		}
		set
		{
			settingResolutionIndex = value;
		}
	}

	public Resolution[] Resolutions
	{
		get
		{
			return resolutions;
		}
		set
		{
			resolutions = value;
		}
	}

	public Dropdown ResolutionDropdown
	{
		get
		{
			return resolutionDropdown;
		}
		set
		{
			resolutionDropdown = value;
		}
	}

	public Toggle FullscreenToggle
	{
		get
		{
			return fullscreenToggle;
		}
		set
		{
			fullscreenToggle = value;
		}
	}

	public bool SettingFullscreen
	{
		get
		{
			return settingFullscreen;
		}
		set
		{
			settingFullscreen = value;
		}
	}

	private void Start()
	{
		SetUpSettings();
		InitializeResolutionSettings();
	}

	public void SetUpSettings()
	{
		UpdateSensitivity(PlayerPrefs.GetFloat("Input_Sensitivity", 0.5f));
		UpdateVolume(PlayerPrefs.GetFloat("Audio_Volume", 0.5f));
		UpdateLeanToggle(PlayerPrefs.GetInt("Input_ToggleLean", 0));
		UpdateChatToggle(PlayerPrefs.GetInt("Gameplay_ToggleChat", 1));
		UpdateFOV(PlayerPrefs.GetInt("Video_Fov", 80));
		if ((bool)graphicsDropdown)
		{
			UpdateGraphics(PlayerPrefs.GetInt("Video_Graphics", 2));
		}
	}

	public void InitializeResolutionSettings()
	{
		if ((bool)ResolutionDropdown)
		{
			ResolutionDropdown.ClearOptions();
			Resolutions = Screen.resolutions;
			List<string> list = new List<string>();
			for (int i = 0; i < Resolutions.Length; i++)
			{
				string item = Resolutions[i].width + " x " + Resolutions[i].height + " " + Resolutions[i].refreshRate + "hz";
				list.Add(item);
			}
			ResolutionDropdown.AddOptions(list);
			SettingResolutionIndex = PlayerPrefs.GetInt("Display_ResolutionSettings", 0);
			ResolutionDropdown.value = SettingResolutionIndex;
			if (PlayerPrefs.GetInt("Display_Fullscreen", 1) == 1)
			{
				SettingFullscreen = true;
				FullscreenToggle.isOn = true;
			}
			else
			{
				SettingFullscreen = false;
				FullscreenToggle.isOn = false;
			}
		}
	}

	public void UpdateSensitivity()
	{
		sensitivityText.text = sensitivitySlider.value.ToString();
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateSensitivity(sensitivitySlider.value);
		}
		PlayerPrefs.SetFloat("Input_Sensitivity", sensitivitySlider.value);
	}

	public void UpdateSensitivity(float sensitivity)
	{
		sensitivityText.text = sensitivity.ToString();
		sensitivitySlider.value = sensitivity;
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateSensitivity(sensitivitySlider.value);
		}
		PlayerPrefs.SetFloat("Input_Sensitivity", sensitivitySlider.value);
	}

	public void UpdateVolume()
	{
		volumeText.text = volumeSlider.value.ToString();
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateVolume(volumeSlider.value);
		}
		PlayerPrefs.SetFloat("Audio_Volume", volumeSlider.value);
	}

	public void UpdateVolume(float volume)
	{
		volumeText.text = volume.ToString();
		volumeSlider.value = volume;
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateVolume(volumeSlider.value);
		}
		PlayerPrefs.SetFloat("Audio_Volume", volume);
	}

	public void UpdateFOV()
	{
		fovValue.text = fovSlider.value.ToString();
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateFOV(fovSlider.value);
		}
		PlayerPrefs.SetInt("Video_Fov", Mathf.RoundToInt(fovSlider.value));
	}

	public void UpdateFOV(int fov)
	{
		fovSlider.value = fov;
		fovValue.text = fovSlider.value.ToString();
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateFOV(fovSlider.value);
		}
		PlayerPrefs.SetInt("Video_Fov", fov);
	}

	public void UpdateLeanToggle()
	{
		toggleLeanValue = toggleLean.isOn;
		int value = 0;
		if (toggleLeanValue)
		{
			value = 1;
		}
		PlayerPrefs.SetInt("Input_ToggleLean", value);
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateToggleLean(toggleLeanValue);
		}
	}

	public void UpdateLeanToggle(int toggle)
	{
		if (toggle == 1)
		{
			toggleLeanValue = true;
		}
		else
		{
			toggleLeanValue = false;
		}
		toggleLean.isOn = toggleLeanValue;
		PlayerPrefs.SetInt("Input_ToggleLean", toggle);
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateToggleLean(toggleLeanValue);
		}
	}

	public void UpdateChatToggle()
	{
		toggleChatValue = toggleChat.isOn;
		int value = 0;
		if (toggleChatValue)
		{
			value = 1;
		}
		PlayerPrefs.SetInt("Gameplay_ToggleChat", value);
		ChatHandler.chatEnabled = toggleChatValue;
	}

	public void UpdateChatToggle(int toggle)
	{
		if ((bool)toggleChat)
		{
			if (toggle == 1)
			{
				toggleChatValue = true;
			}
			else
			{
				toggleChatValue = false;
			}
			toggleChat.isOn = toggleChatValue;
			PlayerPrefs.SetInt("Gameplay_ToggleChat", toggle);
			ChatHandler.chatEnabled = toggleChatValue;
		}
	}

	public void UpdateGraphics()
	{
		PlayerPrefs.SetInt("Video_Graphics", graphicsDropdown.value);
		QualitySettings.SetQualityLevel(graphicsDropdown.value);
	}

	public void UpdateGraphics(int setting)
	{
		graphicsDropdown.value = setting;
		PlayerPrefs.SetInt("Video_Graphics", setting);
		QualitySettings.SetQualityLevel(setting);
	}

	public void SetResolution()
	{
		SettingResolutionIndex = resolutionDropdown.value;
	}

	public void FullScreenToggle()
	{
		SettingFullscreen = fullscreenToggle.isOn;
	}

	public void ApplySettings()
	{
		PlayerPrefs.SetInt("Display_ResolutionSettings", SettingResolutionIndex);
		if (SettingFullscreen)
		{
			PlayerPrefs.SetInt("Display_Fullscreen", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Display_Fullscreen", 0);
		}
		Screen.SetResolution(Resolutions[SettingResolutionIndex].width, Resolutions[SettingResolutionIndex].height, SettingFullscreen);
	}
}
