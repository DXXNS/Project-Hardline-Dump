using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResonanceAudioMaterialMapper : ScriptableObject
{
	private class UnityMaterialAcousticMeshData
	{
		public List<ResonanceAudioAcousticMesh> acousticMeshes;

		public List<int> subMeshIndices;

		public UnityMaterialAcousticMeshData()
		{
			acousticMeshes = new List<ResonanceAudioAcousticMesh>();
			subMeshIndices = new List<int>();
		}
	}

	private class TerrainAcousticMeshData
	{
		public List<ResonanceAudioAcousticMesh> acousticMeshes;

		public TerrainAcousticMeshData()
		{
			acousticMeshes = new List<ResonanceAudioAcousticMesh>();
		}
	}

	[SerializeField]
	private ResonanceAudioMaterialMap materialMap;

	private Dictionary<string, UnityMaterialAcousticMeshData> unityMaterialAcousticMeshDataFromGuid;

	private Dictionary<string, TerrainAcousticMeshData> terrainAcousticMeshDataFromGuid;

	[SerializeField]
	private LayerMask reverbLayerMask = -1;

	[SerializeField]
	private bool includeNonStaticGameObjects = true;

	public void Initialize()
	{
		unityMaterialAcousticMeshDataFromGuid = new Dictionary<string, UnityMaterialAcousticMeshData>();
		terrainAcousticMeshDataFromGuid = new Dictionary<string, TerrainAcousticMeshData>();
	}

	public void ApplyMaterialMapping(MeshRenderer[] meshRenderers, List<string>[] guidsForMeshRenderers, Terrain[] activeTerrains, string[] guidsForTerrains, Shader surfaceMaterialShader)
	{
		BuildUnityMaterialData(meshRenderers, guidsForMeshRenderers, surfaceMaterialShader);
		BuildTerrainData(activeTerrains, guidsForTerrains, surfaceMaterialShader);
		ApplyMaterialMappingToGuids(materialMap.GuidList());
		ApplyObjectFiltering();
	}

	public void RenderAcousticMeshes()
	{
		List<ResonanceAudioAcousticMesh> includedAcousticMeshes = GetIncludedAcousticMeshes();
		for (int i = 0; i < includedAcousticMeshes.Count; i++)
		{
			includedAcousticMeshes[i].Render();
		}
	}

	public List<ResonanceAudioAcousticMesh> GetIncludedAcousticMeshes()
	{
		List<ResonanceAudioAcousticMesh> list = new List<ResonanceAudioAcousticMesh>();
		foreach (UnityMaterialAcousticMeshData value in unityMaterialAcousticMeshDataFromGuid.Values)
		{
			for (int i = 0; i < value.acousticMeshes.Count; i++)
			{
				ResonanceAudioAcousticMesh resonanceAudioAcousticMesh = value.acousticMeshes[i];
				if (resonanceAudioAcousticMesh.IsIncluded())
				{
					list.Add(resonanceAudioAcousticMesh);
				}
			}
		}
		foreach (TerrainAcousticMeshData value2 in terrainAcousticMeshDataFromGuid.Values)
		{
			for (int j = 0; j < value2.acousticMeshes.Count; j++)
			{
				ResonanceAudioAcousticMesh resonanceAudioAcousticMesh2 = value2.acousticMeshes[j];
				if (resonanceAudioAcousticMesh2.IsIncluded())
				{
					list.Add(resonanceAudioAcousticMesh2);
				}
			}
		}
		return list;
	}

	private void BuildUnityMaterialData(MeshRenderer[] meshRenderers, List<string>[] guidsForMeshRenderers, Shader surfaceMaterialShader)
	{
		unityMaterialAcousticMeshDataFromGuid.Clear();
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			MeshRenderer obj = meshRenderers[i];
			GameObject gameObject = obj.gameObject;
			Material[] sharedMaterials = obj.sharedMaterials;
			if (sharedMaterials.Length == 0 || !gameObject.activeInHierarchy)
			{
				continue;
			}
			ResonanceAudioAcousticMesh resonanceAudioAcousticMesh = ResonanceAudioAcousticMesh.GenerateFromMeshFilter(gameObject.GetComponent<MeshFilter>(), surfaceMaterialShader);
			if (resonanceAudioAcousticMesh == null)
			{
				continue;
			}
			List<string> list = guidsForMeshRenderers[i];
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (resonanceAudioAcousticMesh.IsSubMeshTriangular(j))
				{
					string text = list[j];
					materialMap.AddDefaultMaterialIfGuidUnmapped(text);
					if (!unityMaterialAcousticMeshDataFromGuid.ContainsKey(text))
					{
						unityMaterialAcousticMeshDataFromGuid[text] = new UnityMaterialAcousticMeshData();
					}
					UnityMaterialAcousticMeshData unityMaterialAcousticMeshData = unityMaterialAcousticMeshDataFromGuid[text];
					unityMaterialAcousticMeshData.acousticMeshes.Add(resonanceAudioAcousticMesh);
					unityMaterialAcousticMeshData.subMeshIndices.Add(j);
				}
			}
		}
	}

	private void BuildTerrainData(Terrain[] activeTerrains, string[] guidsForTerrains, Shader surfaceMaterialShader)
	{
		terrainAcousticMeshDataFromGuid.Clear();
		for (int i = 0; i < activeTerrains.Length; i++)
		{
			Terrain terrain = activeTerrains[i];
			string text = guidsForTerrains[i];
			ResonanceAudioAcousticMesh item = ResonanceAudioAcousticMesh.GenerateFromTerrain(terrain, surfaceMaterialShader);
			materialMap.AddDefaultMaterialIfGuidUnmapped(text);
			if (!terrainAcousticMeshDataFromGuid.ContainsKey(text))
			{
				terrainAcousticMeshDataFromGuid[text] = new TerrainAcousticMeshData();
			}
			terrainAcousticMeshDataFromGuid[text].acousticMeshes.Add(item);
		}
	}

	private void ApplyMaterialMappingToGuids(List<string> guids)
	{
		for (int i = 0; i < guids.Count; i++)
		{
			string text = guids[i];
			ResonanceAudioRoomManager.SurfaceMaterial materialFromGuid = materialMap.GetMaterialFromGuid(text);
			if (unityMaterialAcousticMeshDataFromGuid.ContainsKey(text))
			{
				ApplySurfaceMaterialToGameObjects(materialFromGuid, text);
			}
			else if (terrainAcousticMeshDataFromGuid.ContainsKey(text))
			{
				ApplySurfaceMaterialToTerrains(materialFromGuid, text);
			}
		}
	}

	private void ApplySurfaceMaterialToGameObjects(ResonanceAudioRoomManager.SurfaceMaterial surfaceMaterial, string guid)
	{
		UnityMaterialAcousticMeshData unityMaterialAcousticMeshData = unityMaterialAcousticMeshDataFromGuid[guid];
		if (unityMaterialAcousticMeshData.acousticMeshes.Count != unityMaterialAcousticMeshData.subMeshIndices.Count)
		{
			Debug.LogError("Number of acoustic meshes (" + unityMaterialAcousticMeshData.acousticMeshes.Count + ") != number of sub-mesh indices (" + unityMaterialAcousticMeshData.subMeshIndices.Count + ")");
		}
		List<ResonanceAudioAcousticMesh> acousticMeshes = unityMaterialAcousticMeshData.acousticMeshes;
		List<int> subMeshIndices = unityMaterialAcousticMeshData.subMeshIndices;
		for (int i = 0; i < acousticMeshes.Count; i++)
		{
			acousticMeshes[i].SetSurfaceMaterialToSubMesh(surfaceMaterial, subMeshIndices[i]);
		}
	}

	private void ApplySurfaceMaterialToTerrains(ResonanceAudioRoomManager.SurfaceMaterial surfaceMaterial, string guid)
	{
		List<ResonanceAudioAcousticMesh> list = terrainAcousticMeshDataFromGuid[guid].acousticMeshes.ToList();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SetSurfaceMaterialToAllSubMeshes(surfaceMaterial);
		}
	}

	private void ApplyObjectFiltering()
	{
		List<UnityMaterialAcousticMeshData> list = unityMaterialAcousticMeshDataFromGuid.Values.ToList();
		for (int i = 0; i < list.Count; i++)
		{
			UnityMaterialAcousticMeshData unityMaterialAcousticMeshData = list[i];
			for (int j = 0; j < unityMaterialAcousticMeshData.acousticMeshes.Count; j++)
			{
				ResonanceAudioAcousticMesh resonanceAudioAcousticMesh = unityMaterialAcousticMeshData.acousticMeshes[j];
				resonanceAudioAcousticMesh.isIncludedByObjectFiltering = IsIncludedByObjectFiltering(resonanceAudioAcousticMesh.sourceObject);
			}
		}
		List<TerrainAcousticMeshData> list2 = terrainAcousticMeshDataFromGuid.Values.ToList();
		for (int k = 0; k < list2.Count; k++)
		{
			TerrainAcousticMeshData terrainAcousticMeshData = list2[k];
			for (int l = 0; l < terrainAcousticMeshData.acousticMeshes.Count; l++)
			{
				ResonanceAudioAcousticMesh resonanceAudioAcousticMesh2 = terrainAcousticMeshData.acousticMeshes[l];
				resonanceAudioAcousticMesh2.isIncludedByObjectFiltering = IsIncludedByObjectFiltering(resonanceAudioAcousticMesh2.sourceObject);
			}
		}
	}

	private bool IsIncludedByObjectFiltering(GameObject gameObject)
	{
		if (((1 << gameObject.layer) & reverbLayerMask.value) == 0)
		{
			return false;
		}
		if (!includeNonStaticGameObjects)
		{
			return gameObject.isStatic;
		}
		return true;
	}
}
