using System.Collections.Generic;
using UnityEngine;

public class MeshDestroy : MonoBehaviour
{
	public class PartMesh
	{
		private List<Vector3> _Verticies = new List<Vector3>();

		private List<Vector3> _Normals = new List<Vector3>();

		private List<List<int>> _Triangles = new List<List<int>>();

		private List<Vector2> _UVs = new List<Vector2>();

		public Vector3[] Vertices;

		public Vector3[] Normals;

		public int[][] Triangles;

		public Vector2[] UV;

		public GameObject GameObject;

		public Bounds Bounds;

		public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			if (_Triangles.Count - 1 < submesh)
			{
				_Triangles.Add(new List<int>());
			}
			_Triangles[submesh].Add(_Verticies.Count);
			_Verticies.Add(vert1);
			_Triangles[submesh].Add(_Verticies.Count);
			_Verticies.Add(vert2);
			_Triangles[submesh].Add(_Verticies.Count);
			_Verticies.Add(vert3);
			_Normals.Add(normal1);
			_Normals.Add(normal2);
			_Normals.Add(normal3);
			_UVs.Add(uv1);
			_UVs.Add(uv2);
			_UVs.Add(uv3);
			Bounds.min = Vector3.Min(Bounds.min, vert1);
			Bounds.min = Vector3.Min(Bounds.min, vert2);
			Bounds.min = Vector3.Min(Bounds.min, vert3);
			Bounds.max = Vector3.Min(Bounds.max, vert1);
			Bounds.max = Vector3.Min(Bounds.max, vert2);
			Bounds.max = Vector3.Min(Bounds.max, vert3);
		}

		public void FillArrays()
		{
			Vertices = _Verticies.ToArray();
			Normals = _Normals.ToArray();
			UV = _UVs.ToArray();
			Triangles = new int[_Triangles.Count][];
			for (int i = 0; i < _Triangles.Count; i++)
			{
				Triangles[i] = _Triangles[i].ToArray();
			}
		}

		public void MakeGameobject(MeshDestroy original)
		{
			GameObject = new GameObject(original.name);
			GameObject.transform.position = original.transform.position;
			GameObject.transform.rotation = original.transform.rotation;
			GameObject.transform.localScale = original.transform.localScale;
			Mesh mesh = new Mesh();
			mesh.name = original.GetComponent<MeshFilter>().mesh.name;
			mesh.vertices = Vertices;
			mesh.normals = Normals;
			mesh.uv = UV;
			for (int i = 0; i < Triangles.Length; i++)
			{
				mesh.SetTriangles(Triangles[i], i, calculateBounds: true);
			}
			Bounds = mesh.bounds;
			GameObject.AddComponent<MeshRenderer>().materials = original.GetComponent<MeshRenderer>().materials;
			GameObject.AddComponent<MeshFilter>().mesh = mesh;
			GameObject.AddComponent<MeshCollider>().convex = true;
			GameObject.AddComponent<Rigidbody>();
			MeshDestroy meshDestroy = GameObject.AddComponent<MeshDestroy>();
			meshDestroy.CutCascades = original.CutCascades;
			meshDestroy.ExplodeForce = original.ExplodeForce;
		}
	}

	private bool edgeSet;

	private Vector3 edgeVertex = Vector3.zero;

	private Vector2 edgeUV = Vector2.zero;

	private Plane edgePlane;

	[SerializeField]
	private int cutCascades = 1;

	[SerializeField]
	private float explodeForce;

	public int CutCascades
	{
		get
		{
			return cutCascades;
		}
		set
		{
			cutCascades = value;
		}
	}

	public float ExplodeForce
	{
		get
		{
			return explodeForce;
		}
		set
		{
			explodeForce = value;
		}
	}

	private void Start()
	{
	}

	public void DestroyMesh()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.RecalculateBounds();
		List<PartMesh> list = new List<PartMesh>();
		List<PartMesh> list2 = new List<PartMesh>();
		PartMesh partMesh = new PartMesh
		{
			UV = mesh.uv,
			Vertices = mesh.vertices,
			Normals = mesh.normals,
			Triangles = new int[mesh.subMeshCount][],
			Bounds = mesh.bounds
		};
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			partMesh.Triangles[i] = mesh.GetTriangles(i);
		}
		list.Add(partMesh);
		for (int j = 0; j < CutCascades; j++)
		{
			for (int k = 0; k < list.Count; k++)
			{
				Bounds bounds = list[k].Bounds;
				bounds.Expand(0.5f);
				Plane plane = new Plane(Random.onUnitSphere, new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z)));
				list2.Add(GenerateMesh(list[k], plane, left: true));
				list2.Add(GenerateMesh(list[k], plane, left: false));
			}
			list = new List<PartMesh>(list2);
			list2.Clear();
		}
		for (int l = 0; l < list.Count; l++)
		{
			list[l].MakeGameobject(this);
			list[l].GameObject.GetComponent<Rigidbody>().AddForceAtPosition(list[l].Bounds.center * ExplodeForce, base.transform.position);
			GameObjectDestroyer gameObjectDestroyer = list[l].GameObject.AddComponent<GameObjectDestroyer>();
			gameObjectDestroyer.Time = 5f;
			gameObjectDestroyer.InitTimedDestroy();
			if ((bool)GameObject.Find("Temp"))
			{
				list[l].GameObject.transform.SetParent(GameObject.Find("Temp").transform);
			}
		}
		Object.Destroy(base.gameObject);
	}

	private PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
	{
		PartMesh partMesh = new PartMesh();
		Ray ray = default(Ray);
		Ray ray2 = default(Ray);
		for (int i = 0; i < original.Triangles.Length; i++)
		{
			int[] array = original.Triangles[i];
			edgeSet = false;
			for (int j = 0; j < array.Length; j += 3)
			{
				bool flag = plane.GetSide(original.Vertices[array[j]]) == left;
				bool flag2 = plane.GetSide(original.Vertices[array[j + 1]]) == left;
				bool flag3 = plane.GetSide(original.Vertices[array[j + 2]]) == left;
				int num = (flag ? 1 : 0) + (flag2 ? 1 : 0) + (flag3 ? 1 : 0);
				switch (num)
				{
				case 3:
					partMesh.AddTriangle(i, original.Vertices[array[j]], original.Vertices[array[j + 1]], original.Vertices[array[j + 2]], original.Normals[array[j]], original.Normals[array[j + 1]], original.Normals[array[j + 2]], original.UV[array[j]], original.UV[array[j + 1]], original.UV[array[j + 2]]);
					continue;
				case 0:
					continue;
				}
				int num2 = ((flag2 != flag3) ? ((flag == flag3) ? 1 : 2) : 0);
				ray.origin = original.Vertices[array[j + num2]];
				Vector3 vector2 = (ray.direction = original.Vertices[array[j + (num2 + 1) % 3]] - original.Vertices[array[j + num2]]);
				plane.Raycast(ray, out var enter);
				float t = enter / vector2.magnitude;
				ray2.origin = original.Vertices[array[j + num2]];
				Vector3 vector4 = (ray2.direction = original.Vertices[array[j + (num2 + 2) % 3]] - original.Vertices[array[j + num2]]);
				plane.Raycast(ray2, out var enter2);
				float t2 = enter2 / vector4.magnitude;
				AddEdge(i, partMesh, left ? (plane.normal * -1f) : plane.normal, ray.origin + ray.direction.normalized * enter, ray2.origin + ray2.direction.normalized * enter2, Vector2.Lerp(original.UV[array[j + num2]], original.UV[array[j + (num2 + 1) % 3]], t), Vector2.Lerp(original.UV[array[j + num2]], original.UV[array[j + (num2 + 2) % 3]], t2));
				switch (num)
				{
				case 1:
					partMesh.AddTriangle(i, original.Vertices[array[j + num2]], ray.origin + ray.direction.normalized * enter, ray2.origin + ray2.direction.normalized * enter2, original.Normals[array[j + num2]], Vector3.Lerp(original.Normals[array[j + num2]], original.Normals[array[j + (num2 + 1) % 3]], t), Vector3.Lerp(original.Normals[array[j + num2]], original.Normals[array[j + (num2 + 2) % 3]], t2), original.UV[array[j + num2]], Vector2.Lerp(original.UV[array[j + num2]], original.UV[array[j + (num2 + 1) % 3]], t), Vector2.Lerp(original.UV[array[j + num2]], original.UV[array[j + (num2 + 2) % 3]], t2));
					break;
				case 2:
					partMesh.AddTriangle(i, ray.origin + ray.direction.normalized * enter, original.Vertices[array[j + (num2 + 1) % 3]], original.Vertices[array[j + (num2 + 2) % 3]], Vector3.Lerp(original.Normals[array[j + num2]], original.Normals[array[j + (num2 + 1) % 3]], t), original.Normals[array[j + (num2 + 1) % 3]], original.Normals[array[j + (num2 + 2) % 3]], Vector2.Lerp(original.UV[array[j + num2]], original.UV[array[j + (num2 + 1) % 3]], t), original.UV[array[j + (num2 + 1) % 3]], original.UV[array[j + (num2 + 2) % 3]]);
					partMesh.AddTriangle(i, ray.origin + ray.direction.normalized * enter, original.Vertices[array[j + (num2 + 2) % 3]], ray2.origin + ray2.direction.normalized * enter2, Vector3.Lerp(original.Normals[array[j + num2]], original.Normals[array[j + (num2 + 1) % 3]], t), original.Normals[array[j + (num2 + 2) % 3]], Vector3.Lerp(original.Normals[array[j + num2]], original.Normals[array[j + (num2 + 2) % 3]], t2), Vector2.Lerp(original.UV[array[j + num2]], original.UV[array[j + (num2 + 1) % 3]], t), original.UV[array[j + (num2 + 2) % 3]], Vector2.Lerp(original.UV[array[j + num2]], original.UV[array[j + (num2 + 2) % 3]], t2));
					break;
				}
			}
		}
		partMesh.FillArrays();
		return partMesh;
	}

	private void AddEdge(int subMesh, PartMesh partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2)
	{
		if (!edgeSet)
		{
			edgeSet = true;
			edgeVertex = vertex1;
			edgeUV = uv1;
		}
		else
		{
			edgePlane.Set3Points(edgeVertex, vertex1, vertex2);
			partMesh.AddTriangle(subMesh, edgeVertex, edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2, edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1, normal, normal, normal, edgeUV, uv1, uv2);
		}
	}
}
