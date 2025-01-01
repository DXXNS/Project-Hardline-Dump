using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ResonanceAudio
{
	[StructLayout(LayoutKind.Sequential)]
	private class RoomProperties
	{
		public float positionX;

		public float positionY;

		public float positionZ;

		public float rotationX;

		public float rotationY;

		public float rotationZ;

		public float rotationW;

		public float dimensionsX;

		public float dimensionsY;

		public float dimensionsZ;

		public ResonanceAudioRoomManager.SurfaceMaterial materialLeft;

		public ResonanceAudioRoomManager.SurfaceMaterial materialRight;

		public ResonanceAudioRoomManager.SurfaceMaterial materialBottom;

		public ResonanceAudioRoomManager.SurfaceMaterial materialTop;

		public ResonanceAudioRoomManager.SurfaceMaterial materialFront;

		public ResonanceAudioRoomManager.SurfaceMaterial materialBack;

		public float reflectionScalar;

		public float reverbGain;

		public float reverbTime;

		public float reverbBrightness;
	}

	private static Transform listenerTransform = null;

	public static readonly Color listenerDirectivityColor = 0.65f * Color.magenta;

	public static readonly Color sourceDirectivityColor = 0.65f * Color.blue;

	public const float distanceEpsilon = 0.01f;

	public const float maxDistanceLimit = 1000000f;

	public const float minDistanceLimit = 990099f;

	public const float maxGainDb = 24f;

	public const float minGainDb = -24f;

	public const float maxReverbBrightness = 1f;

	public const float minReverbBrightness = -1f;

	public const float maxReverbTime = 10f;

	public const float maxReflectivity = 2f;

	public const int maxNumOcclusionHits = 12;

	public const float occlusionDetectionInterval = 0.2f;

	private static readonly Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));

	private static RaycastHit[] occlusionHits = new RaycastHit[12];

	private static int occlusionMaskValue = -1;

	private static float[] roomPosition = new float[3];

	private static RoomProperties roomProperties = new RoomProperties();

	private static IntPtr roomPropertiesPtr = IntPtr.Zero;

	private static Matrix4x4 transformMatrix = Matrix4x4.identity;

	private const string pluginName = "audiopluginresonanceaudio";

	public static Transform ListenerTransform
	{
		get
		{
			if (listenerTransform == null)
			{
				AudioListener audioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
				if (audioListener != null)
				{
					listenerTransform = audioListener.transform;
				}
			}
			return listenerTransform;
		}
	}

	public static void UpdateAudioListener(ResonanceAudioListener listener)
	{
		occlusionMaskValue = listener.occlusionMask.value;
		SetListenerGain(ConvertAmplitudeFromDb(listener.globalGainDb));
		SetListenerStereoSpeakerMode(listener.stereoSpeakerModeEnabled);
	}

	public static void DisableRoomEffects()
	{
		SetRoomProperties(IntPtr.Zero, null);
		if (roomPropertiesPtr != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(roomPropertiesPtr);
			roomPropertiesPtr = IntPtr.Zero;
		}
	}

	public static void UpdateRoom(ResonanceAudioRoom room)
	{
		if (roomPropertiesPtr == IntPtr.Zero)
		{
			roomPropertiesPtr = Marshal.AllocHGlobal(Marshal.SizeOf(roomProperties));
		}
		UpdateRoomProperties(room);
		Marshal.StructureToPtr(roomProperties, roomPropertiesPtr, fDeleteOld: false);
		SetRoomProperties(roomPropertiesPtr, null);
		Marshal.DestroyStructure(roomPropertiesPtr, typeof(RoomProperties));
	}

	public static void UpdateReverbProbe(ResonanceAudioReverbProbe reverbPobe)
	{
		if (roomPropertiesPtr == IntPtr.Zero)
		{
			roomPropertiesPtr = Marshal.AllocHGlobal(Marshal.SizeOf(roomProperties));
		}
		UpdateRoomProperties(reverbPobe);
		Marshal.StructureToPtr(roomProperties, roomPropertiesPtr, fDeleteOld: false);
		SetRoomProperties(roomPropertiesPtr, reverbPobe.rt60s);
		Marshal.DestroyStructure(roomPropertiesPtr, typeof(RoomProperties));
	}

	public static bool StartRecording()
	{
		return false;
	}

	public static bool StopRecordingAndSaveToFile(string filePath, bool seamless)
	{
		return false;
	}

	public static void InitializeReverbComputer(float[] vertices, int[] triangles, int[] materialIndices, float scatteringCoefficient)
	{
	}

	public static bool ComputeRt60sAndProxyRoom(ResonanceAudioReverbProbe reverbProbe, int totalNumPaths, int numPathsPerBatch, int maxDepth, float energyThreshold, float listenerSphereRadius)
	{
		return false;
	}

	public static float ComputeOcclusion(Transform sourceTransform)
	{
		float num = 0f;
		if (ListenerTransform != null)
		{
			Vector3 position = listenerTransform.position;
			Vector3 direction = sourceTransform.position - position;
			int num2 = Physics.RaycastNonAlloc(position, direction, occlusionHits, direction.magnitude, occlusionMaskValue);
			for (int i = 0; i < num2; i++)
			{
				if (occlusionHits[i].transform != listenerTransform && occlusionHits[i].transform != sourceTransform)
				{
					num += 1f;
				}
			}
		}
		return num;
	}

	public static float ConvertAmplitudeFromDb(float db)
	{
		return Mathf.Pow(10f, 0.05f * db);
	}

	public static Vector2[] Generate2dPolarPattern(float alpha, float order, int resolution)
	{
		Vector2[] array = new Vector2[resolution];
		float num = MathF.PI * 2f / (float)resolution;
		for (int i = 0; i < resolution; i++)
		{
			float f = (float)i * num;
			float num2 = Mathf.Pow(Mathf.Abs(1f - alpha + alpha * Mathf.Cos(f)), order);
			array[i] = new Vector2(num2 * Mathf.Sin(f), num2 * Mathf.Cos(f));
		}
		return array;
	}

	private static void ConvertAudioTransformFromUnity(ref Vector3 position, ref Quaternion rotation)
	{
		transformMatrix = flipZ * Matrix4x4.TRS(position, rotation, Vector3.one) * flipZ;
		position = transformMatrix.GetColumn(3);
		rotation = Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1));
	}

	private static void SetProxyRoomProperties(ResonanceAudioReverbProbe reverbProbe, RoomProperties proxyRoomProperties)
	{
		reverbProbe.proxyRoomPosition.x = proxyRoomProperties.positionX;
		reverbProbe.proxyRoomPosition.y = proxyRoomProperties.positionY;
		reverbProbe.proxyRoomPosition.z = proxyRoomProperties.positionZ;
		reverbProbe.proxyRoomRotation.x = proxyRoomProperties.rotationX;
		reverbProbe.proxyRoomRotation.y = proxyRoomProperties.rotationY;
		reverbProbe.proxyRoomRotation.z = proxyRoomProperties.rotationZ;
		reverbProbe.proxyRoomRotation.w = proxyRoomProperties.rotationW;
		reverbProbe.proxyRoomSize.x = proxyRoomProperties.dimensionsX;
		reverbProbe.proxyRoomSize.y = proxyRoomProperties.dimensionsY;
		reverbProbe.proxyRoomSize.z = proxyRoomProperties.dimensionsZ;
		reverbProbe.proxyRoomLeftWall = proxyRoomProperties.materialLeft;
		reverbProbe.proxyRoomRightWall = proxyRoomProperties.materialRight;
		reverbProbe.proxyRoomFloor = proxyRoomProperties.materialBottom;
		reverbProbe.proxyRoomCeiling = proxyRoomProperties.materialTop;
		reverbProbe.proxyRoomBackWall = proxyRoomProperties.materialBack;
		reverbProbe.proxyRoomFrontWall = proxyRoomProperties.materialFront;
	}

	private static void UpdateRoomProperties(ResonanceAudioRoom room)
	{
		FillGeometryOfRoomProperties(room.transform.position, room.transform.rotation, Vector3.Scale(room.transform.lossyScale, room.size));
		FillWallMaterialsOfRoomProperties(room.leftWall, room.rightWall, room.floor, room.ceiling, room.frontWall, room.backWall);
		FillModifiersOfRoomProperties(room.reverbGainDb, room.reverbTime, room.reverbBrightness, room.reflectivity);
	}

	private static void UpdateRoomProperties(ResonanceAudioReverbProbe reverbProbe)
	{
		FillGeometryOfRoomProperties(reverbProbe.proxyRoomPosition, reverbProbe.proxyRoomRotation, reverbProbe.proxyRoomSize);
		FillWallMaterialsOfRoomProperties(reverbProbe.proxyRoomLeftWall, reverbProbe.proxyRoomRightWall, reverbProbe.proxyRoomFloor, reverbProbe.proxyRoomCeiling, reverbProbe.proxyRoomFrontWall, reverbProbe.proxyRoomBackWall);
		float reflectivity = 1f;
		FillModifiersOfRoomProperties(reverbProbe.reverbGainDb, reverbProbe.reverbTime, reverbProbe.reverbBrightness, reflectivity);
	}

	private static void FillGeometryOfRoomProperties(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		ConvertAudioTransformFromUnity(ref position, ref rotation);
		roomProperties.positionX = position.x;
		roomProperties.positionY = position.y;
		roomProperties.positionZ = position.z;
		roomProperties.rotationX = rotation.x;
		roomProperties.rotationY = rotation.y;
		roomProperties.rotationZ = rotation.z;
		roomProperties.rotationW = rotation.w;
		roomProperties.dimensionsX = scale.x;
		roomProperties.dimensionsY = scale.y;
		roomProperties.dimensionsZ = scale.z;
	}

	private static void FillWallMaterialsOfRoomProperties(ResonanceAudioRoomManager.SurfaceMaterial leftWall, ResonanceAudioRoomManager.SurfaceMaterial rightWall, ResonanceAudioRoomManager.SurfaceMaterial floor, ResonanceAudioRoomManager.SurfaceMaterial ceiling, ResonanceAudioRoomManager.SurfaceMaterial frontWall, ResonanceAudioRoomManager.SurfaceMaterial backWall)
	{
		roomProperties.materialLeft = leftWall;
		roomProperties.materialRight = rightWall;
		roomProperties.materialBottom = floor;
		roomProperties.materialTop = ceiling;
		roomProperties.materialFront = frontWall;
		roomProperties.materialBack = backWall;
	}

	private static void FillModifiersOfRoomProperties(float reverbGainDb, float reverbTime, float reverbBrightness, float reflectivity)
	{
		roomProperties.reverbGain = ConvertAmplitudeFromDb(reverbGainDb);
		roomProperties.reverbTime = reverbTime;
		roomProperties.reverbBrightness = reverbBrightness;
		roomProperties.reflectionScalar = reflectivity;
	}

	[DllImport("audiopluginresonanceaudio")]
	private static extern void SetListenerGain(float gain);

	[DllImport("audiopluginresonanceaudio")]
	private static extern void SetListenerStereoSpeakerMode(bool enableStereoSpeakerMode);

	[DllImport("audiopluginresonanceaudio")]
	private static extern void SetRoomProperties(IntPtr roomProperties, float[] rt60s);
}
