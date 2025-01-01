using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ResonanceAudio/ResonanceAudioListener")]
[RequireComponent(typeof(AudioListener))]
[ExecuteInEditMode]
public class ResonanceAudioListener : MonoBehaviour
{
	[Tooltip("Sets the global gain for all spatialized audio sources. Can be used to adjust the overall output volume.")]
	public float globalGainDb;

	[Tooltip("Sets the global layer mask for occlusion detection.")]
	public LayerMask occlusionMask = -1;

	[Tooltip("Disables HRTF-based rendering and force stereo-panning only rendering for all spatialized audio sources. This mode is recommended only when the audio output is routed to a stereo loudspeaker configuration.")]
	public bool stereoSpeakerModeEnabled;

	[Tooltip("Sets whether the recorded soundfield clip should be saved as a seamless loop.")]
	public bool recorderSeamless;

	[Tooltip("Specify by tag which spatialized audio sources will be recorded. Choose \"Untagged\" to include all enabled spatialized audio sources in the scene.")]
	public string recorderSourceTag = "Untagged";

	[SerializeField]
	private bool recorderFoldout;

	private List<AudioSource> recorderTaggedSources;

	private double recorderStartTime;

	public bool IsRecording { get; private set; }

	private void OnEnable()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			IsRecording = false;
			recorderStartTime = 0.0;
			recorderTaggedSources = new List<AudioSource>();
		}
	}

	private void OnDisable()
	{
		if (Application.isEditor && IsRecording)
		{
			StopSoundfieldRecorder(null);
			Debug.LogWarning("Soundfield recording is stopped.");
		}
	}

	private void Update()
	{
		if (Application.isEditor && !Application.isPlaying && !IsRecording)
		{
			UpdateTaggedSources();
		}
		else
		{
			ResonanceAudio.UpdateAudioListener(this);
		}
	}

	public double GetCurrentRecordDuration()
	{
		if (IsRecording)
		{
			return AudioSettings.dspTime - recorderStartTime;
		}
		return 0.0;
	}

	public void StartSoundfieldRecorder()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Debug.LogError("Soundfield recording is only supported in Unity Editor \"Edit Mode\".");
			return;
		}
		if (IsRecording)
		{
			Debug.LogWarning("Soundfield recording is already in progress.");
			return;
		}
		recorderStartTime = AudioSettings.dspTime;
		for (int i = 0; i < recorderTaggedSources.Count; i++)
		{
			if (recorderTaggedSources[i].playOnAwake)
			{
				recorderTaggedSources[i].PlayScheduled(recorderStartTime);
			}
		}
		IsRecording = ResonanceAudio.StartRecording();
		if (!IsRecording)
		{
			Debug.LogError("Failed to start soundfield recording.");
			IsRecording = false;
			for (int j = 0; j < recorderTaggedSources.Count; j++)
			{
				recorderTaggedSources[j].Stop();
			}
		}
	}

	public void StopSoundfieldRecorder(string filePath)
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Debug.LogError("Soundfield recording is only supported in Unity Editor \"Edit Mode\".");
			return;
		}
		if (!IsRecording)
		{
			Debug.LogWarning("No recorded soundfield was found.");
			return;
		}
		IsRecording = false;
		recorderStartTime = 0.0;
		if (!ResonanceAudio.StopRecordingAndSaveToFile(filePath, recorderSeamless))
		{
			Debug.LogError("Failed to save soundfield recording into file.");
		}
		for (int i = 0; i < recorderTaggedSources.Count; i++)
		{
			recorderTaggedSources[i].Stop();
		}
	}

	private void UpdateTaggedSources()
	{
		recorderTaggedSources.Clear();
		AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
		for (int i = 0; i < array.Length; i++)
		{
			if ((recorderSourceTag == "Untagged" || array[i].tag == recorderSourceTag) && array[i].enabled && array[i].spatialize)
			{
				recorderTaggedSources.Add(array[i]);
			}
		}
	}
}
