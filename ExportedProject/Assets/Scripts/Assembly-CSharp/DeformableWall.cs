using System.Collections.Generic;
using System.Linq;
using Sebastian.Geometry;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class DeformableWall : MonoBehaviour
{
	private ProBuilderMesh proBuilderMesh;

	private ProBuilderMesh backFace;

	[SerializeField]
	private Vector3 wallDimensions;

	private List<Vector3> vertices = new List<Vector3>();

	private List<Vector3> savedVertices = new List<Vector3>();

	[SerializeField]
	private List<Vector2> hullVertices = new List<Vector2>();

	private List<List<Vector2>> holesList = new List<List<Vector2>>();

	private List<List<Vector2>> savedHolesList = new List<List<Vector2>>();

	private List<int> triangles = new List<int>();

	private List<int> savedTriangles = new List<int>();

	[SerializeField]
	private float frameLength;

	[SerializeField]
	private float wallThickness;

	[SerializeField]
	private float frameAdditionalThickness;

	[SerializeField]
	private float frameExtraLength;

	[SerializeField]
	private float frameMultiplier;

	[SerializeField]
	private Material wallMaterial;

	[SerializeField]
	private Material frameMaterial;

	[SerializeField]
	private float wallArmour;

	private int gizmosIterator;

	private bool updateMeshRequired;

	private void Start()
	{
		DestroyChildren();
		InitHull();
		ConstructWall();
	}

	private void OnDrawGizmos()
	{
		if (holesList.Count == 0)
		{
			return;
		}
		if (gizmosIterator >= vertices.Count)
		{
			gizmosIterator = 4;
		}
		foreach (Vector3 vertex in vertices)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(vertex + base.transform.position, 0.02f);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(vertices[gizmosIterator] + base.transform.position, 0.06f);
	}

	public void DestroyChildren()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Object.Destroy(base.transform.GetChild(i).gameObject);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			gizmosIterator++;
		}
	}

	private void LateUpdate()
	{
		if (updateMeshRequired)
		{
			updateMeshRequired = false;
			UpdateMesh();
		}
	}

	public void InitHull()
	{
		hullVertices.AddRange(new Vector2[4]
		{
			new Vector2((0f - wallDimensions.x) / 2f, (0f - wallDimensions.y) / 2f),
			new Vector2(wallDimensions.x / 2f, (0f - wallDimensions.y) / 2f),
			new Vector2(wallDimensions.x / 2f, wallDimensions.y / 2f),
			new Vector2((0f - wallDimensions.x) / 2f, wallDimensions.y / 2f)
		});
		new List<Vector2>(hullVertices.Count);
	}

	public void ConstructWall()
	{
		foreach (Vector2 hullVertex in hullVertices)
		{
			vertices.Add(new Vector3(hullVertex.x, hullVertex.y, 0f));
		}
		triangles.AddRange(new int[6] { 0, 3, 2, 1, 0, 2 });
		CreateFrame();
		UpdateMesh();
	}

	public void UpdateMesh()
	{
		MonoBehaviour.print("update mesh");
		if (GenerateTriangulation(holesList, hullVertices) != null)
		{
			triangles = GenerateTriangulation(holesList, hullVertices);
			List<Vertex> list = new List<Vertex>();
			for (int i = 0; i < vertices.Count; i++)
			{
				Vertex vertex = new Vertex();
				vertex.position = vertices[i];
				list.Add(vertex);
			}
			List<Face> list2 = new List<Face>();
			for (int j = 0; j + 2 < triangles.Count; j += 3)
			{
				list2.Add(new Face(new int[3]
				{
					triangles[j],
					triangles[j + 1],
					triangles[j + 2]
				}));
			}
			ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create(vertices.ToArray(), list2);
			List<Edge> list3 = new List<Edge>();
			int num = vertices.Count - 1;
			for (int num2 = holesList.Count - 1; num2 >= 0; num2--)
			{
				for (int k = 0; k < holesList[num2].Count; k++)
				{
					int num3 = k + 1;
					if (num3 >= holesList[num2].Count)
					{
						num3 -= holesList[num2].Count;
					}
					list3.Add(new Edge(num - k, num - num3));
				}
				num -= holesList[num2].Count;
			}
			for (int num4 = hullVertices.Count - 1; num4 >= 0; num4--)
			{
				int num5 = num4 - 1;
				if (num5 < 0)
				{
					num5 += hullVertices.Count;
				}
				list3.Add(new Edge(num4, num5));
			}
			for (int l = 0; l < proBuilderMesh.faceCount; l++)
			{
				proBuilderMesh.faces[l].Reverse();
			}
			Edge[] edges = proBuilderMesh.Extrude(list3.ToArray(), 0f, extrudeAsGroup: false, enableManifoldExtrude: false);
			proBuilderMesh.TranslateVertices(edges, Vector3.forward * wallThickness);
			Vector3[] array = vertices.ToArray();
			for (int m = 0; m < array.Length; m++)
			{
				array[m] += Vector3.forward * wallThickness;
			}
			ProBuilderMesh proBuilderMesh2 = ProBuilderMesh.Create(array, list2);
			for (int n = 0; n < proBuilderMesh2.faceCount; n++)
			{
				proBuilderMesh2.faces[n].Reverse();
			}
			proBuilderMesh.SetMaterial(proBuilderMesh.faces, wallMaterial);
			proBuilderMesh2.SetMaterial(proBuilderMesh.faces, wallMaterial);
			proBuilderMesh.ToMesh();
			proBuilderMesh2.Refresh();
			proBuilderMesh.transform.SetParent(base.transform);
			proBuilderMesh.transform.localPosition = Vector3.zero;
			proBuilderMesh.transform.localEulerAngles = Vector3.zero;
			proBuilderMesh2.transform.SetParent(base.transform);
			proBuilderMesh2.transform.localPosition = Vector3.zero;
			proBuilderMesh2.transform.localEulerAngles = Vector3.zero;
			proBuilderMesh.gameObject.AddComponent<MeshCollider>();
			proBuilderMesh.gameObject.AddComponent<GeneratedMesh>();
			proBuilderMesh2.gameObject.AddComponent<MeshCollider>();
			proBuilderMesh2.gameObject.AddComponent<GeneratedMesh>();
			if (this.proBuilderMesh != null)
			{
				Object.Destroy(this.proBuilderMesh.gameObject);
			}
			if (backFace != null)
			{
				Object.Destroy(backFace.gameObject);
			}
			this.proBuilderMesh = proBuilderMesh;
			backFace = proBuilderMesh2;
			savedTriangles = triangles;
			savedHolesList = holesList;
			savedVertices = vertices;
		}
		else
		{
			MonoBehaviour.print("illegal");
			triangles = savedTriangles;
			holesList = savedHolesList;
			vertices = savedVertices;
		}
	}

	public void CreateFrame()
	{
		MonoBehaviour.print("create frame");
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		List<Face> list3 = new List<Face>();
		List<List<Vector2>> list4 = new List<List<Vector2>>();
		List<Vector2> list5 = new List<Vector2>(new Vector2[4]
		{
			new Vector2((0f - wallDimensions.x) / 2f - frameExtraLength, (0f - wallDimensions.y) / 2f - frameExtraLength),
			new Vector2(wallDimensions.x / 2f + frameExtraLength, (0f - wallDimensions.y) / 2f - frameExtraLength),
			new Vector2(wallDimensions.x / 2f + frameExtraLength, wallDimensions.y / 2f + frameExtraLength),
			new Vector2((0f - wallDimensions.x) / 2f - frameExtraLength, wallDimensions.y / 2f + frameExtraLength)
		});
		list.AddRange(ConvertListVector2ToVector3(list5));
		List<Vector2> list6 = new List<Vector2>(new Vector2[4]
		{
			new Vector2((0f - wallDimensions.x) / 2f + frameLength * frameMultiplier, wallDimensions.y / 2f - frameLength * frameMultiplier),
			new Vector2(wallDimensions.x / 2f - frameLength * frameMultiplier, wallDimensions.y / 2f - frameLength * frameMultiplier),
			new Vector2(wallDimensions.x / 2f - frameLength * frameMultiplier, (0f - wallDimensions.y) / 2f + frameLength * frameMultiplier),
			new Vector2((0f - wallDimensions.x) / 2f + frameLength * frameMultiplier, (0f - wallDimensions.y) / 2f + frameLength * frameMultiplier)
		});
		list.AddRange(ConvertListVector2ToVector3(list6));
		list4.Add(list6);
		list2 = GenerateTriangulation(list4, list5);
		List<Vertex> list7 = new List<Vertex>();
		for (int i = 0; i < list.Count; i++)
		{
			Vertex vertex = new Vertex();
			vertex.position = list[i];
			list7.Add(vertex);
		}
		for (int j = 0; j + 2 < list2.Count; j += 3)
		{
			list3.Add(new Face(new int[3]
			{
				list2[j],
				list2[j + 1],
				list2[j + 2]
			}));
		}
		ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create(list.ToArray(), list3);
		List<Edge> list8 = new List<Edge>();
		MonoBehaviour.print("frame holes" + list4.Count);
		int num = list.Count - 1;
		for (int num2 = list4.Count - 1; num2 >= 0; num2--)
		{
			for (int k = 0; k < list4[num2].Count; k++)
			{
				int num3 = k + 1;
				if (num3 >= list4[num2].Count)
				{
					num3 -= list4[num2].Count;
				}
				list8.Add(new Edge(num - k, num - num3));
			}
			num -= list4[num2].Count;
		}
		for (int num4 = list5.Count - 1; num4 >= 0; num4--)
		{
			int num5 = num4 - 1;
			if (num5 < 0)
			{
				num5 += list5.Count;
			}
			list8.Add(new Edge(num4, num5));
		}
		for (int l = 0; l < proBuilderMesh.faceCount; l++)
		{
			proBuilderMesh.faces[l].Reverse();
		}
		Edge[] edges = proBuilderMesh.Extrude(list8.ToArray(), 0f, extrudeAsGroup: false, enableManifoldExtrude: false);
		proBuilderMesh.TranslateVertices(edges, Vector3.forward * (wallThickness + frameAdditionalThickness));
		Vector3[] array = list.ToArray();
		for (int m = 0; m < array.Length; m++)
		{
			array[m] += Vector3.forward * (wallThickness + frameAdditionalThickness);
		}
		ProBuilderMesh proBuilderMesh2 = ProBuilderMesh.Create(array, list3);
		for (int n = 0; n < proBuilderMesh2.faceCount; n++)
		{
			proBuilderMesh2.faces[n].Reverse();
		}
		proBuilderMesh.SetMaterial(proBuilderMesh.faces, frameMaterial);
		proBuilderMesh2.SetMaterial(proBuilderMesh.faces, frameMaterial);
		proBuilderMesh.transform.name = "frame";
		proBuilderMesh2.transform.name = "frameBack";
		proBuilderMesh.ToMesh();
		proBuilderMesh.Refresh();
		proBuilderMesh2.Refresh();
		MonoBehaviour.print(proBuilderMesh);
		proBuilderMesh.transform.SetParent(base.transform);
		proBuilderMesh.transform.localPosition = Vector3.zero;
		proBuilderMesh.transform.Translate(base.transform.forward * ((0f - frameAdditionalThickness) / 2f));
		proBuilderMesh.transform.localEulerAngles = Vector3.zero;
		proBuilderMesh2.transform.SetParent(base.transform);
		proBuilderMesh2.transform.localPosition = Vector3.zero;
		proBuilderMesh2.transform.Translate(base.transform.forward * ((0f - frameAdditionalThickness) / 2f));
		proBuilderMesh2.transform.localEulerAngles = Vector3.zero;
		proBuilderMesh.gameObject.AddComponent<MeshCollider>();
		proBuilderMesh2.gameObject.AddComponent<MeshCollider>();
	}

	public void AddDeformation(Vector3 hitPosition, float scale, float penetration)
	{
		if (penetration < wallArmour)
		{
			return;
		}
		Vector2 point = ConvertWorldTransforms(hitPosition);
		if (InsideAHole(point, holesList))
		{
			return;
		}
		Vector2[] clipperHole = new Vector2[8]
		{
			new Vector3(point.x - scale / 1.4f, point.y - scale / 1.4f),
			new Vector3(point.x - scale, point.y),
			new Vector3(point.x - scale / 1.4f, point.y + scale / 1.4f),
			new Vector3(point.x, point.y + scale),
			new Vector3(point.x + scale / 1.4f, point.y + scale / 1.4f),
			new Vector3(point.x + scale, point.y),
			new Vector3(point.x + scale / 1.4f, point.y - scale / 1.4f),
			new Vector3(point.x, point.y - scale)
		};
		List<Vector2> list = ClipHole(clipperHole);
		if (list != null)
		{
			clipperHole = list.ToArray();
			List<Vector3> list2 = new List<Vector3>();
			Vector2[] array = clipperHole;
			for (int i = 0; i < array.Length; i++)
			{
				Vector2 vector = array[i];
				list2.Add(new Vector3(vector.x, vector.y, 0f));
			}
			holesList.Add(new List<Vector2>(clipperHole));
			vertices.AddRange(list2);
			updateMeshRequired = true;
		}
	}

	public Vector2 ConvertWorldTransforms(Vector3 worldTransform)
	{
		Transform obj = new GameObject().transform;
		obj.position = worldTransform;
		obj.transform.parent = base.transform;
		Vector2 result = obj.localPosition;
		Object.Destroy(obj.gameObject);
		return result;
	}

	public List<int>? GenerateTriangulation(List<List<Vector2>> holesList, List<Vector2> hullVertices)
	{
		List<int> result = new List<int>();
		Vector2[][] holes = holesList.Select((List<Vector2> a) => a.ToArray()).ToArray();
		Triangulator triangulator = new Triangulator(new Polygon(hullVertices.ToArray(), holes));
		if (triangulator.Triangulate() != null)
		{
			result = new List<int>(triangulator.Triangulate());
		}
		else if (triangulator.Triangulate() == null)
		{
			return null;
		}
		return result;
	}

	private List<Vector2> ClipHole(Vector2[] clipperHole)
	{
		Vector2[] array = MeshGeneration.ClampClipperHole(clipperHole, hullVertices, frameLength);
		if (array == null)
		{
			return null;
		}
		new List<List<Vector2>>();
		int i = 0;
		bool flag = false;
		MergedClipperHole mergedClipperHole = new MergedClipperHole();
		MergedClipperHole mergedClipperHole2 = new MergedClipperHole();
		for (; i < clipperHole.Length; i++)
		{
			if (flag)
			{
				break;
			}
			mergedClipperHole = new MergedClipperHole(i, array, holesList);
			if (mergedClipperHole.Cancel)
			{
				return null;
			}
			_ = mergedClipperHole.Intersecting;
			if (Geometry.IsClockwise(mergedClipperHole.Vertices))
			{
				mergedClipperHole2 = mergedClipperHole;
				flag = true;
			}
		}
		if (flag)
		{
			RemoveHoles(mergedClipperHole2.HoleRemovalList);
			return mergedClipperHole2.Vertices;
		}
		MonoBehaviour.print("no valid clipperHole");
		return mergedClipperHole.Vertices;
	}

	public void RemoveHoles(List<List<Vector2>> holeRemovalList)
	{
		foreach (List<Vector2> holeRemoval in holeRemovalList)
		{
			holesList.Remove(holeRemoval);
			foreach (Vector2 item in holeRemoval)
			{
				vertices.Remove(item);
			}
		}
	}

	public bool InsideAHole(Vector2 point, List<List<Vector2>> holes)
	{
		bool result = false;
		for (int i = 0; i < holes.Count; i++)
		{
			if (Geometry.IsPointInPolygon(point, holes[i].ToArray()) == true)
			{
				MonoBehaviour.print("inside hole");
				return true;
			}
		}
		return result;
	}

	public List<Vector3> ConvertListVector2ToVector3(List<Vector2> list)
	{
		List<Vector3> list2 = new List<Vector3>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(new Vector3(list[i].x, list[i].y, 0f));
		}
		return list2;
	}
}
