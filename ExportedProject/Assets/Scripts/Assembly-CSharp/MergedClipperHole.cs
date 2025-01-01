using System.Collections.Generic;
using UnityEngine;

public class MergedClipperHole : MonoBehaviour
{
	private List<Vector2> vertices;

	private List<List<Vector2>> holeRemovalList = new List<List<Vector2>>();

	private List<int> checkedClipVerts = new List<int>();

	private bool intersecting;

	private bool cancel;

	public List<List<Vector2>> HoleRemovalList
	{
		get
		{
			return holeRemovalList;
		}
		set
		{
			holeRemovalList = value;
		}
	}

	public List<int> CheckedClipVerts
	{
		get
		{
			return checkedClipVerts;
		}
		set
		{
			checkedClipVerts = value;
		}
	}

	public bool Intersecting
	{
		get
		{
			return intersecting;
		}
		set
		{
			intersecting = value;
		}
	}

	public List<Vector2> Vertices
	{
		get
		{
			return vertices;
		}
		set
		{
			vertices = value;
		}
	}

	public bool Cancel
	{
		get
		{
			return cancel;
		}
		set
		{
			cancel = value;
		}
	}

	public MergedClipperHole()
	{
	}

	public MergedClipperHole(int startClipI, Vector2[] correctedClipperHole, List<List<Vector2>> holesList)
	{
		vertices = new List<Vector2>();
		int num = startClipI;
		List<int> list = new List<int>();
		while (num < correctedClipperHole.Length && !list.Contains(num))
		{
			list.Add(num);
			if (InsideAHole(correctedClipperHole[num], holesList) == false)
			{
				Vertices.Add(correctedClipperHole[num]);
			}
			else if (!InsideAHole(correctedClipperHole[num], holesList).HasValue)
			{
				cancel = true;
			}
			bool flag = false;
			List<ClipSegment> sideSegments = MeshGeneration.GetSideSegments(num, correctedClipperHole, holesList);
			for (int i = 0; i < sideSegments.Count; i++)
			{
				if (flag)
				{
					break;
				}
				ClipSegment clipSegment = sideSegments[i];
				Vertices.Add(clipSegment.EnterPoint);
				for (int j = 0; j < clipSegment.SegmentVertices.Count; j++)
				{
					Vertices.Add(clipSegment.SegmentVertices[j]);
				}
				Vertices.Add(clipSegment.ExitPoint);
				holeRemovalList.Add(clipSegment.Hole);
				num = clipSegment.ExitClipperVertice;
				flag = true;
				intersecting = true;
			}
			if (!flag)
			{
				num++;
			}
		}
		foreach (List<Vector2> holes in holesList)
		{
			foreach (Vector2 item in holes)
			{
				if (Geometry.IsPointInPolygon(item, Vertices.ToArray()) == true)
				{
					holeRemovalList.Add(holes);
					break;
				}
				if (!Geometry.IsPointInPolygon(item, Vertices.ToArray()).HasValue)
				{
					cancel = true;
				}
			}
		}
		if (!intersecting)
		{
			vertices = new List<Vector2>(correctedClipperHole);
		}
	}

	public bool? InsideAHole(Vector2 point, List<List<Vector2>> holes)
	{
		bool value = false;
		for (int i = 0; i < holes.Count; i++)
		{
			if (Geometry.IsPointInPolygon(point, holes[i].ToArray()) == true)
			{
				return true;
			}
			if (!Geometry.IsPointInPolygon(point, holes[i].ToArray()).HasValue)
			{
				return null;
			}
		}
		return value;
	}
}
