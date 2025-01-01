using System;
using EZCameraShake;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	private static float fixedUpdateCallRate = 0.02f;

	[SerializeField]
	private float fov;

	[SerializeField]
	private Player player;

	private float targFov;

	private float baseFov;

	private bool hasInitCameraComponents;

	private int occlusionMaskLayer = 8;

	private float globalGain = 10f;

	private float fov_kP = 0.3f;

	private Camera playersCamera;

	private Vector3 basePosition;

	private Vector3 baseRotation;

	[SerializeField]
	private Vector3 cameraShake;

	[SerializeField]
	private Vector3 cameraShakeVelocity;

	[SerializeField]
	private Vector3 targCameraShakeVelocity;

	[SerializeField]
	private float cameraShake_kP;

	[SerializeField]
	private float cameraShakeVelocity_kP;

	private Vector3 cameraSway;

	private float cameraSwayMagnitudeTarget;

	private float cameraSwayMagnitude;

	private float cameraSwaySpeed;

	private static float cameraSwayTransition_kP = 0.2f;

	private float swayTime;

	private float lean;

	private float applyCounterLean;

	public float TargFOV
	{
		get
		{
			return targFov;
		}
		set
		{
			targFov = value;
		}
	}

	public float BaseFov
	{
		get
		{
			return baseFov;
		}
		set
		{
			baseFov = value;
		}
	}

	private void Start()
	{
		basePosition = base.transform.localPosition;
		baseRotation = base.transform.localEulerAngles;
		playersCamera = GetComponent<Camera>();
		baseFov = PlayerPrefs.GetInt("Video_Fov", 80);
		fov = baseFov;
		playersCamera.fieldOfView = fov;
		if (player.hasAuthority && !hasInitCameraComponents)
		{
			base.gameObject.AddComponent<AudioListener>();
			base.gameObject.AddComponent<ResonanceAudioListener>();
			base.gameObject.GetComponent<ResonanceAudioListener>().occlusionMask = occlusionMaskLayer;
			base.gameObject.GetComponent<ResonanceAudioListener>().globalGainDb = globalGain;
			base.gameObject.GetComponent<ResonanceAudioListener>().stereoSpeakerModeEnabled = true;
			hasInitCameraComponents = true;
		}
	}

	public void EnableAudioListening(bool enabled)
	{
		if (enabled)
		{
			if ((bool)GetComponent<AudioListener>())
			{
				GetComponent<AudioListener>().enabled = true;
				GetComponent<ResonanceAudioListener>().enabled = true;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(GetComponent<ResonanceAudioListener>());
			UnityEngine.Object.Destroy(GetComponent<AudioListener>());
		}
	}

	private void UpdateAdditionalTransforms()
	{
		cameraShake += cameraShakeVelocity * Mathf.Min(Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, (0f - player.Lean) / 4f) + cameraSway;
	}

	private void FixedUpdate()
	{
		UpdateFOV();
		UpdateCameraSway();
	}

	private void LateUpdate()
	{
		UpdateAdditionalTransforms();
	}

	private void UpdateFOV()
	{
		fov += (targFov - fov) * fov_kP;
		playersCamera.fieldOfView = fov;
	}

	public void SetTargFOV(float targFov, bool active)
	{
		if (active)
		{
			this.targFov = targFov;
		}
		else
		{
			this.targFov = baseFov;
		}
	}

	public void SetBaseFOV(float fov)
	{
		BaseFov = fov;
	}

	public void UpdateCameraSway()
	{
		swayTime += cameraSwaySpeed;
		if (swayTime > MathF.PI * 2f)
		{
			swayTime = MathF.PI / 180f * (swayTime - 360f);
		}
		cameraSwayMagnitude += (cameraSwayMagnitudeTarget - cameraSwayMagnitude) * cameraSwayTransition_kP;
		cameraSway = new Vector3(cameraSwayMagnitude * Mathf.Sin(swayTime * 2f), cameraSwayMagnitude * Mathf.Sin(swayTime), 0f);
	}

	public void SetCameraSwayValues(float speed, float magnitude)
	{
		cameraSwaySpeed = speed;
		cameraSwayMagnitudeTarget = magnitude;
	}

	public void AddCameraShake(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
	{
		if (magnitude != 0f && roughness != 0f && fadeInTime != 0f && fadeOutTime != 0f)
		{
			CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
		}
	}

	public void ResetCamera()
	{
		if ((bool)CameraShaker.Instance)
		{
			CameraShaker.Instance.Reset();
		}
	}
}
