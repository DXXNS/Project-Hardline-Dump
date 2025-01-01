using System;
using System.Collections.Generic;
using UnityEngine;

public static class ResonanceAudioRoomManager
{
	public enum SurfaceMaterial
	{
		Transparent = 0,
		AcousticCeilingTiles = 1,
		BrickBare = 2,
		BrickPainted = 3,
		ConcreteBlockCoarse = 4,
		ConcreteBlockPainted = 5,
		CurtainHeavy = 6,
		FiberglassInsulation = 7,
		GlassThin = 8,
		GlassThick = 9,
		Grass = 10,
		LinoleumOnConcrete = 11,
		Marble = 12,
		Metal = 13,
		ParquetOnConcrete = 14,
		PlasterRough = 15,
		PlasterSmooth = 16,
		PlywoodPanel = 17,
		PolishedConcreteOrTile = 18,
		Sheetrock = 19,
		WaterOrIceSurface = 20,
		WoodCeiling = 21,
		WoodPanel = 22
	}

	[Serializable]
	public class SurfaceMaterialDictionary : Dictionary<string, SurfaceMaterial>, ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<string> guids;

		[SerializeField]
		private List<SurfaceMaterial> surfaceMaterials;

		public SurfaceMaterialDictionary()
		{
			guids = new List<string>();
			surfaceMaterials = new List<SurfaceMaterial>();
		}

		public void OnBeforeSerialize()
		{
			guids.Clear();
			surfaceMaterials.Clear();
			using Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, SurfaceMaterial> current = enumerator.Current;
				guids.Add(current.Key);
				surfaceMaterials.Add(current.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			Clear();
			for (int i = 0; i < guids.Count; i++)
			{
				Add(guids[i], surfaceMaterials[i]);
			}
		}
	}

	private struct RoomEffectsRegion
	{
		public ResonanceAudioRoom room;

		public ResonanceAudioReverbProbe reverbProbe;

		public RoomEffectsRegion(ResonanceAudioRoom room, ResonanceAudioReverbProbe reverbProbe)
		{
			this.room = room;
			this.reverbProbe = reverbProbe;
		}
	}

	private static List<RoomEffectsRegion> roomEffectsRegions = new List<RoomEffectsRegion>();

	private static Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	public static float ComputeRoomEffectsGain(Vector3 sourcePosition)
	{
		if (roomEffectsRegions.Count == 0)
		{
			return 1f;
		}
		float num = 0f;
		RoomEffectsRegion roomEffectsRegion = roomEffectsRegions[roomEffectsRegions.Count - 1];
		if (roomEffectsRegion.room != null)
		{
			ResonanceAudioRoom room = roomEffectsRegion.room;
			bounds.size = Vector3.Scale(room.transform.lossyScale, room.size);
			Vector3 vector = Quaternion.Inverse(room.transform.rotation) * (sourcePosition - room.transform.position);
			Vector3 b = bounds.ClosestPoint(vector);
			num = Vector3.Distance(vector, b);
		}
		else
		{
			ResonanceAudioReverbProbe reverbProbe = roomEffectsRegion.reverbProbe;
			Vector3 vector2 = sourcePosition - reverbProbe.transform.position;
			if (reverbProbe.regionShape == ResonanceAudioReverbProbe.RegionShape.Box)
			{
				bounds.size = reverbProbe.GetScaledBoxRegionSize();
				vector2 = Quaternion.Inverse(reverbProbe.transform.rotation) * vector2;
				Vector3 b2 = bounds.ClosestPoint(vector2);
				num = Vector3.Distance(vector2, b2);
			}
			else
			{
				float scaledSphericalRegionRadius = reverbProbe.GetScaledSphericalRegionRadius();
				num = Mathf.Max(0f, vector2.magnitude - scaledSphericalRegionRadius);
			}
		}
		return ComputeRoomEffectsAttenuation(num);
	}

	public static void UpdateRoom(ResonanceAudioRoom room)
	{
		UpdateRoomEffectsRegions(room, IsListenerInsideRoom(room));
		UpdateRoomEffects();
	}

	public static void RemoveRoom(ResonanceAudioRoom room)
	{
		UpdateRoomEffectsRegions(room, isEnabled: false);
		UpdateRoomEffects();
	}

	public static void UpdateReverbProbe(ResonanceAudioReverbProbe reverbProbe)
	{
		UpdateRoomEffectsRegions(reverbProbe, IsListenerInsideVisibleReverbProbe(reverbProbe));
		UpdateRoomEffects();
	}

	public static void RemoveReverbProbe(ResonanceAudioReverbProbe reverbProbe)
	{
		UpdateRoomEffectsRegions(reverbProbe, isEnabled: false);
		UpdateRoomEffects();
	}

	private static void UpdateRoomEffectsRegions(ResonanceAudioRoom room, bool isEnabled)
	{
		int num = -1;
		for (int i = 0; i < roomEffectsRegions.Count; i++)
		{
			if (roomEffectsRegions[i].room == room)
			{
				num = i;
				break;
			}
		}
		if (isEnabled && num == -1)
		{
			roomEffectsRegions.Add(new RoomEffectsRegion(room, null));
		}
		else if (!isEnabled && num != -1)
		{
			roomEffectsRegions.RemoveAt(num);
		}
	}

	private static void UpdateRoomEffectsRegions(ResonanceAudioReverbProbe reverbProbe, bool isEnabled)
	{
		int num = -1;
		for (int i = 0; i < roomEffectsRegions.Count; i++)
		{
			if (roomEffectsRegions[i].reverbProbe == reverbProbe)
			{
				num = i;
				break;
			}
		}
		if (isEnabled && num == -1)
		{
			roomEffectsRegions.Add(new RoomEffectsRegion(null, reverbProbe));
		}
		else if (!isEnabled && num != -1)
		{
			roomEffectsRegions.RemoveAt(num);
		}
	}

	private static void UpdateRoomEffects()
	{
		if (roomEffectsRegions.Count == 0)
		{
			ResonanceAudio.DisableRoomEffects();
			return;
		}
		RoomEffectsRegion roomEffectsRegion = roomEffectsRegions[roomEffectsRegions.Count - 1];
		if (roomEffectsRegion.room != null)
		{
			ResonanceAudio.UpdateRoom(roomEffectsRegion.room);
		}
		else
		{
			ResonanceAudio.UpdateReverbProbe(roomEffectsRegion.reverbProbe);
		}
	}

	private static float ComputeRoomEffectsAttenuation(float distanceToRoom)
	{
		float f = 1f + distanceToRoom;
		return 1f / Mathf.Pow(f, 2f);
	}

	private static bool IsListenerInsideRoom(ResonanceAudioRoom room)
	{
		bool result = false;
		Transform listenerTransform = ResonanceAudio.ListenerTransform;
		if (listenerTransform != null)
		{
			Vector3 vector = listenerTransform.position - room.transform.position;
			Quaternion quaternion = Quaternion.Inverse(room.transform.rotation);
			bounds.size = Vector3.Scale(room.transform.lossyScale, room.size);
			result = bounds.Contains(quaternion * vector);
		}
		return result;
	}

	private static bool IsListenerInsideVisibleReverbProbe(ResonanceAudioReverbProbe reverbProbe)
	{
		Transform listenerTransform = ResonanceAudio.ListenerTransform;
		if (listenerTransform == null)
		{
			return false;
		}
		Vector3 vector = listenerTransform.position - reverbProbe.transform.position;
		if (reverbProbe.regionShape == ResonanceAudioReverbProbe.RegionShape.Sphere)
		{
			if (vector.magnitude > reverbProbe.GetScaledSphericalRegionRadius())
			{
				return false;
			}
		}
		else
		{
			Quaternion quaternion = Quaternion.Inverse(reverbProbe.transform.rotation);
			bounds.size = reverbProbe.GetScaledBoxRegionSize();
			if (!bounds.Contains(quaternion * vector))
			{
				return false;
			}
		}
		if (reverbProbe.onlyApplyWhenVisible && ResonanceAudio.ComputeOcclusion(reverbProbe.transform) > 0f)
		{
			return false;
		}
		return true;
	}
}
