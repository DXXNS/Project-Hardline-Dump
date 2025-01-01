using System;
using UnityEngine;

public class ResonanceAudioAcousticMesh
{
	public bool isIncludedByObjectFiltering;

	private ResonanceAudioRoomManager.SurfaceMaterial[] surfaceMaterialsFromSubMesh;

	private RangeInt[] triangleRangesFromSubMesh;

	private const int unityMaxNumVerticesPerMesh = 65000;

	private const int maxNumSubMeshes = 256;

	private Material visualizationMaterial;

	public Mesh mesh { get; private set; }

	public GameObject sourceObject { get; private set; }

	public static ResonanceAudioAcousticMesh GenerateFromMeshFilter(MeshFilter meshFilter, Shader surfaceMaterialShader)
	{
		GameObject gameObject = meshFilter.gameObject;
		Mesh sharedMesh = meshFilter.sharedMesh;
		if (sharedMesh == null)
		{
			Debug.LogWarning("GameObject: " + gameObject.name + " has no mesh and will not be included in reverb baking.");
			return null;
		}
		int numTriangleIndices = CountTriangleIndices(sharedMesh);
		int vertexCount = sharedMesh.vertexCount;
		ResonanceAudioAcousticMesh resonanceAudioAcousticMesh = new ResonanceAudioAcousticMesh();
		int[] triangles = null;
		Vector3[] vertices = null;
		resonanceAudioAcousticMesh.InitializeMesh(numTriangleIndices, vertexCount, out triangles, out vertices);
		resonanceAudioAcousticMesh.FillVerticesAndTrianglesFromMesh(sharedMesh, gameObject.transform, ref vertices, ref triangles);
		resonanceAudioAcousticMesh.mesh.vertices = vertices;
		resonanceAudioAcousticMesh.mesh.triangles = triangles;
		resonanceAudioAcousticMesh.mesh.RecalculateNormals();
		resonanceAudioAcousticMesh.InitializeSubMeshMaterials();
		resonanceAudioAcousticMesh.InitializeVisualizationMaterial(surfaceMaterialShader);
		resonanceAudioAcousticMesh.sourceObject = gameObject;
		return resonanceAudioAcousticMesh;
	}

	public static ResonanceAudioAcousticMesh GenerateFromTerrain(Terrain terrain, Shader surfaceMaterialShader)
	{
		TerrainData terrainData = terrain.terrainData;
		float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
		SubSampleHeightMap(heights.GetLength(0), heights.GetLength(1), out var m, out var n, out var subSampleStep, out var subSampledNumTriangleIndices);
		ResonanceAudioAcousticMesh resonanceAudioAcousticMesh = new ResonanceAudioAcousticMesh();
		resonanceAudioAcousticMesh.InitializeMesh(subSampledNumTriangleIndices, subSampledNumTriangleIndices, out var triangles, out var vertices);
		resonanceAudioAcousticMesh.FillTrianglesAndVerticesFromHeightMap(terrain.transform.position, terrainData.size, heights, m, n, subSampleStep, ref triangles, ref vertices);
		resonanceAudioAcousticMesh.mesh.vertices = vertices;
		resonanceAudioAcousticMesh.mesh.triangles = triangles;
		resonanceAudioAcousticMesh.mesh.RecalculateNormals();
		resonanceAudioAcousticMesh.InitializeSubMeshMaterials();
		resonanceAudioAcousticMesh.InitializeVisualizationMaterial(surfaceMaterialShader);
		resonanceAudioAcousticMesh.sourceObject = terrain.gameObject;
		return resonanceAudioAcousticMesh;
	}

	public int[] GetSurfaceMaterialIndicesFromTriangle()
	{
		int[] array = new int[mesh.triangles.Length / 3];
		for (int i = 0; i < surfaceMaterialsFromSubMesh.Length; i++)
		{
			int num = (int)surfaceMaterialsFromSubMesh[i];
			for (int j = triangleRangesFromSubMesh[i].start; j < triangleRangesFromSubMesh[i].end; j++)
			{
				array[j] = num;
			}
		}
		return array;
	}

	public void SetSurfaceMaterialToAllSubMeshes(ResonanceAudioRoomManager.SurfaceMaterial surfaceMaterial)
	{
		for (int i = 0; i < surfaceMaterialsFromSubMesh.Length; i++)
		{
			surfaceMaterialsFromSubMesh[i] = surfaceMaterial;
		}
		SetSubMeshSurfaceMaterials();
	}

	public void SetSurfaceMaterialToSubMesh(ResonanceAudioRoomManager.SurfaceMaterial surfaceMaterial, int subMeshIndex)
	{
		if (subMeshIndex < 0 || subMeshIndex >= triangleRangesFromSubMesh.Length)
		{
			Debug.LogError("subMeshIndex= " + subMeshIndex + " out of range [0, " + triangleRangesFromSubMesh.Length + "]");
		}
		else
		{
			surfaceMaterialsFromSubMesh[subMeshIndex] = surfaceMaterial;
			SetSubMeshSurfaceMaterials();
		}
	}

	public bool Render()
	{
		if (mesh == null)
		{
			return false;
		}
		Graphics.DrawMesh(mesh, Matrix4x4.identity, visualizationMaterial, 0);
		return true;
	}

	public bool IsIncluded()
	{
		if (!isIncludedByObjectFiltering)
		{
			return false;
		}
		if (mesh == null)
		{
			return false;
		}
		if (sourceObject == null || !sourceObject.activeInHierarchy)
		{
			return false;
		}
		return true;
	}

	public bool IsSubMeshTriangular(int subMeshIndex)
	{
		return triangleRangesFromSubMesh[subMeshIndex].length > 0;
	}

	private static int CountTriangleIndices(Mesh sourceMesh)
	{
		int num = 0;
		for (int i = 0; i < sourceMesh.subMeshCount; i++)
		{
			if (sourceMesh.GetTopology(i) == MeshTopology.Triangles)
			{
				num += (int)sourceMesh.GetIndexCount(i);
			}
		}
		return num;
	}

	private static void SubSampleHeightMap(int originalM, int originalN, out int m, out int n, out int subSampleStep, out int subSampledNumTriangleIndices)
	{
		m = originalM;
		n = originalN;
		subSampledNumTriangleIndices = (m - 1) * (n - 1) * 6;
		subSampleStep = 1;
		while (subSampledNumTriangleIndices >= 65000)
		{
			subSampleStep *= 2;
			m = (m - 1) / 2 + 1;
			n = (n - 1) / 2 + 1;
			subSampledNumTriangleIndices = (m - 1) * (n - 1) * 6;
		}
	}

	private void InitializeMesh(int numTriangleIndices, int numVertices, out int[] triangles, out Vector3[] vertices)
	{
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		triangles = mesh.triangles;
		Array.Resize(ref triangles, numTriangleIndices);
		vertices = mesh.vertices;
		Array.Resize(ref vertices, numVertices);
	}

	private void InitializeSubMeshMaterials()
	{
		int num = triangleRangesFromSubMesh.Length;
		if (surfaceMaterialsFromSubMesh == null || surfaceMaterialsFromSubMesh.Length != num)
		{
			surfaceMaterialsFromSubMesh = new ResonanceAudioRoomManager.SurfaceMaterial[num];
			for (int i = 0; i < num; i++)
			{
				surfaceMaterialsFromSubMesh[i] = ResonanceAudioRoomManager.SurfaceMaterial.Transparent;
			}
		}
	}

	private void InitializeVisualizationMaterial(Shader surfaceMaterialShader)
	{
		if (visualizationMaterial == null)
		{
			visualizationMaterial = new Material(surfaceMaterialShader);
		}
		SetSubMeshEnds();
	}

	private void FillVerticesAndTrianglesFromMesh(Mesh sourceMesh, Transform sourceObjectTransform, ref Vector3[] vertices, ref int[] triangles)
	{
		Vector3[] vertices2 = sourceMesh.vertices;
		for (int i = 0; i < vertices2.Length; i++)
		{
			vertices[i] = sourceObjectTransform.TransformPoint(vertices2[i]);
		}
		Array.Resize(ref triangleRangesFromSubMesh, sourceMesh.subMeshCount);
		Vector3 lossyScale = sourceObjectTransform.lossyScale;
		bool flag = lossyScale.x * lossyScale.y * lossyScale.z < 0f;
		int num = 0;
		for (int j = 0; j < sourceMesh.subMeshCount; j++)
		{
			triangleRangesFromSubMesh[j].start = num / 3;
			if (sourceMesh.GetTopology(j) != 0)
			{
				triangleRangesFromSubMesh[j].length = 0;
				continue;
			}
			int[] triangles2 = sourceMesh.GetTriangles(j);
			for (int k = 0; k < triangles2.Length; k += 3)
			{
				if (flag)
				{
					triangles[num] = triangles2[k + 2];
					triangles[num + 1] = triangles2[k + 1];
					triangles[num + 2] = triangles2[k];
				}
				else
				{
					triangles[num] = triangles2[k];
					triangles[num + 1] = triangles2[k + 1];
					triangles[num + 2] = triangles2[k + 2];
				}
				num += 3;
			}
			triangleRangesFromSubMesh[j].length = triangles2.Length / 3;
		}
	}

	private void FillTrianglesAndVerticesFromHeightMap(Vector3 terrainPosition, Vector3 terrainSize, float[,] heightMap, int m, int n, int subSampleStep, ref int[] triangles, ref Vector3[] vertices)
	{
		int num = 6;
		int[,] array = new int[6, 2]
		{
			{ 0, 0 },
			{ 1, 0 },
			{ 0, 1 },
			{ 0, 1 },
			{ 1, 0 },
			{ 1, 1 }
		};
		int length = heightMap.GetLength(0);
		int length2 = heightMap.GetLength(1);
		Vector3 b = Vector3.Scale(terrainSize, new Vector3(1f / (float)(length2 - 1), 1f, 1f / (float)(length - 1)));
		Vector3 a = default(Vector3);
		for (int i = 0; i < m - 1; i++)
		{
			for (int j = 0; j < n - 1; j++)
			{
				int num2 = i * (n - 1) + j;
				for (int k = 0; k < num; k++)
				{
					int num3 = num * num2 + k;
					triangles[num3] = num3;
					int num4 = (i + array[k, 0]) * subSampleStep;
					int num5 = (j + array[k, 1]) * subSampleStep;
					a.Set(num5, heightMap[num4, num5], num4);
					vertices[num3] = terrainPosition + Vector3.Scale(a, b);
				}
			}
		}
		Array.Resize(ref triangleRangesFromSubMesh, 1);
		triangleRangesFromSubMesh[0].start = 0;
		triangleRangesFromSubMesh[0].length = triangles.Length / 3;
	}

	private void SetSubMeshEnds()
	{
		int num = surfaceMaterialsFromSubMesh.Length;
		if (num > 256)
		{
			Debug.LogError("Too many sub-meshes: " + sourceObject.name + " has " + num + " sub-meshes. Sub-meshes more than " + 256 + " are not allowed.");
		}
		else
		{
			float[] array = new float[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = triangleRangesFromSubMesh[i].end;
			}
			visualizationMaterial.SetFloatArray("_SubMeshEnds", array);
			visualizationMaterial.SetInt("_NumSubMeshes", num);
		}
	}

	private void SetSubMeshSurfaceMaterials()
	{
		int num = surfaceMaterialsFromSubMesh.Length;
		float[] array = visualizationMaterial.GetFloatArray("_SubMeshMaterials");
		if (array == null)
		{
			array = new float[num];
		}
		for (int i = 0; i < surfaceMaterialsFromSubMesh.Length; i++)
		{
			array[i] = (float)surfaceMaterialsFromSubMesh[i];
		}
		visualizationMaterial.SetFloatArray("_SubMeshSurfaceMaterials", array);
	}
}
