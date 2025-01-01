using UnityEngine;

[AddComponentMenu("ResonanceAudio/ResonanceAudioSource")]
[RequireComponent(typeof(AudioSource))]
[ExecuteInEditMode]
public class ResonanceAudioSource : MonoBehaviour
{
	public enum Quality
	{
		Stereo = 0,
		Low = 1,
		High = 2
	}

	private enum EffectData
	{
		Id = 0,
		DistanceAttenuation = 1,
		RoomEffectsGain = 2,
		Gain = 3,
		DirectivityAlpha = 4,
		DirectivitySharpness = 5,
		ListenerDirectivityAlpha = 6,
		ListenerDirectivitySharpness = 7,
		Occlusion = 8,
		Quality = 9,
		NearFieldEffectGain = 10,
		Volume = 11
	}

	[Tooltip("Sets whether the room effects for the source should be bypassed.")]
	public bool bypassRoomEffects;

	[Range(0f, 1f)]
	[Tooltip("Controls the balance between a dipole pattern and an omnidirectional pattern for source emission. By varying this value, different directivity patterns can be formed.")]
	public float directivityAlpha;

	[Range(1f, 10f)]
	[Tooltip("Sets the sharpness of the source directivity pattern. Higher values will result in increased directivity.")]
	public float directivitySharpness = 1f;

	[Range(0f, 1f)]
	[Tooltip("Controls the balance between a dipole pattern and an omnidirectional pattern for listener sensitivity. By varying this value, different directivity patterns can be formed.")]
	public float listenerDirectivityAlpha;

	[Range(1f, 10f)]
	[Tooltip("Sets the sharpness of the listener directivity pattern. Higher values will result in increased directivity.")]
	public float listenerDirectivitySharpness = 1f;

	[Tooltip("Applies a gain to the source for adjustment of relative loudness.")]
	public float gainDb;

	[Tooltip("Sets whether the near field effect should be applied when the distance between the source and the listener is less than 1m (in Unity units).")]
	public bool nearFieldEffectEnabled;

	[Range(0f, 9f)]
	[Tooltip("Sets the nearfield effect gain. Note that the near field effect could result in up to ~9x gain boost on the source input, therefore, it is advised to set smaller gain values for louder sound sources to avoid clipping of the output signal.")]
	public float nearFieldEffectGain = 1f;

	[Tooltip("Sets whether the sound of the source should be occluded when there are other objects between the source and the listener.")]
	public bool occlusionEnabled;

	[Range(0f, 10f)]
	[Tooltip("Sets the occlusion effect intensity. Higher values will result in a stronger effect when the source is occluded.")]
	public float occlusionIntensity = 1f;

	[Tooltip("Sets the quality mode in which the spatial audio will be rendered. Higher quality modes allow increased fidelity at the cost of greater CPU usage.")]
	public Quality quality = Quality.High;

	private float currentOcclusion;

	private float nextOcclusionUpdate;

	public AudioSource audioSource { get; private set; }

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (!occlusionEnabled)
		{
			currentOcclusion = 0f;
		}
		else if (Time.time >= nextOcclusionUpdate)
		{
			nextOcclusionUpdate = Time.time + 0.2f;
			currentOcclusion = occlusionIntensity * ResonanceAudio.ComputeOcclusion(base.transform);
		}
		UpdateSource();
	}

	private void UpdateSource()
	{
		if (audioSource.clip != null && audioSource.clip.ambisonic)
		{
			audioSource.SetAmbisonicDecoderFloat(2, bypassRoomEffects ? 0f : ResonanceAudioRoomManager.ComputeRoomEffectsGain(base.transform.position));
			audioSource.SetAmbisonicDecoderFloat(3, ResonanceAudio.ConvertAmplitudeFromDb(gainDb));
		}
		else if (audioSource.spatialize)
		{
			audioSource.SetSpatializerFloat(2, bypassRoomEffects ? 0f : ResonanceAudioRoomManager.ComputeRoomEffectsGain(base.transform.position));
			audioSource.SetSpatializerFloat(3, ResonanceAudio.ConvertAmplitudeFromDb(gainDb));
			audioSource.SetSpatializerFloat(4, directivityAlpha);
			audioSource.SetSpatializerFloat(5, directivitySharpness);
			audioSource.SetSpatializerFloat(6, listenerDirectivityAlpha);
			audioSource.SetSpatializerFloat(7, listenerDirectivitySharpness);
			audioSource.SetSpatializerFloat(8, currentOcclusion);
			audioSource.SetSpatializerFloat(9, (float)quality);
			audioSource.SetSpatializerFloat(10, nearFieldEffectEnabled ? nearFieldEffectGain : 0f);
		}
	}
}
