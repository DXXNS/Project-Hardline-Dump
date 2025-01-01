using UnityEngine;

[AddComponentMenu("ResonanceAudio/ResonanceAudioReverbProbe")]
[ExecuteInEditMode]
public class ResonanceAudioReverbProbe : MonoBehaviour
{
	public enum RegionShape
	{
		Sphere = 0,
		Box = 1
	}

	[Tooltip("Time required in seconds for the reverb to decay by 60 dB for each frequency band.")]
	public float[] rt60s = new float[9];

	[Tooltip("Adjusts the reverb gain in the room.")]
	public float reverbGainDb;

	[Tooltip("Adjusts the balance between high and low frequencies in the reverb. Increasing this value will increase high frequencies in the reverb, while decreasing the low frequencies respectively.")]
	public float reverbBrightness;

	[Tooltip("Adjusts the overall duration of the reverb by a positive scaling factor.")]
	public float reverbTime = 1f;

	[Tooltip("Shape of the region of application of this reverb.")]
	public RegionShape regionShape = RegionShape.Box;

	[Tooltip("Sets the dimensions of a box-shaped region of application in meters relative to the scale of the game object.")]
	public Vector3 boxRegionSize = Vector3.one;

	[Tooltip("Sets the radius of a spherical region of application in meters relative to the scale of the game object.")]
	public float sphereRegionRadius = 1f;

	[Tooltip("Applies this reverb only when the center of the probe is visible from the listener. The visibility check will be done using physics raycast with respect to the Occlusion Mask selection in the ResonanceAudioListener component.")]
	public bool onlyApplyWhenVisible = true;

	public Vector3 proxyRoomPosition = Vector3.zero;

	public Quaternion proxyRoomRotation = Quaternion.identity;

	public Vector3 proxyRoomSize = Vector3.one;

	public ResonanceAudioRoomManager.SurfaceMaterial proxyRoomLeftWall;

	public ResonanceAudioRoomManager.SurfaceMaterial proxyRoomRightWall;

	public ResonanceAudioRoomManager.SurfaceMaterial proxyRoomFloor;

	public ResonanceAudioRoomManager.SurfaceMaterial proxyRoomCeiling;

	public ResonanceAudioRoomManager.SurfaceMaterial proxyRoomBackWall;

	public ResonanceAudioRoomManager.SurfaceMaterial proxyRoomFrontWall;

	private void OnEnable()
	{
		ResonanceAudioRoomManager.UpdateReverbProbe(this);
	}

	private void OnDisable()
	{
		ResonanceAudioRoomManager.RemoveReverbProbe(this);
	}

	private void Update()
	{
		ResonanceAudioRoomManager.UpdateReverbProbe(this);
	}

	public float GetScaledSphericalRegionRadius()
	{
		Vector3 lossyScale = base.transform.lossyScale;
		float num = Mathf.Max(Mathf.Max(lossyScale.x, lossyScale.y), lossyScale.z);
		return sphereRegionRadius * num;
	}

	public Vector3 GetScaledBoxRegionSize()
	{
		return Vector3.Scale(base.transform.lossyScale, boxRegionSize);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		switch (regionShape)
		{
		case RegionShape.Sphere:
			Gizmos.DrawWireSphere(base.transform.position, GetScaledSphericalRegionRadius());
			break;
		case RegionShape.Box:
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, boxRegionSize);
			break;
		}
	}
}
