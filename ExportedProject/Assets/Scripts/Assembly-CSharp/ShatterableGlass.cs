using System;
using System.Collections.Generic;
using UnityEngine;

public class ShatterableGlass : MonoBehaviour
{
	private class Figure
	{
		public Vector2[] Points;

		public int ForceScale;

		public Figure(Vector2[] Points, int ForceScale)
		{
			this.Points = Points;
			this.ForceScale = ForceScale;
		}

		public void GenerateCollider(float GlassThickness, GameObject Obj)
		{
			BoxCollider boxCollider = Obj.AddComponent<BoxCollider>();
			float num = Vector2.Distance(Points[2], Points[0]);
			float num2 = Vector2.Distance(Points[2], Points[1]);
			float num3 = Vector2.Distance(Points[1], Points[0]);
			float num4 = num + num2 + num3;
			float x = (num * Points[0].x + num2 * Points[1].x + num3 * Points[2].x) / num4;
			float y = (num * Points[0].y + num2 * Points[1].y + num3 * Points[2].y) / num4;
			num4 /= 2f;
			float num5 = Mathf.Sqrt((num4 - num) * (num4 - num2) * (num4 - num3) / num4);
			num5 *= Mathf.Sqrt(2f);
			boxCollider.center = new Vector3(x, y, 0f);
			boxCollider.size = new Vector3(num5, num5, GlassThickness);
		}

		public Mesh GenerateMesh(bool GenerateGlassSides, float GlassHalfThickness, Vector2 UVScale)
		{
			Mesh mesh = new Mesh();
			mesh.name = "GlassGib";
			if (GenerateGlassSides)
			{
				mesh.subMeshCount = 2;
			}
			bool flag = Points.Length == 3;
			Vector3[] array = new Vector3[(!flag) ? (GenerateGlassSides ? 12 : 4) : (GenerateGlassSides ? 9 : 3)];
			Vector2[] array2 = new Vector2[array.Length];
			for (int i = 0; i < Points.Length; i++)
			{
				array[i] = Points[i];
				array2[i] = new Vector2(Points[i].x / UVScale.x, Points[i].y / UVScale.y) + new Vector2(0.5f, 0.5f);
			}
			int[] triangles = ((!flag) ? new int[6] { 0, 1, 2, 3, 2, 1 } : new int[3] { 2, 1, 0 });
			if (GenerateGlassSides)
			{
				int[] triangles2;
				if (flag)
				{
					for (int j = 0; j < 3; j++)
					{
						GlassSideVertex(Points[j], ref array[j * 2 + 3], ref array[j * 2 + 4], GlassHalfThickness);
					}
					triangles2 = new int[18]
					{
						3, 4, 5, 4, 6, 5, 3, 4, 7, 7,
						8, 4, 5, 6, 8, 8, 7, 5
					};
				}
				else
				{
					for (int k = 0; k < 4; k++)
					{
						GlassSideVertex(Points[k], ref array[k * 2 + 4], ref array[k * 2 + 5], GlassHalfThickness);
					}
					triangles2 = new int[24]
					{
						7, 5, 4, 6, 7, 4, 11, 7, 6, 10,
						11, 6, 10, 11, 9, 9, 8, 10, 8, 9,
						5, 8, 4, 5
					};
				}
				mesh.vertices = array;
				mesh.SetTriangles(triangles, 0);
				mesh.SetTriangles(triangles2, 1);
			}
			else
			{
				mesh.vertices = array;
				mesh.triangles = triangles;
			}
			mesh.uv = array2;
			return mesh;
		}

		private void GlassSideVertex(Vector2 Ref, ref Vector3 A, ref Vector3 B, float GlassHalfThickness)
		{
			A = new Vector3(Ref.x, Ref.y, GlassHalfThickness);
			B = new Vector3(Ref.x, Ref.y, 0f - GlassHalfThickness);
		}
	}

	private class BaseLine
	{
		public Vector2[] Points;

		public BaseLine(Vector2 HitPoint, Vector2 End, int Count)
		{
			Points = new Vector2[Count + 1];
			Points[0] = HitPoint;
			Points[Count] = End;
			float num = 1f / (float)Count;
			float num2 = num;
			float num3 = Mathf.Atan2(Mathf.Max(HitPoint.y, End.y) - Mathf.Min(HitPoint.y, End.y), Mathf.Max(End.x, HitPoint.x) - Mathf.Min(HitPoint.x, End.x));
			float num4 = MathF.PI / 4f;
			float num5 = MathF.PI / 2f;
			if (num3 > num4)
			{
				num3 = num5 - num3;
			}
			float t = num3 / num4;
			for (int i = 0; i < Count - 1; i++)
			{
				Points[i + 1] = Vector2.Lerp(HitPoint, End, num2 * Mathf.Lerp(1f, Mathf.Sqrt(2f) / 2f, t));
				num2 += num;
			}
		}
	}

	public int Sectors = 3;

	public int DetailsPerSector = 3;

	public float SimplifyThreshold = 0.05f;

	public bool GlassSides = true;

	public Material GlassSidesMaterial;

	public float GlassThickness = 0.01f;

	public bool ShatterButNotBreak;

	public bool SlightlyRotateGibs = true;

	public bool DestroyGibs = true;

	public float AfterSeconds = 5f;

	public bool GibsOnSeparateLayer;

	public int GibsLayer;

	private bool broken;

	public float Force = 100f;

	public bool AdoptFragments;

	private Vector2[] Bounds = new Vector2[4];

	private float Area = 1f;

	private Material GlassMaterial;

	private AudioSource SoundEmitter;

	public bool Broken
	{
		get
		{
			return broken;
		}
		set
		{
			broken = value;
		}
	}

	private void Start()
	{
		float num = Mathf.Abs(base.transform.lossyScale.x / 2f);
		float num2 = Mathf.Abs(base.transform.lossyScale.y / 2f);
		Area = num * num2;
		Bounds[0] = new Vector2(num, num2);
		Bounds[1] = new Vector2(0f - num, num2);
		Bounds[2] = new Vector2(0f - num, 0f - num2);
		Bounds[3] = new Vector2(num, 0f - num2);
		SoundEmitter = GetComponent<AudioSource>();
		if (GetComponent<Renderer>() == null || GetComponent<MeshFilter>() == null)
		{
			Debug.LogError(base.gameObject.name + ": No Renderer and/or MeshFilter components!");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		GlassMaterial = GetComponent<Renderer>().material;
		if (GlassSides && GlassSidesMaterial == null)
		{
			Debug.LogError(base.gameObject.name + ": GlassSide material must be assigned! Glass will be destroyed.");
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Shatter2D(Vector2 HitPoint)
	{
		if (!broken)
		{
			Shatter(HitPoint, base.transform.forward);
		}
	}

	public void Shatter3D(ShatterableGlassInfo Inf)
	{
		if (broken)
		{
			return;
		}
		Transform parent = base.gameObject.transform.parent;
		bool flag = true;
		while (parent != null)
		{
			if (parent.localScale.x != 1f || parent.localScale.y != 1f || parent.localScale.y != 1f)
			{
				flag = false;
			}
			parent = parent.parent;
		}
		if (!flag)
		{
			Debug.LogWarning(base.gameObject.name + ": scale of all parents in hierarchy recommended to be {1, 1, 1}. Glass may shatter weirdly.");
		}
		Vector3 b = base.transform.TransformPoint(new Vector3(-0.5f, -0.5f));
		Vector3 vector = base.transform.TransformPoint(new Vector3(0.5f, -0.5f));
		float num = Vector3.Distance(Inf.HitPoint, b);
		float num2 = Vector3.Distance(vector, b);
		float num3 = Vector3.Distance(Inf.HitPoint, vector);
		float num4 = (num3 + num + num2) / 2f;
		float num5 = Mathf.Sqrt(num4 * (num4 - num3) * (num4 - num) * (num4 - num2));
		float num6 = 2f / num2 * num5;
		float num7 = Mathf.Sqrt(num * num - num6 * num6);
		num6 -= Mathf.Abs(base.transform.lossyScale.y / 2f);
		num7 -= Mathf.Abs(base.transform.lossyScale.x / 2f);
		Shatter(new Vector2(num7 * Mathf.Sign(base.transform.lossyScale.x), num6 * Mathf.Sign(base.transform.lossyScale.y)), Inf.HitDirrection);
	}

	public void Shatter(Vector2 HitPoint, Vector3 ForceDirrection)
	{
		if (broken)
		{
			return;
		}
		broken = true;
		int num = 4 + (Sectors - 1) * 4;
		BaseLine[] array = new BaseLine[num];
		for (int i = 0; i < 4; i++)
		{
			array[i * Sectors] = new BaseLine(HitPoint, Bounds[i], DetailsPerSector);
			float num2 = 1f / (float)Sectors;
			float num3 = num2;
			for (int j = 1; j < Sectors; j++)
			{
				array[i * Sectors + j] = new BaseLine(HitPoint, Vector2.Lerp(Bounds[i], Bounds[(i + 1) % 4], num3), DetailsPerSector);
				num3 += num2;
			}
		}
		List<Figure> list = new List<Figure>();
		for (int k = 0; k < num; k++)
		{
			int num4 = (k + 1) % num;
			float num5 = Vector2.Distance(HitPoint, array[k].Points[DetailsPerSector]);
			float num6 = Vector2.Distance(HitPoint, array[num4].Points[DetailsPerSector]);
			float num7 = Vector2.Distance(array[k].Points[DetailsPerSector], array[num4].Points[DetailsPerSector]);
			float num8 = (num5 + num6 + num7) * 0.5f;
			if (Mathf.Sqrt(num8 * (num8 - num5) * (num8 - num6) * (num8 - num7)) < Area * SimplifyThreshold)
			{
				list.Add(new Figure(new Vector2[3]
				{
					array[k].Points[DetailsPerSector],
					array[num4].Points[DetailsPerSector],
					HitPoint
				}, DetailsPerSector / 2));
				continue;
			}
			list.Add(new Figure(new Vector2[3]
			{
				array[k].Points[1],
				array[num4].Points[1],
				HitPoint
			}, 1));
			for (int l = 1; l < DetailsPerSector; l++)
			{
				list.Add(new Figure(new Vector2[4]
				{
					array[k].Points[l],
					array[(k + 1) % num].Points[l],
					array[k].Points[l + 1],
					array[(k + 1) % num].Points[l + 1]
				}, k + 1));
			}
		}
		foreach (Figure item in list)
		{
			GameObject gameObject = new GameObject("GlassGib");
			gameObject.transform.rotation = base.transform.rotation;
			gameObject.transform.position = base.transform.position;
			if (AdoptFragments)
			{
				gameObject.transform.parent = base.transform.parent;
			}
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			if (GlassSides)
			{
				meshRenderer.materials = new Material[2] { GlassMaterial, GlassSidesMaterial };
			}
			else
			{
				meshRenderer.material = GlassMaterial;
			}
			Mesh sharedMesh = item.GenerateMesh(GlassSides, GlassThickness / 2f, new Vector2(base.transform.lossyScale.x, base.transform.lossyScale.y));
			meshFilter.sharedMesh = sharedMesh;
			if (!ShatterButNotBreak)
			{
				item.GenerateCollider(GlassThickness, gameObject);
				gameObject.AddComponent<Rigidbody>().AddForce(ForceDirrection * UnityEngine.Random.Range(Force, Force * 1.5f) / item.ForceScale);
				if (GibsOnSeparateLayer)
				{
					gameObject.layer = GibsLayer;
				}
				if (DestroyGibs)
				{
					float num9 = AfterSeconds * 0.1f;
					UnityEngine.Object.Destroy(gameObject, UnityEngine.Random.Range(AfterSeconds - num9, AfterSeconds + num9));
				}
			}
			else if (SlightlyRotateGibs)
			{
				gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)));
			}
			gameObject.transform.SetParent(base.transform.parent);
			gameObject.gameObject.layer = LayerMask.NameToLayer("NoPlayerCollide");
		}
		if ((bool)SoundEmitter)
		{
			SoundEmitter.Play();
		}
		UnityEngine.Object.Destroy(GetComponent<Renderer>());
		UnityEngine.Object.Destroy(GetComponent<MeshFilter>());
		UnityEngine.Object.Destroy(GetComponent<ShatterableGlass>());
		if (ShatterButNotBreak)
		{
			base.gameObject.tag = "Untagged";
			return;
		}
		UnityEngine.Object.Destroy(GetComponent<MeshCollider>());
		if ((bool)SoundEmitter)
		{
			if ((bool)SoundEmitter.clip)
			{
				UnityEngine.Object.Destroy(base.gameObject, SoundEmitter.clip.length);
			}
			else
			{
				Debug.Log(base.gameObject.name + ": AudioSource component is present, but SoundClip is not set.");
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
