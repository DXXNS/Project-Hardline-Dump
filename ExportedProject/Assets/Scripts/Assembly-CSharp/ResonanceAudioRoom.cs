using UnityEngine;

[AddComponentMenu("ResonanceAudio/ResonanceAudioRoom")]
public class ResonanceAudioRoom : MonoBehaviour
{
	[Tooltip("Left wall surface material used to calculate the acoustic properties of the room.")]
	public ResonanceAudioRoomManager.SurfaceMaterial leftWall = ResonanceAudioRoomManager.SurfaceMaterial.ConcreteBlockCoarse;

	[Tooltip("Right wall surface material used to calculate the acoustic properties of the room.")]
	public ResonanceAudioRoomManager.SurfaceMaterial rightWall = ResonanceAudioRoomManager.SurfaceMaterial.ConcreteBlockCoarse;

	[Tooltip("Floor surface material used to calculate the acoustic properties of the room.")]
	public ResonanceAudioRoomManager.SurfaceMaterial floor = ResonanceAudioRoomManager.SurfaceMaterial.ParquetOnConcrete;

	[Tooltip("Ceiling surface material used to calculate the acoustic properties of the room.")]
	public ResonanceAudioRoomManager.SurfaceMaterial ceiling = ResonanceAudioRoomManager.SurfaceMaterial.PlasterRough;

	[Tooltip("Back wall surface material used to calculate the acoustic properties of the room.")]
	public ResonanceAudioRoomManager.SurfaceMaterial backWall = ResonanceAudioRoomManager.SurfaceMaterial.ConcreteBlockCoarse;

	[Tooltip("Front wall surface material used to calculate the acoustic properties of the room.")]
	public ResonanceAudioRoomManager.SurfaceMaterial frontWall = ResonanceAudioRoomManager.SurfaceMaterial.ConcreteBlockCoarse;

	[Tooltip("Adjusts what proportion of the direct sound is reflected back by each surface, after an appropriate delay. Reverberation is unaffected by this setting.")]
	public float reflectivity = 1f;

	[Tooltip("Adjusts the reverb gain in the room.")]
	public float reverbGainDb;

	[Tooltip("Adjusts the balance between high and low frequencies in the reverb. Increasing this value will increase high frequencies in the reverb, while decreasing the low frequencies respectively.")]
	public float reverbBrightness;

	[Tooltip("Adjusts the overall duration of the reverb by a positive scaling factor.")]
	public float reverbTime = 1f;

	[Tooltip("Sets the room dimensions in meters relative to the scale of the game object.")]
	public Vector3 size = Vector3.one;

	private void OnEnable()
	{
		ResonanceAudioRoomManager.UpdateRoom(this);
	}

	private void OnDisable()
	{
		ResonanceAudioRoomManager.RemoveRoom(this);
	}

	private void Update()
	{
		ResonanceAudioRoomManager.UpdateRoom(this);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, size);
	}
}
