using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGeneration : MonoBehaviour
{
	public static Vector2[] ClampClipperHole(Vector2[] clipperHole, List<Vector2> borderVertices, float frameLength)
	{
		Vector2 vector = borderVertices[0];
		Vector2 vector2 = borderVertices[0];
		for (int i = 0; i < borderVertices.Count - 1; i++)
		{
			if (borderVertices[i].x > vector.x && borderVertices[i].y > vector.y)
			{
				vector = borderVertices[i];
			}
			if (borderVertices[i].x < vector2.x && borderVertices[i].y < vector2.y)
			{
				vector2 = borderVertices[i];
			}
		}
		int num = 0;
		for (int j = 0; j < clipperHole.Length; j++)
		{
			if (clipperHole[j].x > vector.x - frameLength)
			{
				clipperHole[j].x = vector.x - frameLength;
				clipperHole[j].x += Random.Range(0f - frameLength, frameLength) / 3f;
			}
			if (clipperHole[j].y > vector.y - frameLength)
			{
				clipperHole[j].y = vector.y - frameLength;
				clipperHole[j].y += Random.Range(0f - frameLength, frameLength) / 3f;
			}
			if (clipperHole[j].x < vector2.x + frameLength)
			{
				clipperHole[j].x = vector2.x + frameLength;
				clipperHole[j].x += Random.Range(0f - frameLength, frameLength) / 3f;
			}
			if (clipperHole[j].y < vector2.y + frameLength)
			{
				clipperHole[j].y = vector2.y + frameLength;
				clipperHole[j].y += Random.Range(0f - frameLength, frameLength) / 3f;
			}
		}
		if (num <= clipperHole.Length)
		{
			return clipperHole.Distinct().ToArray();
		}
		MonoBehaviour.print("illegal position");
		return null;
	}

	public static List<ClipSegment> GetSideSegments(int clipI, Vector2[] clipperVertices, List<List<Vector2>> holesList)
	{
		int num = clipI + 1;
		if (num >= clipperVertices.Length)
		{
			num -= clipperVertices.Length;
		}
		List<ClipSegment> list = new List<ClipSegment>();
		for (int i = 0; i < holesList.Count; i++)
		{
			for (int j = 0; j < holesList[i].Count; j++)
			{
				int num2 = j + 1;
				if (num2 >= holesList[i].Count)
				{
					num2 -= holesList[i].Count;
				}
				Vector2 vector = ((j != 6 || clipI != 7) ? Geometry.GetIntersectingPoint(clipperVertices[clipI], clipperVertices[num], holesList[i][j], holesList[i][num2], checkMode: false) : Geometry.GetIntersectingPoint(clipperVertices[clipI], clipperVertices[num], holesList[i][j], holesList[i][num2], checkMode: true));
				if (vector != Vector2.zero)
				{
					bool checkMode = j == 6 && clipI == 7;
					if (Geometry.IsEntering(clipperVertices[clipI], clipperVertices[num], holesList[i][j], holesList[i][num2], checkMode))
					{
						ClipSegment item = TraceSegment(clipperVertices, holesList[i], j, vector);
						list.Add(item);
					}
				}
			}
		}
		return new List<ClipSegment>(BubbleSortClipSegments(list.ToArray(), clipperVertices[clipI]));
	}

	public static ClipSegment TraceSegment(Vector2[] clipperVertices, List<Vector2> holeVerticesList, int startIndex, Vector2 enterPoint)
	{
		ClipSegment clipSegment = new ClipSegment();
		clipSegment.EnterPoint = enterPoint;
		clipSegment.EnterHoleVertice = startIndex;
		clipSegment.Hole = holeVerticesList;
		bool flag = false;
		bool flag2 = true;
		int num = 0;
		while (!flag)
		{
			int num2 = num + startIndex;
			if (num2 >= holeVerticesList.Count)
			{
				num2 -= holeVerticesList.Count;
			}
			int num3 = num2 + 1;
			if (num3 >= holeVerticesList.Count)
			{
				num3 -= holeVerticesList.Count;
			}
			if (!flag2)
			{
				for (int i = 0; i < clipperVertices.Length; i++)
				{
					int num4 = i + 1;
					if (num4 >= clipperVertices.Length)
					{
						num4 -= clipperVertices.Length;
					}
					Vector2 intersectingPoint = Geometry.GetIntersectingPoint(clipperVertices[i], clipperVertices[num4], holeVerticesList[num2], holeVerticesList[num3], checkMode: false);
					if (intersectingPoint != Vector2.zero && !Geometry.IsEntering(clipperVertices[i], clipperVertices[num4], holeVerticesList[num2], holeVerticesList[num3], checkMode: false))
					{
						clipSegment.ExitPoint = intersectingPoint;
						clipSegment.ExitClipperVertice = i;
						flag = true;
					}
				}
			}
			flag2 = false;
			if (flag)
			{
				break;
			}
			clipSegment.SegmentVertices.Add(holeVerticesList[num3]);
			num++;
		}
		return clipSegment;
	}

	public static ClipSegment[] BubbleSortClipSegments(ClipSegment[] clipSegments, Vector2 startPoint)
	{
		int num = clipSegments.Length;
		for (int i = 0; i < num; i++)
		{
			for (int j = i + 1; j < num; j++)
			{
				if (Vector2.Distance(startPoint, clipSegments[i].EnterPoint) > Vector2.Distance(startPoint, clipSegments[j].EnterPoint))
				{
					ClipSegment clipSegment = clipSegments[i];
					clipSegments[i] = clipSegments[j];
					clipSegments[j] = clipSegment;
				}
			}
		}
		return clipSegments;
	}
}
